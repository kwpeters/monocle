//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

using CsDepot;

namespace CsDepot.Tests;


public class DataConversionTests
{
    //
    // ToByte Tests
    //


    [Fact]
    public void ToByte_WithPositiveValue_ReturnsCorrectByte()
    {
        var result = ((sbyte)42).ToByte();
        Assert.Equal((byte)42, result);
    }


    [Fact]
    public void ToByte_WithNegativeValue_ReturnsCorrectByte()
    {
        var result = ((sbyte)-1).ToByte();
        Assert.Equal((byte)255, result);
    }


    [Fact]
    public void ToByte_WithZero_ReturnsZero()
    {
        var result = ((sbyte)0).ToByte();
        Assert.Equal((byte)0, result);
    }


    [Fact]
    public void ToByte_WithMaxValue_ReturnsCorrectByte()
    {
        var result = sbyte.MaxValue.ToByte();
        Assert.Equal((byte)127, result);
    }


    [Fact]
    public void ToByte_WithMinValue_ReturnsCorrectByte()
    {
        var result = sbyte.MinValue.ToByte();
        Assert.Equal((byte)128, result);
    }


    //
    // ToUInt16 Tests
    //


    [Fact]
    public void ToUInt16_WithPositiveValue_ReturnsCorrectUInt16()
    {
        var result = ((short)1000).ToUInt16();
        Assert.Equal((ushort)1000, result);
    }


    [Fact]
    public void ToUInt16_WithNegativeValue_ReturnsCorrectUInt16()
    {
        var result = ((short)-1).ToUInt16();
        Assert.Equal((ushort)65535, result);
    }


    [Fact]
    public void ToUInt16_WithZero_ReturnsZero()
    {
        var result = ((short)0).ToUInt16();
        Assert.Equal((ushort)0, result);
    }


    [Fact]
    public void ToUInt16_WithMaxValue_ReturnsCorrectUInt16()
    {
        var result = short.MaxValue.ToUInt16();
        Assert.Equal((ushort)32767, result);
    }


    [Fact]
    public void ToUInt16_WithMinValue_ReturnsCorrectUInt16()
    {
        var result = short.MinValue.ToUInt16();
        Assert.Equal((ushort)32768, result);
    }


    //
    // ToUInt32 Tests
    //


    [Fact]
    public void ToUInt32_WithPositiveValue_ReturnsCorrectUInt32()
    {
        var result = 100000.ToUInt32();
        Assert.Equal(100000u, result);
    }


    [Fact]
    public void ToUInt32_WithNegativeValue_ReturnsCorrectUInt32()
    {
        var result = (-1).ToUInt32();
        Assert.Equal(4294967295u, result);
    }


    [Fact]
    public void ToUInt32_WithZero_ReturnsZero()
    {
        var result = 0.ToUInt32();
        Assert.Equal(0u, result);
    }


    [Fact]
    public void ToUInt32_WithMaxValue_ReturnsCorrectUInt32()
    {
        var result = int.MaxValue.ToUInt32();
        Assert.Equal(2147483647u, result);
    }


    [Fact]
    public void ToUInt32_WithMinValue_ReturnsCorrectUInt32()
    {
        var result = int.MinValue.ToUInt32();
        Assert.Equal(2147483648u, result);
    }


    //
    // ToUInt64 Tests
    //


    [Fact]
    public void ToUInt64_WithPositiveValue_ReturnsCorrectUInt64()
    {
        var result = 9000000000L.ToUInt64();
        Assert.Equal(9000000000ul, result);
    }


    [Fact]
    public void ToUInt64_WithNegativeValue_ReturnsCorrectUInt64()
    {
        var result = (-1L).ToUInt64();
        Assert.Equal(18446744073709551615ul, result);
    }


    [Fact]
    public void ToUInt64_WithZero_ReturnsZero()
    {
        var result = 0L.ToUInt64();
        Assert.Equal(0ul, result);
    }


    [Fact]
    public void ToUInt64_WithMaxValue_ReturnsCorrectUInt64()
    {
        var result = long.MaxValue.ToUInt64();
        Assert.Equal(9223372036854775807ul, result);
    }


    [Fact]
    public void ToUInt64_WithMinValue_ReturnsCorrectUInt64()
    {
        var result = long.MinValue.ToUInt64();
        Assert.Equal(9223372036854775808ul, result);
    }
}
