//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Buffers;

namespace CsDepot;


/// <summary>
/// Writes values to a growable buffer of bytes, keeping track of how many bytes
/// have been written.  It is the write-side counterpart to <see cref="BufReader"/>.
/// </summary>
/// <remarks>
/// <para>
/// A writer is stateful: each write appends to the buffer and advances the
/// offset.  A type that is written to bytes has an <c>Encode()</c> method that
/// returns its bytes; pass that to <see cref="WriteBytes"/>.
/// </para>
/// <para>
/// Unlike <see cref="BufReader"/>, whose reads can run out of bytes and so return
/// a <c>Result</c>, writes to a growable buffer cannot fail, so the write methods
/// return nothing.
/// </para>
/// <para>
/// <see cref="AlignTo"/> counts boundaries from the start of the buffer.
/// </para>
/// </remarks>
public sealed class BufWriter
{
    //------------------------------------------------------------------------------
    // Instance fields
    //------------------------------------------------------------------------------

    private readonly ArrayBufferWriter<byte> _buffer = new();

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    /// <summary>
    /// The number of bytes written so far.
    /// </summary>
    public int NumWrittenBytes => _buffer.WrittenCount;


    /// <summary>
    /// The bytes written so far.
    /// </summary>
    public ReadOnlyMemory<byte> WrittenMemory => _buffer.WrittenMemory;

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Appends the specified bytes, advancing past them.
    /// </summary>
    /// <param name="bytes">The bytes to append, such as a value's Encode() output.</param>
    public void WriteBytes(ReadOnlyMemory<byte> bytes) => _buffer.Write(bytes.Span);


    /// <summary>
    /// Writes the zero pad bytes needed to reach the next multiple of
    /// <paramref name="boundaryBytes"/>, counted from the start of the buffer.  If
    /// the offset is already on a boundary, it writes nothing.
    /// </summary>
    /// <param name="boundaryBytes">The boundary, such as 2, 4 or 8.</param>
    public void AlignTo(int boundaryBytes)
    {
        if (boundaryBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(boundaryBytes),
                boundaryBytes,
                "The boundary must be positive.");
        }

        int numPadBytes = (boundaryBytes - (NumWrittenBytes % boundaryBytes)) % boundaryBytes;
        _buffer.GetSpan(numPadBytes)[..numPadBytes].Clear();
        _buffer.Advance(numPadBytes);
    }
}
