# Coding Guidelines

## Guides

The rules below apply to all code. These guides add detailed rules for specific
kinds of work. Read a guide only when the task calls for it.

| Read | When you are |
| --- | --- |
| [docs/csharp_data_modeling.md](docs/csharp_data_modeling.md) | Modeling a new type, adding or changing a domain type, a DTO, a union, or their tests, or serializing a type to JSON |
| [docs/csharp_pinvoke.md](docs/csharp_pinvoke.md) | Calling native code, wrapping a callback-based native library, or tracking native requests and connections |
| A project's `README.md`, and the root [README.md](README.md) | Working in a project. A project's README.md, if it has one, describes the domain the project models. The root README.md lists all domains, including ones no project models yet, and how they relate. When you add a project that models a domain, give it a README.md and add the domain to the root list. |

## Code Style

- **.NET 10**, nullable reference types enabled, implicit usings enabled.
- **File-scoped namespaces** — enforced as error in `.editorconfig`.
- **Explicit types instead of `var`**, unless the type is apparent from the right-hand side (e.g. `var items = new List<string>()`) — enforced as error (IDE0008). This includes `foreach` loops and deconstructions (`(int a, string b) = ...`). Write the non-nullable type (`Option<int>`, `T`), not the nullable type that `var` would infer (`Option<int>?`, `T?`). Use `?` only where the value really can be null, such as the `out` value of `Dictionary.TryGetValue()`.
- **Braces are required** on `if` statements and loops. The sole exception is a body that contains only a `return` statement, which may go on the same line (`if (done) return;`) or the next line.
- **Two blank lines between members** (methods, properties, fields with attributes, test methods). Use one blank line only within a member's body.
- **Private fields**: `_camelCase` with underscore prefix.
- **Physical quantities**: include the units.  For example, `timeoutMs`.
- See [.editorconfig](.editorconfig) for all formatting and style rules.

### Lambdas

- Lambda parameters should be wrapped in parentheses when there are multiple parameters. Single-parameter lambdas may omit parentheses.

### Ternary Formatting

Keep a ternary on a single line when it fits within 100 columns:

```csharp
return input.Count > 0 ? Success(input) : Error(errorFactory());
```

When a ternary spans multiple lines, the `?` and `:` should appear at the end of the line (not the beginning of the next line). For example:

```csharp
string result = condition ?
    valueIfTrue :
    valueIfFalse;
```

### Method Signatures

Keep a method signature on a single line when it fits within 100 columns. When it does not, keep the return type (including `async` and other modifiers) on one line and start the method name on the next:

```csharp
public static Result<int, string>
Initialize(
    uint paramA,
    string paramB
)
```

### Class and Record Member Ordering

Declare members in this order, omitting sections that are not needed: Constants, Static fields, Static properties, Static methods, Instance fields, Constructors, Properties, Instance methods, Nested types.

Start each section with a separator comment naming it. The separators may be omitted when a class or record has only one section (as is often the case in unit test files).

```csharp
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------
```

## Patterns

### Functional Programming

- Always prefer use of the functional data types and patterns defined in the FnUtil library.
- Never use null references.  Use `Option<T>` from FnUtil instead.
- Reserve the throwing of exceptions for truly exception cases (cases that should never happen).
- For operations that can fail and communicating the error reason is desirable,
  prefer returning `Result<TSuccess, TError>` over throwing exceptions.  This allows
  callers to handle errors without try/catch and preserves error information without
  loss from exception types or messages.
- For operations that can fail but communicating the error reason is not needed,
  `Option<T>` is appropriate.  This allows callers to handle the presence or absence
  of a value without try/catch and without using null references.

### Data and Operations

Model data as immutable records, and keep the problem model separate from its wire formats. [Person.cs](libs/ExampleLib/Person.cs) and [PersonDto.cs](libs/ExampleLib/PersonDto.cs) are the reference examples, and [docs/csharp_data_modeling.md](docs/csharp_data_modeling.md) has the full rules.

- Data types are `sealed record class` types. Never use `record struct`.
- Use nominal syntax. Positional syntax is only for single-value wrappers and small types whose parameters have distinct types.
- **Domain types** model the problem and are never serialized. Their factories, constants and pure operations are members of the type. A type with invariants has a private constructor, `{ get; private init; }` properties, and a static `Create` factory that returns `Result<T, TError>`.
- **DTOs** model a wire format, and all JSON goes through them. Their names end in `Dto`. They hold data only, in `required` `{ get; init; }` properties, and map to and from their domain type in a `<Type>DtoExt` class.
- Operations are pure. "Modifying" operations return a copy made with `with`.

### Asynchronous Code

- Use `ConfigureAwait(false)` on all `await` calls in library code.
- **Method naming for async operations:** The `Async` suffix is used only when a **callback/function parameter is async**, not merely because the input or output is a `Task`. For example:
  - `Exists(Task<Result>, Func<T, bool>)` — no Async suffix; Task input but sync predicate
  - `ExistsAsync(Task<Result>, Func<T, Task<bool>>)` — Async suffix; async predicate
  - `FoldAsync(Result, Func<S, T, Task<S>>, S)` — Async suffix; async folder function
  - `Fold(Task<Result>, Func<S, T, S>, S)` — no Async suffix; Task input but sync folder

### Disposable Objects

- Handing a new disposable object to something that will dispose it (a `DisposableResult`, or another object that takes ownership) triggers CA2000, which reports that the object is never disposed. Suppress it around that one statement, with a comment saying where ownership goes:

```csharp
#pragma warning disable CA2000 // Ownership transfers into DisposableResult
        return DisposableResult.Success<Session, NativeError>(new Session(handle));
#pragma warning restore CA2000
```

### Unit Tests

- Order the unit tests in a spec file to match the order of the methods under test in the implementation file.

## Build and Test

```shell
dotnet build
dotnet test
```
