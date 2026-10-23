# C# P/Invoke Guide

This guide covers calling native libraries from C#, including libraries that report results through callbacks. The general coding guidelines in [AGENTS.md](../AGENTS.md) also apply.

## 1. Declarations

- Use `LibraryImport` (source-generated), not `DllImport`, with the `__stdcall` calling convention.
- Structs passed to native code use `[StructLayout(LayoutKind.Sequential, Pack = 1)]`.

## 2. Callbacks

- Pin callback delegates as instance fields so they are not garbage collected while native code holds them.
- Use `TaskCompletionSource` to convert a native callback into a `Task<T>`.

## 3. Concurrency

- Track pending asynchronous requests and open connections in a `ConcurrentDictionary`.
- Stream data received on a connection through a `Channel<T>`.

## 4. Structure

Keep the type that owns native handles small. It is the stateful exception described in [csharp_data_modeling.md](csharp_data_modeling.md); its domain logic still belongs in records and `<Type>Ext` functions.
