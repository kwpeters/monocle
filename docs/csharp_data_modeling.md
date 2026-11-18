# C# Data Modeling Guide

This guide covers how we model data in C#: domain types, DTOs, unions, and serialization. Where a rule has a reason, the reason is stated so exceptions can be judged sensibly.

The general coding guidelines in [AGENTS.md](../AGENTS.md) apply to all code, including the code referenced here. The working examples live in the repo:

| Example | Shows |
| --- | --- |
| [Person.cs](../libs/ExampleLib/Person.cs) | A domain type with invariants: private constructor, `Create`, members on the type, and a `PersonExt` class for collections |
| [PersonDto.cs](../libs/ExampleLib/PersonDto.cs) | A DTO and its mapping to and from `Person` |
| [Circle.cs](../libs/ExampleLib/Drawing/Circle.cs), [Rect.cs](../libs/ExampleLib/Drawing/Rect.cs), [Label.cs](../libs/ExampleLib/Drawing/Label.cs) | Domain types without invariants |
| [CircleDto.cs](../libs/ExampleLib/Drawing/CircleDto.cs), [RectDto.cs](../libs/ExampleLib/Drawing/RectDto.cs), [LabelDto.cs](../libs/ExampleLib/Drawing/LabelDto.cs) | Their DTOs |
| [Shape.cs](../libs/ExampleLib/Drawing/Shape.cs), [Drawable.cs](../libs/ExampleLib/Drawing/Drawable.cs) | Two domain unions sharing `Circle`; `Shape.Describe` matching over a union |
| [ShapeDto.cs](../libs/ExampleLib/Drawing/ShapeDto.cs), [DrawableDto.cs](../libs/ExampleLib/Drawing/DrawableDto.cs) | Their DTO unions and mapping |
| [JsonOptions.cs](../libs/CsDepot/JsonOptions.cs) | The shared serializer options |
| [TaggedJson.cs](../libs/FnUtil/TaggedJson.cs), [OptionJsonConverter.cs](../libs/FnUtil/OptionJsonConverter.cs), [ResultJsonConverter.cs](../libs/FnUtil/ResultJsonConverter.cs) | How `Option<T>` and `Result<TSuccess, TError>` are serialized |
| [UnionConventions.cs](../libs/CsDepot/UnionConventions.cs) | The union convention checks run by every assembly that defines unions |
| [UnionRegistration.spec.cs](../libs/ExampleLib.Tests/UnionRegistration.spec.cs) | The per-assembly registration test |
| [Person.spec.cs](../libs/ExampleLib.Tests/Person.spec.cs), [PersonDto.spec.cs](../libs/ExampleLib.Tests/PersonDto.spec.cs) | Domain type and DTO tests |
| [Shape.spec.cs](../libs/ExampleLib.Tests/Drawing/Shape.spec.cs), [ShapeDto.spec.cs](../libs/ExampleLib.Tests/Drawing/ShapeDto.spec.cs) | Domain union and DTO union tests |

## 1. Two kinds of type

Every data type is one of two kinds, and the two follow different rules:

| | Domain type | DTO |
| --- | --- | --- |
| Models | The problem: people, shapes, protocol values | A wire format: the JSON a service sends or stores |
| Name | The concept (`Person`) | The concept plus `Dto` (`PersonDto`) |
| Invariants | Enforced. Every instance is valid. | None. Any well-formed JSON deserializes. |
| Members | Factories, constants and pure operations live on the type | Data only |
| Serialized | Never | Always the thing that is serialized |

The two meet at the boundary. Incoming JSON deserializes into a DTO, and the DTO's `ToDomain` maps it into a domain type, validating as it goes. Outgoing data maps from the domain type with `FromDomain`, then serializes. Validation happens once, at that boundary, and every domain value after it is valid.

Domain types never reference DTOs, so the problem model does not depend on any wire format. DTOs reference their domain types only in their mapping code.

## 2. Rules for every type

- Data types are `sealed record class` types. Do not use `record struct`, because `default(T)` bypasses `required` and constructors and yields null reference-type fields.
- All records are `sealed` unless they are deliberately a base type (a union).
- Records are immutable. Properties have `get` and `init` (or `private init`) accessors only. Operations never mutate their inputs; "modifying" operations return a new instance made with a `with` expression.
- Optional values are `Option<T>`, never `T?`. Operations that can fail return `Result<TSuccess, TError>`. Compose them with FnUtil's `Bind`, `MapSuccess`, `Map` and `Match`.
- Use `decimal` for money and other exact quantities. Use `double` only for measurements where floating point is acceptable.

