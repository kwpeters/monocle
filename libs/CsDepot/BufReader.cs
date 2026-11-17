//
// At least a portion of the code below was created using AI tool Claude Code.
//

using FnUtil;
using static FnUtil.F;

namespace CsDepot;


/// <summary>
/// Reads values from a buffer of bytes, keeping track of the current offset.
/// Each successful read advances the offset past the bytes it read.  A failed
/// read leaves the offset unchanged.
/// </summary>
/// <remarks>
/// <para>
/// A reader is stateful, not a data type: reading changes it.  A type that is
/// read from bytes usually has a static <c>Read(BufReader)</c> method that
/// returns a <c>Result</c>.
/// </para>
/// <para>
/// To read several values as one unit, so that a failure part way through
/// leaves the offset where it started, use <see cref="Attempt"/>.  Inside it,
/// LINQ query syntax reads one field per <c>from</c> clause.  Each clause
/// names the value it read, or stops the query with the read's error:
/// <code>
/// Result&lt;Sample, string&gt; sample = reader.Attempt(r =>
///     from tag in r.ReadBytes(1)
///     from pad in r.Align(2)
///     from value in r.ReadBytes(2)
///     select new Sample { Tag = tag, Value = value });
/// </code>
/// </para>
/// <para>
/// See libs/CsDepot/docs/buf_reader.md for a detailed explanation.
/// </para>
/// </remarks>
public sealed class BufReader
{
    //------------------------------------------------------------------------------
    // Instance fields
    //------------------------------------------------------------------------------

    private readonly ReadOnlyMemory<byte> _buffer;
    private readonly int _originOffsetBytes;

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    /// <summary>
    /// Creates a reader at the start of the specified buffer.
    /// </summary>
    /// <param name="buffer">The bytes to read.</param>
    public BufReader(ReadOnlyMemory<byte> buffer) : this(buffer, 0)
    {
    }


    private BufReader(ReadOnlyMemory<byte> buffer, int offsetBytes)
    {
        this._buffer = buffer;
        this._originOffsetBytes = offsetBytes;
        this.OffsetBytes = offsetBytes;
    }

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    /// <summary>
    /// The offset of the next byte to read, from the start of the buffer.
    /// </summary>
    public int OffsetBytes { get; private set; }


    /// <summary>
    /// The number of bytes not yet read.
    /// </summary>
    public int NumRemainingBytes => this._buffer.Length - this.OffsetBytes;

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Reads exactly <paramref name="numBytes"/> raw bytes.
    /// </summary>
    /// <param name="numBytes">The number of bytes to read.</param>
    /// <returns>The bytes read, or an error if too few bytes remain.</returns>
    public Result<ReadOnlyMemory<byte>, string>
    ReadBytes(
        int numBytes
    )
    {
        if (numBytes < 0)
        {
            return Error($"Cannot read {numBytes} bytes. The number of bytes must not be negative.");
        }

        if (this.NumRemainingBytes < numBytes)
        {
            return Error(
                $"Cannot read {numBytes} byte(s) at offset {this.OffsetBytes}. " +
                $"Only {this.NumRemainingBytes} are available.");
        }

        ReadOnlyMemory<byte> bytes = this._buffer.Slice(this.OffsetBytes, numBytes);
        this.OffsetBytes += numBytes;
        return Success(bytes);
    }


    /// <summary>
    /// Reads the pad bytes needed to reach the next multiple of
    /// <paramref name="boundaryBytes"/>.  If the offset is already on a
    /// boundary, it reads nothing.
    /// </summary>
    /// <remarks>
    /// Boundaries are counted from where this reader started.  For a reader
    /// passed to an <see cref="Attempt"/> function, that is where the attempt
    /// started, so a structure that reads itself inside an attempt pads
    /// relative to its own start.
    /// </remarks>
    /// <param name="boundaryBytes">The boundary, such as 2, 4 or 8.</param>
    /// <returns>
    /// The pad bytes, so callers can check them if needed, or an error if too
    /// few bytes remain.
    /// </returns>
    public Result<ReadOnlyMemory<byte>, string>
    Align(
        int boundaryBytes
    )
    {
        if (boundaryBytes <= 0)
        {
            return Error(
                $"Cannot align to a {boundaryBytes}-byte boundary. " +
                "The boundary must be positive.");
        }

        int distanceBytes = this.OffsetBytes - this._originOffsetBytes;
        int numPadBytes = (boundaryBytes - (distanceBytes % boundaryBytes)) % boundaryBytes;

        return this.ReadBytes(numPadBytes)
            .MapError(error => $"Cannot align to a {boundaryBytes}-byte boundary. {error}");
    }


    /// <summary>
    /// Reads several values as one unit.  <paramref name="readFn"/> receives a
    /// new reader that starts at this reader's offset.  If it succeeds, this
    /// reader advances past everything it read.  If it fails, this reader is
    /// left unchanged.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <paramref name="readFn"/> must read only from the reader it is given.
    /// Reads from this reader inside <paramref name="readFn"/> are not undone on
    /// failure, and are overwritten on success.
    /// </para>
    /// <para>
    /// Offsets, including those in error messages, are still counted from the
    /// start of the buffer.  Only <see cref="Align"/> counts from the start of
    /// the attempt.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the value read.</typeparam>
    /// <param name="readFn">Reads the values and returns the result.</param>
    /// <returns>The result returned by <paramref name="readFn"/>.</returns>
    public Result<T, string>
    Attempt<T>(
        Func<BufReader, Result<T, string>> readFn
    )
    {
        BufReader attemptReader = new(this._buffer, this.OffsetBytes);
        Result<T, string> result = readFn(attemptReader);
        if (result.IsSuccess)
        {
            this.OffsetBytes = attemptReader.OffsetBytes;
        }

        return result;
    }
}
