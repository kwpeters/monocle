# C# Data Modeling Guide

This guide covers how we model data, operations, unions, and serialization in C#. The rules are meant to be followed mechanically by both humans and AI agents. Where a rule has a reason, the reason is stated so exceptions can be judged sensibly.

The general coding guidelines in [AGENTS.md](../AGENTS.md) apply to all code, including the code referenced here. The working examples live in the repo:

| Example | Shows |
| --- | --- |
| [Person.cs](../libs/ExampleLib/Person.cs) | A record with a `PersonExt` class, including a validating `Create` factory |
| [Circle.cs](../libs/ExampleLib/Drawing/Circle.cs), [Rect.cs](../libs/ExampleLib/Drawing/Rect.cs), [Label.cs](../libs/ExampleLib/Drawing/Label.cs) | Domain types |
| [Shape.cs](../libs/ExampleLib/Drawing/Shape.cs), [Drawable.cs](../libs/ExampleLib/Drawing/Drawable.cs) | Two unions sharing `Circle`; `ShapeExt` matching over a union |
| [JsonOptions.cs](../libs/CsDepot/JsonOptions.cs) | The shared serializer options |
| [TaggedJson.cs](../libs/FnUtil/TaggedJson.cs), [OptionJsonConverter.cs](../libs/FnUtil/OptionJsonConverter.cs), [ResultJsonConverter.cs](../libs/FnUtil/ResultJsonConverter.cs) | How `Option<T>` and `Result<TSuccess, TError>` are serialized |
| [UnionConventions.cs](../libs/CsDepot/UnionConventions.cs) | The union convention checks run by every assembly that defines unions |
| [UnionRegistration.spec.cs](../libs/ExampleLib.Tests/UnionRegistration.spec.cs) | The per-assembly registration test |
| [Shape.spec.cs](../libs/ExampleLib.Tests/Drawing/Shape.spec.cs) | The per-union serialization and matching tests |

## 1. Project settings

- The serializer options required by this guide (`AllowOutOfOrderMetadataProperties`, `RespectRequiredConstructorParameters`, `RespectNullableAnnotations`) need .NET 9 or later.
- Use `decimal` for money and other exact quantities. Use `double` only for measurements where floating point is acceptable.
- Computed values are operations in the type's `<Type>Ext` class (`circle.Area()`), never properties on the record. A computed property would be serialized into JSON by default; an extension method will not.

## 2. Data and operations

Keep data and behavior separate. Data is defined as immutable records, and operations on that data are pure functions in a companion static class. See [Person.cs](../libs/ExampleLib/Person.cs).

- **Data types** are `sealed record class` types using nominal syntax with `required` `{ get; init; }` properties. Records hold data only; they contain no methods beyond compiler-generated members.
  - Do not use `record struct`, because `default(T)` bypasses `required` and yields null reference-type fields.
  - All records are `sealed` unless they are deliberately a base type (for example, a union).
- **Operations** live in a static class named `<Type>Ext` (e.g. `PersonExt`), declared in the same file as the record.
  - Operations are pure functions and never mutate their inputs. "Modifying" operations return a new instance using a `with` expression.
  - Declare operations as extension methods on the record so they can be called either as functions (`PersonExt.FullName(p)`) or fluently (`p.FullName()`).
  - When a type has invariants, provide a validating factory named `Create` that returns `Result<T, TError>`, and create instances through it. `Create` lives in the `<Type>Ext` class even though it is not an extension method.
  - Operations that can fail return `Result<TSuccess, TError>`. Lookups that may find nothing return `Option<T>`. Compose them with FnUtil's `Bind`, `MapSuccess`, `Map` and `Match`.
- Stateful components that must own resources (native handles, connections, channels) are the exception. Keep them small, and keep their domain logic in records and `<Type>Ext` functions.

### 2.1 Nominal vs. positional records

| Use | For |
| --- | --- |
| **Nominal** `required` / `init` records | Domain entities, DTOs, anything with two or more fields, anything crossing a serialization boundary |
| **Positional** records | Single-value wrappers (union cases, strongly typed IDs) and small value-like types whose parameters have distinct types, so that argument-order mistakes cannot compile |

Reasons for nominal as the default: call sites are self-documenting, adding or reordering a field cannot silently change the meaning of existing code, attributes sit directly on properties, and missing `required` values fail deserialization instead of becoming `default`.

## 3. Domain types