### 2.1 Nominal vs. positional records

| Use | For |
| --- | --- |
| **Nominal** records (named properties) | Domain entities, DTOs, anything with two or more fields, and every type with invariants |
| **Positional** records | Single-value wrappers without invariants (union cases, strongly typed IDs) and small value-like types whose parameters have distinct types, so that argument-order mistakes cannot compile |

Reasons for nominal as the default: call sites are self-documenting, adding or reordering a field cannot silently change the meaning of existing code, and attributes sit directly on properties. A positional record's primary constructor is public, so a type with invariants is always nominal.

## 3. Domain types

### 3.1 Members live on the type

A domain type carries its own behavior. Static factories (`Create`), constants, static instances, and pure instance methods are members of the record, so callers write `Person.Create(...)`, `circle.Area` and `person.WithLastName(...)`. Order them as AGENTS.md describes.

- A cheap derived value with no arguments may be a get-only computed property (`circle.Area`, `person.FullName`). Anything that takes arguments, can fail, or does real work is a method.
- An operation on a type that cannot be given members goes in a static `<Type>Ext` class, declared as an extension method in the same file as the type it concerns. These types are collections (`IEnumerable<Person>`, in `PersonExt.FindByLastName`), delegates, and types from other assemblies.
- Stateful components that must own resources (native handles, connections, channels) are a separate category, not data types. Keep them small, and keep their domain logic in domain types.

### 3.2 Types without invariants

When every combination of property values is valid, the type **may** be publicly constructible with `required` `{ get; init; }` properties, or positional when section 2.1 allows. See [Circle.cs](../libs/ExampleLib/Drawing/Circle.cs).

It may instead follow the pattern in section 3.3: a private constructor and a static factory method, even though the factory cannot fail.
Reasons to choose this include giving the construction a name (`Order.Create(id)`), and being able to add an invariant later without changing every call site.
Whether the factory returns the type itself or a `Result<T, string>` is the author's choice.
Return a `Result` when an invariant is likely to be added, so that adding it changes no signatures.

### 3.3 Types with invariants

When some values are invalid, make invalid instances impossible to create. See [Person.cs](../libs/ExampleLib/Person.cs).

1. Declare a **private constructor** that takes every property value as a parameter. The compiler then makes every caller supply them all.
2. Declare properties as **`{ get; private init; }`**. Code outside the type cannot set them or use `with` on them. Code inside the type can use `with`.
3. Provide a static **`Create`** factory on the type that validates its inputs and returns `Result<T, string>`. It is the only public way to get an instance.
4. Write "modifying" operations to validate the new value first, then copy: `ValidateName(lastName, ...).MapSuccess((last) => this with { LastName = last })`.

A closed set of predefined instances (an enumeration-like type whose values carry data) follows the same pattern. It exposes its instances as `static readonly` fields on the type and, in place of `Create`, a lookup such as `TryFromCode` that returns `Option<T>`.

### 3.4 Domain types and unions

Domain types **never** reference, derive from, or implement a union. Because they know nothing about unions, one domain type can appear in any number of them. `Circle` appears in both `Shape` and `Drawable`.

## 4. DTOs

A DTO is the exact shape of a wire format. See [PersonDto.cs](../libs/ExampleLib/PersonDto.cs).

Create a DTO when a domain type first crosses a JSON boundary. A domain type that is never serialized has no DTO. Adding a DTO later also requires DTOs for everything it contains: each property whose type is a domain type, and, for a domain union, a DTO union with a DTO for every case's payload.

- The name is the concept's name plus `Dto`: `PersonDto`, `CircleDto`, `ShapeDto`. The suffix is how `UnionConventions` tells DTO unions from domain unions, so it is required.
- Properties are public, `required` and `{ get; init; }`. A missing property then fails deserialization instead of silently becoming `default`.
- The record holds data only: no methods and no computed properties. Everything on it is serialized.
- Property types are primitives, other DTOs, DTO unions, collections of those, and `Option<T>`/`Result<TSuccess, TError>` of those. An optional field is a `required Option<T>` property (section 6.4).
- Mapping lives in a static `<Type>DtoExt` class in the same file as the DTO:
  - `public static TDto FromDomain(T value)` builds the DTO from the domain type.
  - `public static Result<T, string> ToDomain(this TDto dto)` builds the domain type, through its `Create` when it has invariants. It returns a `Result` even when the domain type has no invariants today, so adding one later changes no signatures and union mapping composes uniformly.

