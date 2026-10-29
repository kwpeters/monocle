---
name: add-data-type
description: Creates C# data types in this repo following its data modeling rules, along with their required unit tests. Use when adding a domain type, a DTO, a discriminated union, or a case to an existing union, or when serializing a type to JSON.
---

# Add a data type

Paths are relative to the repository root. Before starting, read
`docs/csharp_data_modeling.md`. These steps say what to do; the guide says how and
why, and the section numbers below point into it. All code follows `AGENTS.md`.

Every data type is a **domain type** (models the problem, never serialized) or a
**DTO** (models a wire format, name ends in `Dto`). Choose the workflow:

- **A. New domain type**: a record with its behavior on the type.
- **B. New DTO**: the JSON shape of a domain type, with its mapping.
- **C. New union**: a domain union, plus its DTO union when it is serialized.
- **D. New case on an existing union.**

A type that is serialized needs both: do workflow A for the domain type, then B
for its DTO. Every union case wraps a type that already exists; do A (and B) for
it first.

## A. New domain type

Model it on `libs/ExampleLib/Person.cs` (with invariants) or
`libs/ExampleLib/Drawing/Circle.cs` (without).

1. Create `<Type>.cs` in the library folder that matches its namespace, declaring
   `public sealed record class <Type>`.
2. Shape it by whether it has invariants (guide 3.2, 3.3):
   - **None**: `required` `{ get; init; }` properties, or positional when guide
     2.1 allows.
   - **Some**: a private constructor taking every value, `{ get; private init; }`
     properties, and a static `Create` on the type returning
     `Result<<Type>, string>`. Compose validations the way `Person.Create` does,
     and reuse `CsDepot.StringValidators` and `CsDepot.StringNormalizers` where
     they fit.
3. Put factories, constants and pure operations on the type (guide 3.1). An
   operation on a collection of the type, or on a delegate, goes in a
   `<Type>Ext` class in the same file.
4. Add `<Library>.Tests/<folder>/<Type>.spec.cs` with class `<Type>Tests`
   (guide 7.2), modeled on `libs/ExampleLib.Tests/Person.spec.cs`, with one
   banner comment per member under test, in the order the members are declared.

## B. New DTO

Model it on `libs/ExampleLib/PersonDto.cs`.

1. Create `<Type>Dto.cs` beside the domain type, declaring
   `public sealed record class <Type>Dto` with public `required` `{ get; init; }`
   properties matching the JSON, and nothing else (guide 4). An optional field is
   a `required Option<T>` property (guide 6.4).
2. In the same file, declare `public static class <Type>DtoExt` with
   `FromDomain(<Type>)` and `ToDomain(this <Type>Dto)` returning
   `Result<<Type>, string>`, built through `<Type>.Create` when the domain type
   has invariants.
3. Add `<Type>Dto.spec.cs` (guide 7.4), modeled on
   `libs/ExampleLib.Tests/PersonDto.spec.cs`.

## C. New union

Model it on `libs/ExampleLib/Drawing/Shape.cs` and `ShapeDto.cs`.

1. Create `<Union>.cs`: `public abstract record class <Union>` with a
   `private <Union>() { }` constructor and, per case, a nested
   `public sealed record class <Concept>Case(<Concept> Value) : <Union>;`.
   It carries no JSON attributes (guide 5.2).
2. Put behavior that spans the cases on the union, as a `switch` on `this` with a
   positional pattern per case, ending with `_ => throw new UnreachableException()`.
   That is the only discard arm: list every case explicitly (guide 5.4).
3. If the union is serialized, create `<Union>Dto.cs` (guide 5.3):
   - `[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]` on
     `public abstract record class <Union>Dto` with a private constructor;
   - per case, `public sealed record class <Concept>Case(<Concept>Dto Value) : <Union>Dto;`
     and `[JsonDerivedType(typeof(<Union>Dto.<Concept>Case), "<concept>")]`, where
     the kind is the lowerCamelCase concept name. If the concept already appears
     in another DTO union, reuse that union's kind string;
   - `<Union>DtoExt` with `FromDomain` and `ToDomain`, one arm per case.
4. If `<Library>.Tests` has no `UnionRegistration.spec.cs`, copy
   `libs/ExampleLib.Tests/UnionRegistration.spec.cs`, fix its namespace, and set
   `_targetAssembly` to a type from the library.
5. Add `<Union>.spec.cs` (guide 7.3) with `Samples()` holding one instance of
   every case, and a test for every `switch` arm.
6. If step 3 applied, add `<Union>Dto.spec.cs` (guide 7.5), modeled on
   `libs/ExampleLib.Tests/Drawing/ShapeDto.spec.cs`, including the mapping
   round-trip test driven by `<Union>Tests.Samples`.

## D. New case on an existing union

1. Add the nested `<Concept>Case` record to the domain union.
2. If a `<Union>Dto` exists, add the matching case and its `[JsonDerivedType]`
   attribute there, reusing the kind string if the concept appears in another
   DTO union.
3. Find every `switch` on both unions. The compiler does not flag missing arms,
   and switches can live outside the union and `<Union>DtoExt`, so search the
   whole repo for the unions' case names (for example,
   `grep -rnE "<Union>(Dto)?\.[A-Za-z]*Case" --include=*.cs`). Add an explicit arm
   to each, before the `UnreachableException` arm. If a switch handles cases
   with any other discard arm, replace it with explicit arms.
4. Add a sample to `<Union>Tests.Samples()` and, if it exists,
   `<Union>DtoTests.Samples()`, plus a test for each new arm.

## Finish

Run `dotnet build` and `dotnet test`. Done means both pass. Warnings are errors,
so the build also enforces the style rules in `.editorconfig`. If
`UnionRegistrationTests` fails, its message names the union, the case, and the
broken convention. If a library got a new test project, it must be listed in the
repository's `.slnx` file; copy `libs/ExampleLib.Tests/ExampleLib.Tests.csproj`
and `GlobalUsings.cs` to create one.