- Domain records are plain records. They **never** reference, derive from, or implement a union.
- Behavior that is intrinsic to a domain type (area, validation) lives in that type's `<Type>Ext` class.
- Because domain types know nothing about unions, one domain type can appear in any number of unions. `Circle` appears in both `Shape` and `Drawable`.

## 4. Unions

A union is a closed set of cases. In C# it is an abstract record with a private constructor and nested sealed case records. This replaces TypeScript's `A | B | C`. See [Shape.cs](../libs/ExampleLib/Drawing/Shape.cs).

FnUtil's `Option<T>` and `Result<TSuccess, TError>` are library primitives, not examples of this section. Their cases are internal, created through `F.Some()`, `F.none`, `F.Success()` and `F.Error()`, and handled with `Match` and `Bind` rather than pattern matching. They share the union wire format (section 5.5) but not the structure rules. Do not change them to follow these rules, and do not copy their structure into domain unions.

### 4.1 Structure rules

1. The union is an `abstract record class` with a **`private` parameterless constructor**. Only nested types can derive from it, so the compiler keeps the set of cases closed.
2. Each case is a **nested, public, sealed, positional record** with **exactly one parameter**, named `Value`, whose type is a domain type.
3. A case never carries more than one value. Extra data belongs on the domain type.
4. Cases are named **`{DomainType}Case`** and are always referenced through the union: `Shape.CircleCase`. Inside a union, `Circle` always means the domain type, so nothing is shadowed.
5. The parameter name is always `Value`, spelled out. Do not abbreviate it (`V`, `Dv`). It appears in the JSON, in generated TypeScript, and in `with` expressions, and `Value` is the conventional name for a wrapped payload.
6. Do not use `using static` on a union.
7. Every union carries the JSON attributes (section 5), even if it is not serialized today. The required tests (section 6) fail any union without them, so a forgotten attribute is caught.

### 4.2 Construction and updating

```csharp
Shape shape = new Shape.CircleCase(new Circle { Radius = 2 });

// with works on a concrete case and replaces the whole payload
Shape.CircleCase updated = circleCase with { Value = circleCase.Value with { Radius = 5 } };
```

### 4.3 Matching

Behavior that spans a union's cases is an operation in the union's `<Union>Ext` class, written as a `switch` expression (see `ShapeExt.Describe` in [Shape.cs](../libs/ExampleLib/Drawing/Shape.cs)). Use a positional pattern for the case and a property pattern for the payload. Every such `switch` ends with a discard arm that throws `UnreachableException` (in `System.Diagnostics`).

That is the only discard arm allowed. List every case explicitly; never handle cases with a `_` arm or a `default:` label that has real behavior. A fallback arm hides cases added later, and it will keep hiding them once C# 15's `closed` modifier makes the compiler check exhaustiveness. Migrating to `closed` then means deleting the `UnreachableException` arms.

The compiler cannot check exhaustiveness over an abstract record, so adding a case does **not** break existing `switch` expressions. When you add a case, search for every `switch` on that union and update it. Each such `switch` has a unit test that exercises every case.

### 4.4 Ad hoc unions

For an operation that returns a value or an error, use FnUtil's `Result<TSuccess, TError>`. For a value that may be absent, use `Option<T>`. Model any other combination as a union following this section. Do not add union libraries such as OneOf or Dunet; C# 15's `union` keyword will replace them, and they would only need to be migrated away from.

## 5. Serialization

### 5.1 Wire format

Unions are serialized as:

```json
{ "kind": "circle", "value": { "radius": 2 } }
```

- The discriminator property is always `kind`: `[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]`.
- Each case is registered with `[JsonDerivedType(typeof({Union}.{DomainType}Case), "{lowerCamelDomainType}")]`.
- The payload property is always `value`.
- The `kind` string is the **lowerCamelCase name of the domain type** (`Circle` -> `"circle"`), and it is **identical in every union** that contains that domain type.
- TypeScript types for these unions are **generated from the C# definitions**, never written by hand. The generated form is `{ kind: "circle"; value: Circle }`.
- Flat (internally tagged) payloads such as `{"kind":"circle","radius":2}` need a custom converter. They are used only when an external contract demands them, and each such union is documented as an exception.

### 5.2 Shared serializer options

All serialization uses `CsDepot.JsonOptions.Default` ([JsonOptions.cs](../libs/CsDepot/JsonOptions.cs)). Never create another `JsonSerializerOptions` instance, and configure any host framework's serializer (for example, ASP.NET Core) with the same settings. `JsonSerializerDefaults.Web` supplies camelCase property names, which produces `value`, `radius`, and so on.