## 5. Unions

A union is a closed set of cases. In C# it is an abstract record with a private constructor and nested sealed case records. This replaces TypeScript's `A | B | C`. A **domain union** (`Shape`) is a domain type; a **DTO union** (`ShapeDto`) is its wire format and mirrors it case for case.

FnUtil's `Option<T>` and `Result<TSuccess, TError>` are library primitives, not examples of this section. Their cases are internal, created through `F.Some()`, `F.none`, `F.Success()` and `F.Error()`, and handled with `Match` and `Bind` rather than pattern matching. They share the DTO union wire format (section 6.4) but not the structure rules. Leave their structure as it is, and model domain unions on `Shape`.

### 5.1 Structure rules (both kinds)

1. The union is an `abstract record class` with a **`private` parameterless constructor**. Only nested types can derive from it, so the compiler keeps the set of cases closed.
2. Each case is a **nested, public, sealed, positional record** with **exactly one parameter**, named `Value`. Extra data belongs on the wrapped type.
3. Cases are named **`{Concept}Case`**, where the concept is the wrapped type's name without any `Dto` suffix: `Shape.CircleCase(Circle Value)` and `ShapeDto.CircleCase(CircleDto Value)`.
4. Cases are always referenced through the union (`Shape.CircleCase`), so inside a union, `Circle` always means the wrapped type and nothing is shadowed. Write `using` directives for namespaces only, never `using static` on a union.
5. The parameter name is always `Value`, spelled out. It appears in the JSON, in generated TypeScript, and in `with` expressions, and `Value` is the conventional name for a wrapped payload.

### 5.2 Domain unions

- Cases wrap domain types.
- A domain union carries no JSON attributes. Its DTO union carries them.
- Behavior that spans the cases is a method on the union, written as a `switch` on `this` (see `Shape.Describe` in [Shape.cs](../libs/ExampleLib/Drawing/Shape.cs)). Matching rules are in section 5.4.

### 5.3 DTO unions

See [ShapeDto.cs](../libs/ExampleLib/Drawing/ShapeDto.cs).

- The name is the domain union's name plus `Dto`. It has one case per domain union case, with the same case name, wrapping that case's DTO.
- It carries the JSON attributes in section 6.1.
- `<Union>DtoExt.FromDomain` and `ToDomain` map between the two unions, each a `switch` with one arm per case. `ToDomain` maps the payload with its own `ToDomain`, then wraps it: `circleDto.ToDomain().MapSuccess(Shape (Circle circle) => new Shape.CircleCase(circle))`. The explicit lambda return type widens the case to the union type.

### 5.4 Construction, updating and matching

```csharp
Shape shape = new Shape.CircleCase(new Circle { Radius = 2 });

// with works on a concrete case and replaces the whole payload
Shape.CircleCase updated = circleCase with { Value = circleCase.Value with { Radius = 5 } };
```

Every `switch` over a union uses a positional pattern for the case and a property or type pattern for the payload, and ends with a discard arm that throws `UnreachableException` (in `System.Diagnostics`).

That is the only discard arm allowed. List every case explicitly, each with its own arm. A fallback arm hides cases added later, and it will keep hiding them once C# 15's `closed` modifier makes the compiler check exhaustiveness. Migrating to `closed` then means deleting the `UnreachableException` arms.

The compiler cannot check exhaustiveness over an abstract record, so adding a case does **not** break existing `switch` expressions. When you add a case, search for every `switch` on that union, and on its DTO union, and update it. Each such `switch` has a unit test that exercises every case (section 7).

### 5.5 Ad hoc unions

For an operation that returns a value or an error, use FnUtil's `Result<TSuccess, TError>`. For a value that may be absent, use `Option<T>`. Model any other combination as a union following this section. Write unions by hand rather than with union libraries such as OneOf or Dunet; C# 15's `union` keyword will replace them, and they would only need to be migrated away from.

## 6. Serialization

Only DTOs and DTO unions are serialized. A domain type reaches JSON through its DTO's `FromDomain`, and comes back through `ToDomain`.

### 6.1 Wire format

DTO unions are serialized as:

```json
{ "kind": "circle", "value": { "radius": 2 } }
```

