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
- **Fixed-width numeric values**: type a value that has a fixed bit width with the bit-width framework name (`Int16`, `UInt16`, `Int32`, `UInt32`, `Int64`, `UInt64`) rather than the language keyword, so the width is explicit in the type — for example a CIP or Logix data type's value. Use the language keywords (`int`, `string`, `bool`, `byte`, …) everywhere else, such as counts, indices, sizes, and loop variables. `byte`, `sbyte`, `float`, and `double` have no bit-width framework name, so they keep the keyword.
- **Braces are required** on `if` statements and loops. The sole exception is a body that contains only a `return` statement, which may go on the same line (`if (done) return;`) or the next line.
- **Two blank lines between members** (methods, properties, fields with attributes, test methods). Use one blank line only within a member's body.
- **Private fields**: `_camelCase` with underscore prefix.
- **Physical quantities**: include the units.  For example, `timeoutMs`.
- **Functions**: end the name of a parameter, property or field whose type is a function (`Func<...>`, `Action<...>`, or any other delegate type) with `Fn`. For example, `predicateFn`, `runFn`, `successFn`. This is a convention that tooling does not enforce. Exceptions: events, which keep the usual .NET event naming, and parameters of members that override or implement an API from outside this repo, which keep that API's names so that named arguments still match.
- **`this.` qualification**: qualify every use of an instance member (field, property, method or event) with `this.`, including private `_camelCase` fields (`this._logger`). Do not qualify static members, locals or parameters. Enforced as a warning (IDE0009), which the build treats as an error. To fix a whole project at once, run `dotnet format style --diagnostics IDE0009 --severity warn`.
- **Encoding**: every file is UTF-8 without a byte order mark (BOM). Enforced for `.cs` files by `dotnet format whitespace`, so do not let a tool or editor add a BOM.
- See [.editorconfig](.editorconfig) for all formatting and style rules.

### Lambdas

- Lambda parameters should be wrapped in parentheses when there are multiple parameters. Single-parameter lambdas may omit parentheses.
- Keep lambdas flat and LINQ queries free of side effects, because queries run lazily.
  Where a lambda would nest or a query would start work,
  write a `foreach` with a named local per step:

  ```csharp
  foreach (Order order in orders)
  {
      var processor = new OrderProcessor(order, logger);
      tasks.Add(Task.Run(() => processor.RunAsync(token)));
  }
  ```

### Ternary Formatting

Keep a ternary on a single line when it fits within 100 columns:

```csharp
return input.Count > 0 ? Success(input) : Error(errorFactoryFn());
```

When a ternary spans multiple lines, the `?` and `:` should appear at the end of the line (not the beginning of the next line). For example:

```csharp
string result = condition ?
    valueIfTrue :
    valueIfFalse;
```

### Method Chains

When a chain of calls doesn't fit on one line, put each call on its own line, starting with its `.`, and line the calls up with the start of the expression they're called on:

- If the expression starts its own line, give the calls the same indentation:

  ```csharp
  public Result<Person, string> WithLastName(string lastName) =>
      ValidateName(lastName, nameof(lastName))
      .MapSuccess((last) => this with { LastName = last });
  ```

- If the expression starts partway through a line (after `return` or `Type name =`), indent the calls one level deeper than that line:

  ```csharp
  IEnumerable<string> names = people
      .Where((person) => person.IsActive)
      .Select((person) => person.Name);
  ```

### Method Signatures

Put a method signature on a single line unless one of the following is true of the signature written on a single line, counting its modifiers (`public`, `static`, `async`, and so on) and its leading indentation:

- it extends past column 100,
- the method declares type parameters of its own, or
- the method name starts at column 30 or later.

The method name is the most important part of a signature. Moving it to the start of its own line lets a reader scan down a file and find it without hunting past modifiers and a long return type.

When any of these is true, keep the return type (including `async` and other modifiers) on one line, start the method name on the next, and put each parameter on its own line:

```csharp
public static Result<int, string>
Initialize(
    uint paramA,
    string paramB
)
```

A generic method, such as `MapSuccess<TInSuccess, TOutSuccess, TError>`, always uses this style, however short it is. A non-generic method in a generic type, such as `Result<TSuccess, TError>`, doesn't count as generic.

### Class and Record Member Ordering

Declare members in this order, omitting sections that are not needed: Constants, Static fields, Static properties, Static methods, Instance fields, Constructors, Properties, Events, Instance methods, Nested types.

Start each section with a separator comment naming it. The separators may be omitted when a class or record has only one section (as is often the case in unit test files).

```csharp
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------
```

## Patterns

### Decomposition

Decompose a method that does several things into a short sequence of named steps,
so that the caller reads without opening them.
Give a step its own function when its name tells the reader something the code does not,
when several places use it,
or when it hides detail the caller does not need.
Leave a step inline when it is a one-line wrapper used once,
or when its name would nearly repeat an existing function's.

Choose the home that fits the function:

