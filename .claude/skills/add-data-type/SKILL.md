---
name: add-data-type
description: Creates C# data types in this repo following its data modeling rules, along with their required unit tests. Use when adding a record type and its Ext class, adding a discriminated union, adding a case to an existing union, or adding a type that is serialized to JSON.
---

# Add a data type

Paths are relative to the repository root. Before starting, read
`docs/csharp_data_modeling.md`. These steps say what to do; the guide says how and
why. All code follows `AGENTS.md`.

Choose the workflow:

- **A. New record type**: a plain data type with its operations.
- **B. New union**: a closed set of cases, each wrapping a domain type.
- **C. New case on an existing union.**

Every union case wraps a domain type. If that type does not exist yet, do
workflow A for it first.

## A. New record type

Model it on `libs/ExampleLib/Person.cs`.

1. Create `<Type>.cs` in the library folder that matches its namespace.
2. Declare `public sealed record class <Type>` with `required` `{ get; init; }`
   properties and no methods. Use nominal syntax unless the type is a
   single-value wrapper or a small type whose parameters have distinct types
   (guide section 2.1).
3. In the same file, declare `public static class <Type>Ext`. Write each operation
   as a pure extension method (`this <Type> value`). "Modifying" operations return
   a copy made with `with`. Operations that can fail return `Result`; lookups that
   may find nothing return `Option`. Computed values are `Ext` methods, never
   properties on the record.
4. If the type has invariants, add a `Create` factory to `<Type>Ext` that returns
   `Result<<Type>, string>`. Compose validations the way `PersonExt.Create` does,
   and reuse `CsDepot.StringValidators` and `CsDepot.StringNormalizers` where they
   fit.
5. Add `<Library>.Tests/<folder>/<Type>.spec.cs` with class `<Type>Tests`. Follow
   `libs/CsDepot.Tests/StringValidators.spec.cs`:
   - one banner comment per method under test, in the same order as the methods in
     `<Type>Ext`;
   - test names like `Method_WhenCondition_ExpectedResult`;
   - for `Create`, one test for success and one for each kind of failure.

If the library has no test project, create one by copying
`libs/ExampleLib.Tests/ExampleLib.Tests.csproj`, `.editorconfig` and
`GlobalUsings.cs`. Point its project references at the library, and add it to
`monocle.slnx`.

## B. New union

Model it on `libs/ExampleLib/Drawing/Shape.cs`.

1. Create `<Union>.cs` and declare `public abstract record class <Union>` with a
   `private <Union>() { }` constructor.
2. For each case, add a nested
   `public sealed record class <DomainType>Case(<DomainType> Value) : <Union>;`.
3. Add `[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]`, and one
   `[JsonDerivedType(typeof(<Union>.<DomainType>Case), "<domainType>")]` per case.
   The kind is the lowerCamelCase domain type name. If the domain type already
   appears in another union, reuse that union's kind string.
4. Put behavior that spans the cases in `<Union>Ext` in the same file. Write each
   operation as a `switch` expression with a positional pattern for the case and a
   property pattern for the payload, ending with
   `_ => throw new UnreachableException()`.
5. If `<Library>.Tests` has no `UnionRegistration.spec.cs`, copy
   `libs/ExampleLib.Tests/UnionRegistration.spec.cs`, fix its namespace, and
   set `_targetAssembly` to a type from the library.
6. Add `<Library>.Tests/<folder>/<Union>.spec.cs`, modeled on
   `libs/ExampleLib.Tests/Drawing/Shape.spec.cs`:
   - `Samples()` with one instance of every case;
   - a round-trip test over the samples, using `CsDepot.JsonOptions.Default`;
   - a malformed-input test for a missing `value`, a null `value`, and an unknown
     `kind`;
   - a test for every case of every `switch` in `<Union>Ext`.

## C. New case on an existing union

1. Add the nested `<DomainType>Case` record to the union.
2. Add its `[JsonDerivedType]` attribute. Reuse the kind string if the domain type
   already appears in another union.
3. Find every `switch` on the union. The compiler does not flag missing arms, and
   switches can live outside `<Union>Ext`, so search the whole repo for the union's
   case names (for example, `grep -rn "<Union>\.[A-Za-z]*Case" --include=*.cs`).
   Add an arm to each.
4. In the union's spec file, add a sample to `Samples()` and a test for each new
   arm.

## Finish

Run `dotnet build` and `dotnet test`. Warnings are errors, so the build also
enforces the style rules in `.editorconfig`. If `UnionRegistrationTests` fails, its
message names the union, the case, and the broken convention.