- The discriminator property is always `kind`: `[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]`.
- Each case is registered with `[JsonDerivedType(typeof({Union}Dto.{Concept}Case), "{lowerCamelConcept}")]`.
- The payload property is always `value`.
- The `kind` string is the **lowerCamelCase concept name** (`CircleDto` -> `"circle"`), and it is **identical in every union** that contains that concept.
- TypeScript types are **generated from the DTO definitions**, never written by hand. The generated form of a DTO union is `{ kind: "circle"; value: CircleDto }`.
- Flat (internally tagged) payloads such as `{"kind":"circle","radius":2}` need a custom converter. They are used only when an external contract demands them, and each such union is documented as an exception.

### 6.2 Shared serializer options

All serialization uses `CsDepot.JsonOptions.Default` ([JsonOptions.cs](../libs/CsDepot/JsonOptions.cs)), which needs .NET 9 or later. Use that one instance everywhere, and configure any host framework's serializer (for example, ASP.NET Core) with the same settings. `JsonSerializerDefaults.Web` supplies camelCase property names, which produces `value`, `radius`, and so on.

### 6.3 Serialize against the union type

Serialize and deserialize using the DTO union type, not a case type, so the `kind` discriminator is written and read:

```csharp
string json = JsonSerializer.Serialize<ShapeDto>(ShapeDtoExt.FromDomain(shape), JsonOptions.Default);
Option<ShapeDto> dto = JsonSerializer.Deserialize<ShapeDto>(json, JsonOptions.Default).ToOption();
```

- An unknown `kind` throws `NotSupportedException`. For a closed union, failing loudly is the intended behavior.
- Native AOT or trimmed apps need a `JsonSerializerContext` with `[JsonSerializable(typeof(ShapeDto))]` for each DTO union. The attributes in this guide work unchanged with it.

### 6.4 Optional values and results

An optional field in a DTO is a `required Option<T>` property. `Option<T>` and `Result<TSuccess, TError>` use the same tagged `kind`/`value` format as DTO unions. Their converters ([OptionJsonConverter.cs](../libs/FnUtil/OptionJsonConverter.cs), [ResultJsonConverter.cs](../libs/FnUtil/ResultJsonConverter.cs)) are attached to the types, so they need no registration:

```csharp
public sealed record class ContactDto
{
    public required string Name { get; init; }
    public required Option<string> MiddleName { get; init; }
}
```

```json
{ "name": "Jane", "middleName": { "kind": "some", "value": "Q" } }
{ "name": "Jane", "middleName": { "kind": "none" } }
```

| Value | JSON |
| --- | --- |
| `Some(x)` | `{ "kind": "some", "value": x }` |
| `None` | `{ "kind": "none" }` |
| `Success(x)` | `{ "kind": "success", "value": x }` |
| `Error(e)` | `{ "kind": "error", "value": e }` |

- Values nest to any depth. `Some(none)` is `{ "kind": "some", "value": { "kind": "none" } }`, so `Option<Option<T>>` round-trips.
- The property must always appear in the JSON. Because it is `required`, a missing property fails deserialization, even when the value is `None`.
- `null` is never valid: not in place of an `Option` or `Result`, and not as a `value`. A `Result` that wraps null is rejected when written, because it could not be read back.
- `kind` may appear in any position, as with unions.
- The generated TypeScript type is `{ kind: "some"; value: string } | { kind: "none" }`. TypeScript's `?.` and `??` do not work on it, so TypeScript consumers need a shared `Option` helper module (`map`, `withDefault`, and so on) rather than handling each field by hand.

## 7. Required unit tests

These tests are **mandatory**. Name tests `Method_WhenCondition_ExpectedResult`, and order them to match the members under test.

### 7.1 Registration test (one per assembly that defines unions)

`[JsonDerivedType]` is only metadata: the compiler does not check it against the nested cases. A case added without a registration compiles cleanly and fails in production with a `NotSupportedException` the first time it is serialized. This test moves that failure to `dotnet test`.

Every assembly that defines one or more unions has a `UnionRegistrationTests` class in its test project. Copy [UnionRegistration.spec.cs](../libs/ExampleLib.Tests/UnionRegistration.spec.cs) and set `_targetAssembly` to any type from the assembly under test. Add it in the same change that gives an assembly its first union.

The test calls [UnionConventions](../libs/CsDepot/UnionConventions.cs). It finds unions by structure (a top-level abstract class with nested types derived from it), not by attribute, so a DTO union missing `[JsonPolymorphic]` is still found. A union whose name ends in `Dto` is checked as a DTO union; any other union is checked as a domain union. It reports:

