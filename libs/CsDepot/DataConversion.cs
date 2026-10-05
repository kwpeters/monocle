//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

namespace CsDepot;


/// <summary>
/// Extension methods for data type conversions.
/// </summary>
public static class DataConversion
{
    /// <summary>
    /// Converts a signed 8-bit integer to its unsigned byte representation.
    /// </summary>
    /// <param name="v">The signed byte value to convert</param>
    /// <returns>The unsigned byte representation of the signed value</returns>
    public static byte ToByte(this sbyte v)
    {
        byte b;
        unchecked
        {
            b = (byte)v;
        }
        return b;
    }


    /// <summary>
    /// Converts a signed 16-bit integer to its unsigned representation.
    /// </summary>
    /// <param name="v">The signed 16-bit value to convert</param>
    /// <returns>The unsigned 16-bit representation of the signed value</returns>
    public static ushort ToUInt16(this short v)
    {
        ushort u;
        unchecked
        {
            u = (ushort)v;
        }
        return u;
    }


    /// <summary>
    /// Converts a signed 32-bit integer to its unsigned representation.
    /// </summary>
    /// <param name="v">The signed 32-bit value to convert</param>
    /// <returns>The unsigned 32-bit representation of the signed value</returns>
    public static uint ToUInt32(this int v)
    {
        uint u;
        unchecked
        {
            u = (uint)v;
        }
        return u;
    }


    /// <summary>
    /// Converts a signed 64-bit integer to its unsigned representation.
    /// </summary>
    /// <param name="v">The signed 64-bit value to convert</param>
    /// <returns>The unsigned 64-bit representation of the signed value</returns>
    public static ulong ToUInt64(this long v)
    {
        ulong u;
        unchecked
        {
            u = (ulong)v;
        }
        return u;
    }
}
