//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//
using CsDepot;

namespace CsDepot.Tests;

public class BufferTests
{
    [Fact]
    public void FromByteString_WithEmptyByteString_CreatesEmptyBuffer()
    {
        // Arrange
        var byteString = ByteString.Create("").AssertSuccessful();

        // Act
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Assert
        Assert.Equal(0, buffer.ByteLength);
    }


    [Fact]
    public void ByteLength_WithEmptyBuffer_ReturnsZero()
    {
        // Arrange
        var byteString = ByteString.Create("").AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var length = buffer.ByteLength;

        // Assert
        Assert.Equal(0, length);
    }


    [Theory]
    [InlineData("AB", 1)]
    [InlineData("AB CD", 2)]
    [InlineData("AB CD EF", 3)]
    [InlineData("01 02 03 04 05", 5)]
    [InlineData("FF EE DD CC BB AA 99 88 77 66", 10)]
    public void ByteLength_WithVariousBuffers_ReturnsCorrectLength(string hexString, int expectedLength)
    {
        // Arrange
        var byteString = ByteString.Create(hexString).AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var length = buffer.ByteLength;

        // Assert
        Assert.Equal(expectedLength, length);
    }


    [Fact]
    public void ReadUInt8_WithValidOffset_ReturnsCorrectValue()
    {
        // Arrange
        var byteString = ByteString.Create("AB CD EF").AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act & Assert
        Assert.Equal(0xAB, buffer.ReadUInt8(0).AssertSuccessful());
        Assert.Equal(0xCD, buffer.ReadUInt8(1).AssertSuccessful());
        Assert.Equal(0xEF, buffer.ReadUInt8(2).AssertSuccessful());
    }


    [Theory]
    [InlineData("00", 0, 0x00)]
    [InlineData("FF", 0, 0xFF)]
    [InlineData("7F", 0, 0x7F)]
    [InlineData("80", 0, 0x80)]
    [InlineData("AB CD EF", 1, 0xCD)]
    public void ReadUInt8_WithVariousValues_ReturnsCorrectUnsignedByte(
        string hexString,
        int offset,
        byte expected
    )
    {
        // Arrange
        var byteString = ByteString.Create(hexString).AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var result = buffer.ReadUInt8(offset).AssertSuccessful();

        // Assert
        Assert.Equal(expected, result);
    }


    [Fact]
    public void ReadUInt8_WithNegativeOffset_ReturnsError()
    {
        // Arrange
        var byteString = ByteString.Create("AB CD EF").AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var result = buffer.ReadUInt8(-1);

        // Assert
        Assert.True(result.IsError);
        var err = result.AssertError();
        Assert.Contains("cannot be negative", err);
    }


    [Fact]
    public void ReadUInt8_WithOffsetBeyondBounds_ReturnsError()
    {
        // Arrange
        var byteString = ByteString.Create("AB CD").AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var result1 = buffer.ReadUInt8(2);
        var result2 = buffer.ReadUInt8(100);

        // Assert
        Assert.True(result1.IsError);
        var err1 = result1.AssertError();
        Assert.Contains("exceeds buffer length", err1);

        Assert.True(result2.IsError);
        var err2 = result2.AssertError();
        Assert.Contains("exceeds buffer length", err2);
    }


    [Fact]
    public void ReadInt8_WithValidOffset_ReturnsCorrectValue()
    {
        // Arrange
        var byteString = ByteString.Create("7F 80 FF").AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act & Assert
        Assert.Equal(127, buffer.ReadInt8(0).AssertSuccessful());   // 0x7F = 127
        Assert.Equal(-128, buffer.ReadInt8(1).AssertSuccessful());   // 0x80 = -128
        Assert.Equal(-1, buffer.ReadInt8(2).AssertSuccessful());     // 0xFF = -1
    }


    [Theory]
    [InlineData("00", 0, 0)]
    [InlineData("7F", 0, 127)]      // Maximum positive sbyte
    [InlineData("80", 0, -128)]     // Minimum negative sbyte
    [InlineData("FF", 0, -1)]       // -1 in two's complement
    [InlineData("01", 0, 1)]
    [InlineData("FE", 0, -2)]
    [InlineData("AB CD EF", 1, -51)] // 0xCD as signed byte
    public void ReadInt8_WithVariousValues_ReturnsCorrectSignedByte(
        string hexString,
        int offset,
        sbyte expected
    )
    {
        // Arrange
        var byteString = ByteString.Create(hexString).AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var result = buffer.ReadInt8(offset).AssertSuccessful();

        // Assert
        Assert.Equal(expected, result);
    }


    [Fact]
    public void ReadInt8_WithNegativeOffset_ReturnsError()
    {
        // Arrange
        var byteString = ByteString.Create("AB CD EF").AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var result = buffer.ReadInt8(-1);

        // Assert
        Assert.True(result.IsError);
        var err = result.AssertError();
        Assert.Contains("cannot be negative", err);
    }


    [Fact]
    public void ReadInt8_WithOffsetBeyondBounds_ReturnsError()
    {
        // Arrange
        var byteString = ByteString.Create("AB CD").AssertSuccessful();
        var buffer = CsDepot.Buffer.FromByteString(byteString);

        // Act
        var result1 = buffer.ReadInt8(2);
        var result2 = buffer.ReadInt8(100);

        // Assert
        Assert.True(result1.IsError);
        var err1 = result1.AssertError();
        Assert.Contains("exceeds buffer length", err1);

        Assert.True(result2.IsError);
        var err2 = result2.AssertError();
        Assert.Contains("exceeds buffer length", err2);
    }
}
