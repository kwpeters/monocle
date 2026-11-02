# Reading binary data with BufReader

Binary protocol messages are sequences of fields packed into bytes. To read
one, you read the first field, then the next, and so on. Often a field's value
tells you how to read a later one, such as a length that says how many bytes
follow. Some structures also contain pad bytes, which push the next field
onto a 2-, 4- or 8-byte boundary.

`BufReader` ([BufReader.cs](../BufReader.cs)) handles all of this. This
document explains how to use it, and is written for readers who don't know C#.

## 1. The C# you need

| C# | Meaning | TypeScript equivalent |
| --- | --- | --- |
| `Result<T, string>` | Either a success holding a `T`, or an error holding a message | `Result<T, string>` |
| `Success(x)`, `Error("...")` | Create a success or an error | `new SucceededResult(x)`, `new FailedResult("...")` |
| `r => ...` | A function (lambda) with parameter `r` | `(r) => ...` |
| `result.Bind(x => ...)` | If `result` succeeded, run the function on its value, which returns another `Result`. If it failed, return its error. | `result.bind((x) => ...)` |
| `result.MapSuccess(x => ...)` | If `result` succeeded, transform its value. If it failed, return its error. | `result.mapSuccess((x) => ...)` |
| `result.MapError(e => ...)` | If `result` failed, transform its error message | `result.mapError((e) => ...)` |
| `static` method | A method called on the type itself, as in `Sample.Read(reader)` | A function exported from a module |

## 2. The reader

A `BufReader` holds a buffer of bytes and an offset into it, which starts at
0. Each successful read returns the value and moves the offset past it. A
failed read returns an error and leaves the offset where it was.

| Member | What it does |
| --- | --- |
| `new BufReader(bytes)` | Creates a reader at offset 0 |
| `OffsetBytes` | The offset of the next byte to read |
| `NumRemainingBytes` | How many bytes are left |
| `ReadBytes(n)` | Reads exactly `n` bytes |
| `Align(n)` | Reads the pad bytes needed to reach the next multiple of `n` (section 5) |
| `Attempt(r => ...)` | Reads several values as one unit (section 4) |

A read that runs out of bytes fails with an error such as "Cannot read 2
byte(s) at offset 3. Only 1 are available." Offsets in error messages always
count from the start of the buffer.

## 3. Reading one value

A type that is read from bytes usually has a static `Read` method built on
`ReadBytes`. For example, a type wrapping a signed 8-bit integer might read
itself like this:

```csharp
public static Result<SampleInt8, string> Read(BufReader reader) =>
    reader.ReadBytes(1)
        .MapSuccess(bytes => new SampleInt8(unchecked((sbyte)bytes.Span[0])))
        .MapError(error => $"Cannot read SampleInt8. {error}");
```

`Read` reads one byte and turns it into a `SampleInt8`.
(`unchecked((sbyte)...)` reinterprets the byte as a signed number, so `0xFF`
becomes -1.) If there's no byte to read, `MapError` adds the type's name to
the front of the error, so the message says what was being read.

Reading values one after another is just a series of calls. The examples
below use a `ReadByte` function that reads one unsigned byte:

```csharp
static Result<byte, string> ReadByte(BufReader reader) =>
    reader.ReadBytes(1).MapSuccess(bytes => bytes.Span[0]);

BufReader reader = new(new byte[] { 0x01, 0xFE });
byte first = ReadByte(reader).ThrowIfError();    // 0x01, offset is now 1
byte second = ReadByte(reader).ThrowIfError();   // 0xFE, offset is now 2
```

`ThrowIfError()` unwraps a `Result` that should never fail, as in a test.
Real code returns the error instead, as the next section shows.

## 4. Reading a structure

A structure is several fields read in order. If any field fails, the whole
structure should fail, and the reader should be left where the structure
started. Otherwise the caller can't tell how far the failed read got.

`Attempt` does this. It passes your function a new reader `r` that starts at
the current offset. If your function succeeds, the original reader moves past
everything `r` read. If it fails, the original reader doesn't move.

Inside `Attempt`, C#'s query syntax reads one field per line. This reply is a
status byte, a byte count, and then that many payload bytes:

