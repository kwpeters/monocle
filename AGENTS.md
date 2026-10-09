# Coding Guidelines

## Code Style

- **.NET 10**, nullable reference types enabled, implicit usings enabled.
- **File-scoped namespaces** — enforced as error in `.editorconfig`.
- **Explicit integer type sizes** (`Int32`, `UInt16`, `Byte`) in CIP/protocol code where bit widths matter. Standard `int`/`byte` elsewhere is fine.
- **Braces always required** — no single-line `if` without braces.
- **Private fields**: `_camelCase` with underscore prefix.
- **P/Invoke function names**: `SCREAMING_SNAKE_CASE` matching the native API (e.g., `DTL_INIT_RSI_EX`).
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
public static DisposableResult<DtlRuntime, DtlError>
Initialize(
    uint maxDefines,
    DtlInitFlags flags
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

### Error Handling & Functional Types (FnUtil)

- Use `Result<TSuccess, TError>` for all operations that can fail — return error values instead of throwing exceptions.
- `Option<T>` — abstract record with internal `Some<T>(T Value)` and `None<T>` subtypes. Use `F.Some()` and `F.none` factories.
- `Result<TSuccess, TError>` — discriminated union with `SuccessResult`/`ErrorResult` subtypes.
- Extension methods: `Match()`, `GetOrElse()`, pipe operators, async helpers.
- Use `ConfigureAwait(false)` on all `await` calls in library code (FnUtil, CipDotNet).

### P/Invoke

- All native interop lives under `FtLinxDotNet/Native/`.
- Use `LibraryImport` (source-generated) with `__stdcall` calling convention.
- Pin callback delegates as instance fields on `DtlRuntime` to prevent GC collection.
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

- **x86 platform constraint**: `dtl_linxe.dll` is Win32 only. The FtLinxDotNet and CipClient projects target x86.
- **Integration tests** are tagged with `[Trait("Category", "Integration")]` and require the native DLL in PATH.
- **Unit tests** use NSubstitute to mock `IDtlRuntime` — no native DLL needed.

## Conventions

- Interfaces for testability: `IDtlRuntime`, `ICipConnection`.
- Sealed classes with private constructors and static factory methods (e.g., `DtlRuntime.Initialize()`).
- `#pragma warning disable CA2000` / `restore` around intentional ownership transfers in factory methods.
- Namespaces: `FtLinxDotNet`, `FtLinxDotNet.Native`, `FtLinxDotNet.Tests`, `CipDotNet`, `CipDotNet.Tests`, `FnUtil`, `FnUtil.Tests`.
