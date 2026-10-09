# PipeAsync Usage Examples

## Overview

`PipeAsync` allows you to chain together asynchronous functions, making it easy to compose complex data transformations. It features:

1. **Type-safe async pipelines** - all functions return `Task<T>`
2. **Simple, consistent API** - just one overload per arity (1-10 functions)
3. **Full compile-time type checking** - no runtime casting

## Design Philosophy

All functions in a pipeline must return `Task<T>`. To use synchronous functions, wrap them:

```csharp
// Wrap synchronous functions
Func<int, Task<int>> add10 = x => Task.FromResult(x + 10);
```

This design eliminates combinatorial explosion of overloads while maintaining full type safety.

## Basic Examples

### Simple Async Pipeline

```csharp
var result = await PipeAsync(
    "42",
    async (string s) => await ParseIntAsync(s),
    async (int x) => await MultiplyAsync(x, 2),
    async (int x) => await ConvertToStringAsync(x)
);
// result: "84"
```

### Using Wrapped Sync Functions

```csharp
// Wrap synchronous functions to make them async
Func<string, Task<int>> parse = s => Task.FromResult(int.Parse(s));
Func<int, Task<int>> multiply = x => Task.FromResult(x * 2);
Func<int, Task<string>> toString = x => Task.FromResult(x.ToString());

var result = await PipeAsync(
    "21",
    parse,
    multiply,
    toString
);
// result: "42"
```

### Working with Async Initial Values

```csharp
// If initial value is a Task, just await it
Task<string> initialValue = FetchDataAsync();

Func<string, Task<int>> parse = s => Task.FromResult(int.Parse(s));
Func<int, Task<int>> multiply10 = x => Task.FromResult(x * 10);

var result = await PipeAsync(
    await initialValue,  // Await the Task first
    parse,
    multiply10
);
```

### Real-World Example

```csharp
// Fetch user data, transform, and save
var userId = "user123";

await PipeAsync(
    userId,
    FetchUserFromDbAsync,                                        // string -> Task<User>
    u => Task.FromResult(ValidateUser(u)),                       // User -> Task<User>
    EnrichWithProfileDataAsync,                                  // User -> Task<EnrichedUser>
    u => Task.FromResult(u with { Email = u.Email.ToLower() }), // EnrichedUser -> Task<EnrichedUser>
    SaveToDbAsync                                                // EnrichedUser -> Task<Unit>
);
```

## Important Notes

- **All Functions Return Task**: Every function must return `Func<T, Task<TResult>>`
- **Wrapping Sync Functions**: Use `x => Task.FromResult(syncFunc(x))` to wrap synchronous functions
- **Type Inference**: The compiler infers types automatically - no explicit type parameters needed
- **Task Initial Values**: If your initial value is a `Task<T>`, simply `await` it before passing to `PipeAsync`

## Type Safety

The compiler ensures at compile-time:

- Each function's output type matches the next function's input type
- All functions return `Task<T>`
- The final return type is `Task<TFinal>`
- No runtime type checking or casting is performed