```csharp
static Result<SampleReply, string> ReadReply(BufReader reader) =>
    reader.Attempt(r =>
        from status in ReadByte(r).Bind(RequireZeroStatus)
        from length in ReadByte(r)
        from payload in r.ReadBytes(length)
        select new SampleReply { Status = status, Payload = payload });
```

Read it top to bottom:

- `from status in ReadByte(r)` reads a byte. If the read succeeded, its value
  is named `status`. If it failed, the whole query stops and returns that
  error.
- `.Bind(RequireZeroStatus)` checks the status as it is read. If the check
  returns an error, the query stops there too.
- `from length in ...` and `from payload in ...` work the same way. A later
  line can use an earlier name, so `length` sets how many payload bytes to
  read.
- `select ...` builds the result from the named values. It runs only if every
  read succeeded.

### What the compiler does with it

Query syntax is shorthand. The compiler rewrites the query above into nested
`Bind` calls, roughly:

```csharp
ReadByte(r).Bind(RequireZeroStatus).Bind(status =>
ReadByte(r).Bind(length =>
r.ReadBytes(length).MapSuccess(payload =>
    new SampleReply { Status = status, Payload = payload })))
```

The calls are nested so that the last step can still see `status`. Each
`from` adds one more name, without you having to write the nesting.

## 5. Padding and alignment

`Align(n)` reads the pad bytes needed to reach the next multiple of `n`. If
the offset is already on a boundary, it reads nothing. It returns the pad
bytes, so callers can check them if needed.

Protocols usually align a field relative to the start of its structure, not
the start of the buffer. Inside `Attempt`, `Align` counts from where the
attempt started. So a structure that reads itself with `Attempt` pads
relative to its own start, automatically.

This structure is a 1-byte tag, a pad if needed to reach a 2-byte boundary,
and a 2-byte value. It follows a 1-byte header:

```csharp
static Result<SampleTaggedValue, string> ReadTaggedValue(BufReader reader) =>
    reader.Attempt(r =>
        from tag in ReadByte(r)
        from pad in r.Align(2)
        from value in r.ReadBytes(2)
        select new SampleTaggedValue { Tag = tag, Value = value });

BufReader reader = new(new byte[] { 0x09, 0x05, 0x00, 0x34, 0x12 });
byte header = ReadByte(reader).ThrowIfError();
SampleTaggedValue body = ReadTaggedValue(reader).ThrowIfError();
```

Here is what happens, step by step:

| Step | Reads | Offset after | Bytes into the structure |
| --- | --- | --- | --- |
| `ReadByte(reader)` | `09`, the header | 1 | (before the structure) |
| `Attempt` starts | nothing | 1 | 0 |
| `ReadByte(r)` | `05`, the tag | 2 | 1 |
| `r.Align(2)` | `00`, one pad byte, because 1 is odd | 3 | 2 |
| `r.ReadBytes(2)` | `34 12`, the value | 5 | 4 |
| `Attempt` succeeds | nothing | 5 | |

Counted from the start of the buffer, offset 2 is already on a 2-byte
boundary. Aligning from there would skip no pad and read `00 34` as the
value, which is wrong.

## 6. Mistakes to avoid

- **Reading from the outer reader inside `Attempt`.** The function can still
  see `reader`, but it must read only from `r`. Reads from `reader` aren't
  undone if the attempt fails, and are overwritten if it succeeds.
- **Using `Attempt` only to undo reads, then aligning.** `Attempt` always
  resets where `Align` counts from, even if you used it only to undo reads on
  failure. Align inside an attempt only when you mean "relative to here."
- **Reusing a name.** Each `from` needs its own name, so two pads must be
  `pad1` and `pad2`. Reusing a name gives error CS1930, "The range variable
  'pad' has already been declared."
- **Using `where`.** `Result` doesn't support `where` clauses (error CS1936).
  Check a value with `.Bind(...)`, as `RequireZeroStatus` does. A `let` clause,
  which names a computed value, does work.

## 7. Summary

- A `BufReader` keeps track of the offset. Each successful read moves it
  forward, and a failed read leaves it alone.
- A type that is read from bytes has a static `Read(BufReader)` method.
- To read a structure, use `Attempt` with one `from` line per field. A failure
  anywhere fails the structure and leaves the reader where it started.
- Inside `Attempt`, `Align` counts from the start of the attempt, so pads are
  relative to the start of the structure.