### 5.3 Serialize against the union type

Serialize and deserialize using the union type, not a case type, so the `kind` discriminator is written and read:

```csharp
string json = JsonSerializer.Serialize<Shape>(shape, JsonOptions.Default);
Option<Shape> copy = JsonSerializer.Deserialize<Shape>(json, JsonOptions.Default).ToOption();
```

### 5.4 Other behavior to know

- An unknown `kind` throws `NotSupportedException`. For a closed union, failing loudly is the intended behavior.
- Native AOT or trimmed apps need a `JsonSerializerContext` with `[JsonSerializable(typeof(Shape))]` for each union. The attributes in this guide work unchanged with it.

### 5.5 Optional values and results

An optional field in a serialized type is a `required Option<T>` property, never a `T?`. `Option<T>` and `Result<TSuccess, TError>` use the same tagged `kind`/`value` format as unions. Their converters ([OptionJsonConverter.cs](../libs/FnUtil/OptionJsonConverter.cs), [ResultJsonConverter.cs](../libs/FnUtil/ResultJsonConverter.cs)) are attached to the types, so they need no registration:

```csharp
public sealed record class Contact
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

## 6. Required unit tests

These tests are **mandatory**. `[JsonDerivedType]` is only metadata: the compiler does not check it against the nested cases. A case added without a registration compiles cleanly and fails in production with a `NotSupportedException` the first time it is serialized. The tests move that failure to `dotnet test`.

### 6.1 Registration test (one per assembly that defines unions)

Every assembly that defines one or more unions has a `UnionRegistrationTests` class in its test project. Copy [UnionRegistration.spec.cs](../libs/ExampleLib.Tests/UnionRegistration.spec.cs) and set `_targetAssembly` to any type from the assembly under test. Add it in the same change that gives an assembly its first union.

The test calls [UnionConventions](../libs/CsDepot/UnionConventions.cs), which finds unions by structure (a top-level abstract class with nested types derived from it), not by attribute, so a union missing `[JsonPolymorphic]` is still found. It reports:

- a missing `[JsonPolymorphic]`, or a discriminator other than `kind`
- a union whose parameterless constructor is not private
- a case with no `[JsonDerivedType]` (the common mistake), or a registration for a type that is not a nested case
- missing or duplicate `kind` strings
- a case that is not public and sealed, has a property other than a single `Value`, is not named `{DomainType}Case`, or has a `kind` that does not match its domain type

Limits:

- The checks cover only the assembly passed in. Each assembly with unions needs its own test.
- They check registration and structure, not behavior. Section 6.2 covers behavior.
- Because unions are found by structure, an abstract record with nested derived records is treated as a union. Do not use that shape for anything else.
- CI must run `dotnet test` and block merges on failure. The tests protect nothing if they are not run.

### 6.2 Serialization and matching tests (one per union)

Each union has a spec file like [Shape.spec.cs](../libs/ExampleLib.Tests/Drawing/Shape.spec.cs), using `JsonOptions.Default`:

- A `Samples()` member with one instance of every case. Add a sample whenever a case is added.
- A round-trip test over the samples.
- A malformed-input test for a missing `value`, a null `value`, and an unknown `kind`. If the serializer options ever lose `RespectRequiredConstructorParameters` or `RespectNullableAnnotations`, this test fails.
- A test for every case of every `switch` in the union's `<Union>Ext` class.

## 7. Checklists

### Adding a union

1. Create `public abstract record class {Name}` with a `private {Name}() { }` constructor.
2. Add `[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]`.
3. Add one nested `public sealed record class {DomainType}Case({DomainType} Value) : {Name};` per case.
4. Add one `[JsonDerivedType(typeof({Name}.{DomainType}Case), "{lowerCamelDomainType}")]` per case.
5. Put behavior that spans the cases in a `{Name}Ext` class (section 4.3).
6. Confirm the assembly's test project has `UnionRegistrationTests` (section 6.1); add it if this is the assembly's first union.
7. Add a spec file with a sample per case (section 6.2).

### Adding a case to an existing union

1. Add the nested `{DomainType}Case` record.
2. Add its `[JsonDerivedType]` entry, reusing the existing `kind` string for that domain type.
3. Search for every `switch` on the union (normally in `{Name}Ext`) and add an arm. The compiler will not flag the missing arms.
4. Add a sample, and a test for each new `switch` arm, to the union's spec file.
5. Run `dotnet test`. `UnionRegistrationTests` fails if step 2 was forgotten.
