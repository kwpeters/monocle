# C# P/Invoke Guide

This guide covers calling native libraries from C#, including libraries that report results through callbacks. The general coding guidelines in [AGENTS.md](../AGENTS.md) also apply.

## 1. Project settings

- Set `PlatformTarget` to match the native library's architecture in every project that loads it: the wrapping library, its test project, and every app that references the wrapper. A 32-bit-only library needs `<PlatformTarget>x86</PlatformTarget>`. A mismatch compiles cleanly and fails at runtime with `BadImageFormatException`.

## 2. Declarations

- Put all native declarations (P/Invoke methods, callback delegates, structs and constants) in a `Native` folder and namespace inside the library that wraps them, and make them `internal`. Code outside that library uses the managed wrapper, never the native declarations.
- Declare the P/Invoke methods in a class named for the native library, such as `Sqlite3NativeMethods`. Suppress CA1060 on that class (`#pragma warning disable CA1060 // P/Invoke class is already in the Native namespace`), because the `Native` namespace already separates the declarations.
- Use the calling convention the native header declares (`__stdcall`, `__cdecl`, or a macro such as `WINAPI` that expands to one), and name its source in the class's doc comment. Set it on every declaration (`CallingConvention = CallingConvention.StdCall` on `DllImport`), and on callback delegates with `[UnmanagedFunctionPointer(CallingConvention.StdCall)]`. x64 has only one calling convention, so a mismatch has no effect there. On x86 it corrupts the stack, so it can go unnoticed until a 32-bit build.
- Structs passed to native code use `[StructLayout(LayoutKind.Sequential)]`, with `Pack` matching the header's packing: `Pack = 1` for `#pragma pack(push, 1)`, and no `Pack` when the header uses default packing. Name the header's packing in the struct's doc comment.
- Name each P/Invoke method and its parameters exactly as the native function and its parameters, even when that breaks C# naming conventions (for example, `sqlite3_open` or `CreateFileW`). The C# declaration can then be found by searching for the name used in the native header and vendor documentation.
- Choose each parameter and field type by the size of the native type, not by its name. C `long` and `unsigned long` are 32 bits on Windows, so they map to `Int32` and `UInt32`, not to C# `long`. Map pointer-sized types (`size_t`, `intptr_t`, handles) to `nuint` or `nint`.
- Write integer types with their sized names (`Int32`, `UInt16`, `Byte`) so the width is visible in the declaration.

## 3. Callbacks

- Pin callback delegates as instance fields so they are not garbage collected while native code holds them.
- Use `TaskCompletionSource` to convert a native callback into a `Task<T>`.

## 4. Concurrency

- Track pending asynchronous requests and open connections in a `ConcurrentDictionary`.
- Stream data received on a connection through a `Channel<T>`.

## 5. Structure

Keep the type that owns native handles small. It is the stateful exception described in [csharp_data_modeling.md](csharp_data_modeling.md); its domain logic still belongs in records and `<Type>Ext` functions.

- A type that owns a native resource is a `sealed` class with a private constructor. Create it through a static factory method that calls the native initialization function and returns FnUtil's `DisposableResult<T, TError>`. A failed initialization then never produces an instance, and the caller disposes a successful one with `using`.

## 6. Testing

- Expose the managed wrapper, and every object it hands out (connections, sessions), through interfaces. Methods return the interface, not the concrete type, and the concrete types can be `internal`.
- Code that uses the wrapper depends on the interfaces. Its unit tests substitute mocks (using NSubstitute), so they run without the native library.
- Tests that call the real native library are integration tests. Tag them `[Trait("Category", "Integration")]` so that `dotnet test --filter "Category!=Integration"` can skip them on machines without the native library and whatever it needs at runtime (services, drivers, hardware). The native library must be on the DLL search path, either on `PATH` or copied to the test output directory.
