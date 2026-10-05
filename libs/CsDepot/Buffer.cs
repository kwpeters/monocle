//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//
using System.Globalization;

namespace CsDepot;

/// <summary>
/// Represents a read-only buffer of bytes.
/// </summary>
public class Buffer
{
    private readonly byte[] _bytes;


    /// <summary>
    /// Private constructor to enforce creation through factory methods.
    /// </summary>
    /// <param name="bytes">The byte array to wrap</param>
    private Buffer(byte[] bytes)
    {
        this._bytes = bytes;
    }


    /// <summary>
    /// Creates a new Buffer from a ByteString by parsing its hexadecimal bytes.
    /// </summary>
    /// <param name="byteString">The ByteString containing hexadecimal byte values</param>
    /// <returns>A new Buffer instance containing the parsed bytes</returns>
    public static Buffer FromByteString(ByteString byteString)
    {
        // Split the hex string into tokens and parse each one
        var tokens = byteString.Value.Split(
            Array.Empty<char>(),
            StringSplitOptions.RemoveEmptyEntries
        );

        var bytes = new byte[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            bytes[i] = byte.Parse(tokens[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return new Buffer(bytes);
    }


    /// <summary>
    /// Gets the number of bytes in the buffer.
    /// </summary>
    public int ByteLength => this._bytes.Length;


    /// <summary>
    /// Validates that a read operation at the specified offset with the specified size is within buffer bounds.
    /// </summary>
    /// <param name="offset">The zero-based byte offset to read from</param>
    /// <param name="byteCount">The number of bytes to read</param>
    /// <returns>
    /// A Result with Unit on success, or an error message if offset is negative or the read would extend
    /// beyond buffer bounds
    /// </returns>
    private Result<Unit, string> ValidateOffset(int offset, int byteCount)
    {
        if (offset < 0)
        {
            return Error($"Offset {offset} cannot be negative.");
        }

        if (offset + byteCount > this._bytes.Length)
        {
            return Error(
                $"Offset {offset} with read size {byteCount} exceeds buffer length {this._bytes.Length}."
            );
        }

        return Success(unit);
    }


    /// <summary>
    /// Reads a signed 8-bit integer (SByte) from the buffer at the specified offset.
    /// </summary>
    /// <param name="offset">The zero-based byte offset to read from</param>
    /// <returns>
    /// A Result containing the signed 8-bit integer value on success, or an error message if the offset is invalid
    /// </returns>
    public Result<sbyte, string> ReadInt8(int offset)
    {
        return this.ValidateOffset(offset, 1)
            .MapSuccess(_ => unchecked((sbyte)this._bytes[offset]));
    }


    /// <summary>
    /// Reads an unsigned 8-bit integer (Byte) from the buffer at the specified offset.
    /// </summary>
    /// <param name="offset">The zero-based byte offset to read from</param>
    /// <returns>
    /// A Result containing the unsigned 8-bit integer value on success, or an error message if the offset is invalid
    /// </returns>
    public Result<byte, string> ReadUInt8(int offset)
    {
        return this.ValidateOffset(offset, 1)
            .MapSuccess(_ => this._bytes[offset]);
    }
}
