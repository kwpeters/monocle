# Coding Guidelines

## Code Style

- **.NET 10**, nullable reference types enabled, implicit usings enabled.
- **File-scoped namespaces** — enforced as error in `.editorconfig`.
- **Explicit types instead of `var`**, unless the type is apparent from the right-hand side (e.g. `var items = new List<string>()`) — enforced as error (IDE0008). This includes `foreach` loops and deconstructions (`(int a, string b) = ...`). Write the non-nullable type (`Option<int>`, `T`), not the nullable type that `var` would infer (`Option<int>?`, `T?`). Use `?` only where the value really can be null, such as the `out` value of `Dictionary.TryGetValue()`.
- **Explicit integer type sizes** (`Int32`, `UInt16`, `Byte`) in CIP/protocol code where bit widths matter. Standard `int`/`byte` elsewhere is fine.
- **Braces always required** — no single-line `if` without braces.
- **Two blank lines between members** (methods, properties, fields with attributes, test methods). Use one blank line only within a member's body.
- **Private fields**: `_camelCase` with underscore prefix.
- **Physical quantities**: include the units.  For example, `timeoutMs`.
- See [.editorconfig](.editorconfig) for all formatting and style rules.

### If Statements and Loops

- If statements and loops must use braces. The sole exception is when the body contains only a `return` statement.

### Lambdas

- Lambda parameters should be wrapped in parentheses when there are multiple parameters. Single-parameter lambdas may omit parentheses.

### Ternary Formatting

Keep a ternary on a single line when it fits within 100 columns:

```csharp
return input.Count > 0 ? Success(input) : Error(errorFactory());
```

When a ternary spans multiple lines, the `?` and `:` should appear at the end of the line (not the beginning of the next line). For example:

```csharp
var result = condition ?
    valueIfTrue :
    valueIfFalse;
```

### Multi-line Method Signatures

When a method signature is too long for a single line, keep the return type (including `async` and other modifiers) on one line and start the method name on the next:

```csharp
public static Result<int, string>
Initialize(
    uint paramA,
    string paramB
)
```

### Class and Record Member Ordering

Members within a class or record must be declared in the following order. Use the following separator comments to delineate the sections of a class or record.  If a class or record only contains one of these sections (as is often the case in unit test files), the separator comment is not required. Omit sections that are not needed.

```csharp
    //------------------------------------------------------------------------------
    // Constants
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Static fields
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Static properties
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Instance fields
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------
```

## Patterns

### Functional Programming

- Always prefer use of the functional data types and patterns defined in the FnUtil library.
- Never use null references.  Use Option<T> from FnUtil instead.
- Reserve the throwing of exceptions for truly exception cases (cases that should never happen).
- For operations that can fail and communicating the error reason is desirable,
  prefer returning `Result<TSuccess, TError>` over throwing exceptions.  This allows
  callers to handle errors without try/catch and preserves error information without
  loss from exception types or messages.
- For operations that can fail but communicating the error reason is not needed,
  `Option<T>` is appropriate.  This allows callers to handle the presence or absence
  of a value without try/catch and without using null references.

### Data and Operations

Keep data and behavior separate. Data is defined as immutable records, and
operations on that data are pure functions in a companion static class.
[Person.cs](libs/ExperimentalLib/Person.cs) is the reference example.

- **Data types** are `sealed record class` types using nominal syntax with
  `required` `{ get; init; }` properties. Records hold data only; they contain no
  methods beyond compiler-generated members.
  - Do not use `record struct`, because `default(T)` bypasses `required` and yields
    null reference-type fields.
  - Positional syntax (`record Foo(int A, string B)`) is acceptable only for small
    value-like types whose parameters have distinct types, so that argument-order
    mistakes cannot compile.
- **Operations** live in a static class named `<Type>Ops` (e.g. `PersonOps`),
  declared in the same file as the record.
  - Operations are pure functions and never mutate their inputs. "Modifying"
    operations return a new instance using a `with` expression.
  - Declare operations as extension methods on the record so they can be called
    either as functions (`PersonOps.FullName(p)`) or fluently (`p.FullName()`).
  - When a type has invariants, provide a validating factory named `Create` that
    returns `Result<T, TError>`, and create instances through it.
  - Operations that can fail return `Result<TSuccess, TError>`. Lookups that may
    find nothing return `Option<T>`. Compose them with FnUtil's `Bind`,
    `MapSuccess`, `Map` and `Match`.
- Stateful components that must own resources (native handles, connections,
  channels) are the exception. Keep them small, and keep their domain logic in
  records and `<Type>Ops` functions.

```csharp
public sealed record class Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}

public static class PersonOps
{
    public static Result<Person, string>
    Create(string firstName, string lastName) => ...;

    public static string
    FullName(this Person person) => $"{person.FirstName} {person.LastName}";

    public static Result<Person, string>
    WithLastName(this Person person, string lastName) =>
        NonBlank(lastName, nameof(lastName))
        .MapSuccess((last) => person with { LastName = last });
}
```

### Asynchronous Code

- Use `ConfigureAwait(false)` on all `await` calls in library code.
- **Method naming for async operations:** The `Async` suffix is used only when a **callback/function parameter is async**, not merely because the input or output is a `Task`. For example:
  - `Exists(Task<Result>, Func<T, bool>)` — no Async suffix; Task input but sync predicate
  - `ExistsAsync(Task<Result>, Func<T, Task<bool>>)` — Async suffix; async predicate
  - `FoldAsync(Result, Func<S, T, Task<S>>, S)` — Async suffix; async folder function
  - `Fold(Task<Result>, Func<S, T, S>, S)` — no Async suffix; Task input but sync folder

### Unit Tests

- Order the unit tests in a spec file to match the order of the methods under test in the implementation file.

### P/Invoke

- Use `LibraryImport` (source-generated) with `__stdcall` calling convention.
- Pin callback delegates as instance fields to prevent GC collection.
- Structs use `[StructLayout(LayoutKind.Sequential, Pack = 1)]`.

### Concurrency

- `ConcurrentDictionary` for tracking pending async requests and open connections.
- `Channel<T>` for streaming packets on connected CIP sessions.
- `TaskCompletionSource` for converting native callbacks into `Task<T>`.

## Build and Test

```shell
dotnet build
dotnet test
```