- for every union: a parameterless constructor that is not private, and a case that is not public and sealed, has a property other than a single `Value`, or is not named `{Concept}Case`
- for a domain union: any JSON attribute, and a case that wraps a DTO
- for a DTO union: a missing `[JsonPolymorphic]` or a discriminator other than `kind`; a case with no `[JsonDerivedType]` (the common mistake), or a registration for a type that is not a nested case; missing or duplicate `kind` strings; a `kind` that does not match the concept; and a case that wraps something other than a DTO

Limits:

- The checks cover only the assembly passed in. Each assembly with unions needs its own test.
- They check registration and structure, not behavior, and not that a DTO union mirrors its domain union. The mapping test in section 7.5 covers that.
- Because unions are found by structure, an abstract record with nested derived records is treated as a union. Reserve that shape for unions.
- CI must run `dotnet test` and block merges on failure. The tests protect nothing if they are not run.

### 7.2 Domain types

See [Person.spec.cs](../libs/ExampleLib.Tests/Person.spec.cs).

- For `Create`, one test for success and one for each kind of failure.
- A test for each operation, including each failure of an operation that returns a `Result`.

### 7.3 Domain unions

See [Shape.spec.cs](../libs/ExampleLib.Tests/Drawing/Shape.spec.cs).

- A `Samples()` member with one instance of every case. Add a sample whenever a case is added. The DTO union's mapping test uses it.
- A test for every case of every `switch` on the union.

### 7.4 DTOs

See [PersonDto.spec.cs](../libs/ExampleLib.Tests/PersonDto.spec.cs). All serialization tests use `JsonOptions.Default`.

- A round-trip test that also checks the JSON text.
- A malformed-input test for a missing property and a null property. If the serializer options ever lose `RespectRequiredConstructorParameters` or `RespectNullableAnnotations`, this test fails.
- Tests for `FromDomain`, for `ToDomain` success, and for each way `ToDomain` can fail.

### 7.5 DTO unions

See [ShapeDto.spec.cs](../libs/ExampleLib.Tests/Drawing/ShapeDto.spec.cs). All serialization tests use `JsonOptions.Default`.

- A `Samples()` member with one instance of every case, and a round-trip test over it.
- A test that a case is written as `kind` and `value`.
- A malformed-input test for a missing `value`, a null `value`, and an unknown `kind`.
- A mapping round-trip test (domain -> DTO -> JSON -> DTO -> domain) driven by the **domain** union's `Samples()`. A domain case with no matching DTO case or mapping arm then fails here with `UnreachableException`.

## 8. Checklists

### Adding a domain type

1. Create the record in its own file, following section 3.2 or 3.3.
2. Put its factories, constants and pure operations on the type (section 3.1).
3. Add its spec file (section 7.2).

### Adding a DTO

1. Create `{Type}Dto` in its own file, following section 4.
2. Add `{Type}DtoExt` with `FromDomain` and `ToDomain` in the same file.
3. Add its spec file (section 7.4).

### Adding a union

1. Create the domain union: `public abstract record class {Name}` with a `private {Name}() { }` constructor and one nested `public sealed record class {Concept}Case({Concept} Value) : {Name};` per case.
2. Put behavior that spans the cases on the union (section 5.2).
3. If it is serialized, create `{Name}Dto` with `[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]`, one `{Concept}Case({Concept}Dto Value)` per case, and one `[JsonDerivedType(typeof({Name}Dto.{Concept}Case), "{lowerCamelConcept}")]` per case. Add `{Name}DtoExt` with `FromDomain` and `ToDomain`.
4. Confirm the assembly's test project has `UnionRegistrationTests` (section 7.1); add it if this is the assembly's first union.
5. Add spec files for the domain union (section 7.3) and the DTO union (section 7.5).

### Adding a case to an existing union

1. Add the nested `{Concept}Case` record to the domain union.
2. If the union has a DTO union, add the matching case there, and its `[JsonDerivedType]` entry, reusing the existing `kind` string for that concept.
3. Search for every `switch` on both unions (normally the union's methods and `{Name}DtoExt`) and add an arm, including both mapping arms. The compiler will not flag the missing arms.
4. Add a sample to both unions' `Samples()`, and a test for each new `switch` arm.
5. Run `dotnet test`. `UnionRegistrationTests` fails if the registration in step 2 was forgotten, and the mapping round-trip test fails if a mapping arm was.
