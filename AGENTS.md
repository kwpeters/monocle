# Coding Guidelines

## Code Style

- **.NET 10**, nullable reference types enabled, implicit usings enabled.
- **File-scoped namespaces** — enforced as error in `.editorconfig`.
- **Explicit integer type sizes** (`Int32`, `UInt16`, `Byte`) in CIP/protocol code where bit widths matter. Standard `int`/`byte` elsewhere is fine.
- **Braces always required** — no single-line `if` without braces.
- **Private fields**: `_camelCase` with underscore prefix.
- **Physical quantities**: include the units.  For example, `timeoutMs`.
- See [.editorconfig](../.editorconfig) for all formatting and style rules.

### If Statements and Loops

- If statements and loops must use braces. The sole exception is when the body contains only a `return` statement.

### Lambdas

- The parameters of a lambda should always be wrapped in parenthesis.

### Ternary Formatting

When a ternary spans multiple lines, the `?` and `:` should appear at the end of the line, not the beginning of the next line. For example:

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

### Asynchronous Code

- Use `ConfigureAwait(false)` on all `await` calls in library code.

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