- A local function, when only that one method uses it.
- A private method, when other members of the same class use it.
- A reusable utility function, when other classes or projects could use it.

Decomposition has a cost in some code,
such as hot paths where call overhead or lost inlining, allocations, or captured variables matter.
In those cases you may keep the operations together,
but write a comment on the method that says why.

### Functional Programming

- Always prefer use of the functional data types and patterns defined in the FnUtil library.
- Never use null references.  Use `Option<T>` from FnUtil instead.
  The one exception is events.
  A C# event is null until a handler subscribes,
  so declare it nullable and raise it with `?.Invoke`:

  ```csharp
  public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;

  this.TemperatureChanged?.Invoke(this, args);
  ```

- Reserve the throwing of exceptions for truly exception cases (cases that should never happen).
- For operations that can fail and communicating the error reason is desirable,
  prefer returning `Result<TSuccess, TError>` over throwing exceptions.  This allows
  callers to handle errors without try/catch and preserves error information without
  loss from exception types or messages.
- For operations that can fail but communicating the error reason is not needed,
  `Option<T>` is appropriate.  This allows callers to handle the presence or absence
  of a value without try/catch and without using null references.
- To unwrap a `Result` or `Option` that should never fail (as in tests), choose the name
  that tells the reader what the call returns:
  - When the value is used inside an expression (an assert, a method argument, a
    `return`, a chained call), use the accessor: `GetValue()` or `GetError()`.
    For example, `Assert.Equal("must not be blank.", result.GetError())`.
  - When the value is assigned to a typed local, or the call stands alone as an
    assertion, use `ThrowIfError()`, `ThrowIfSuccess()`, `ThrowIfNone()` or
    `ThrowIfSome()`. For example, `Person person = Person.Create(...).ThrowIfError()`.
    Use these too when you need a custom exception message.

### Data and Operations

Model data as immutable records, and keep the problem model separate from its wire formats. [Person.cs](libs/ExampleLib/Person.cs) and [PersonDto.cs](libs/ExampleLib/PersonDto.cs) are the reference examples, and [docs/csharp_data_modeling.md](docs/csharp_data_modeling.md) has the full rules.

- Data types are `sealed record class` types. Never use `record struct`.
- Use nominal syntax. Positional syntax is only for single-value wrappers and small types whose parameters have distinct types.
- **Domain types** model the problem and are never serialized. Their factories, constants and pure operations are members of the type. A type with invariants has a private constructor, `{ get; private init; }` properties, and a static `Create` factory that returns `Result<T, TError>`. A type without invariants may be publicly constructible, or may use the same private-constructor and static-factory pattern.
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

## Referencing Code

Write every reference to a file in this repo as a Markdown link, so the reader can jump to it.
Make the path relative to the repository root,
and add a line anchor for a line or a range:
[Person.cs](libs/ExampleLib/Person.cs),
[Person.cs:10](libs/ExampleLib/Person.cs#L10),
[Person.cs:10-20](libs/ExampleLib/Person.cs#L10-L20).

## Markdown Guidelines

### Markdown: semantic line breaks

Write Markdown prose with [semantic line breaks](https://sembr.org).
Line breaks follow the structure of the sentence,
not a column width.

- Start every sentence on a new line.
- Break a long sentence at its clause boundaries:
  after a comma, semicolon, colon, or dash that ends a clause,
  and before a conjunction or relative clause that starts a new idea.
- Keep each line a complete unit of thought;
  short clauses can share a line with the clause they belong to.
- Keep inline code spans, links, and emphasized phrases whole on one line.
- In list items, indent continuation lines to align with the item's text.
- When you edit a paragraph, reflow the whole paragraph to semantic line breaks.

These breaks leave the rendered output unchanged,
because Markdown renders a single newline inside a paragraph as a space.
They keep diffs small, since changing one clause changes one line.

Example:

```markdown
The cache is shared by every worker process,
so a stale entry written by one worker is visible to all of them.
If the entry has expired,
the next read misses the cache
and the value is loaded from the database.
```

### Markdown: aligned tables

Use a table only for short, enumerable data such as names, types, and flags.
If any row would exceed about 120 characters
(prose sentences, long URLs),
use a list or one heading per row instead.

Pad table cells with spaces so the columns line up when the file is viewed as plain text.
Every `|` in a column sits at the same character position on every row,
including the header and the delimiter row.

- When you add, remove, or edit a cell, re-pad the whole table to the new widest cell in each column.
- A table row stays on one line.
  Semantic line breaks apply to prose, not to table rows.

Example:

```markdown
| Name   | Role                  |
| ------ | --------------------- |
| Parser | Reads the input file  |
| Cache  | Stores recent results |
```

### Markdown: images

Put the images that a Markdown document embeds
in an `images` directory inside a directory named after the document, without its extension.
The images for `docs/design.md` go in `docs/design/images/`,
and the document refers to them by a relative path:

```markdown
![The architecture](design/images/architecture.drawio.png)
```
