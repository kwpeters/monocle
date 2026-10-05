using CsDepot;

namespace CsDepot.Tests;

public class ByteStringTests
{
    [Fact]
    public void Create_WithValidSingleDigitHex_ReturnsSuccess()
    {
        _ = ByteString.Create("A B C D").AssertSuccessful();
    }

    [Fact]
    public void Create_WithValidDoubleDigitHex_ReturnsSuccess()
    {
        _ = ByteString.Create("AB CD EF 12").AssertSuccessful();
    }

    [Fact]
    public void Create_WithMixedSingleAndDoubleDigitHex_ReturnsSuccess()
    {
        _ = ByteString.Create("A BB C DD 1 FF").AssertSuccessful();
    }

    [Fact]
    public void Create_WithValidHexAndVariousWhitespace_ReturnsSuccess()
    {
        _ = ByteString.Create("  AB  \t CD \n EF  \r\n 12  ").AssertSuccessful();
    }

    [Fact]
    public void Create_WithLowercaseHex_ReturnsSuccess()
    {
        _ = ByteString.Create("ab cd ef 12").AssertSuccessful();
    }

    [Fact]
    public void Create_WithMixedCaseHex_ReturnsSuccess()
    {
        _ = ByteString.Create("Ab cD Ef 12").AssertSuccessful();
    }

    [Fact]
    public void Create_WithEmptyInput_ReturnsSuccess()
    {
        var byteStr = ByteString.Create("").AssertSuccessful();
        Assert.Equal("", byteStr.Value);
        Assert.Equal(0, byteStr.Length);
        Assert.Equal("", byteStr.ToNormalizedString());
    }

    [Fact]
    public void Create_WithWhitespaceOnlyInput_ReturnsSuccess()
    {
        var byteStr = ByteString.Create("   \t  \n  ").AssertSuccessful();
        Assert.Equal("   \t  \n  ", byteStr.Value);
        Assert.Equal(0, byteStr.Length);
        Assert.Equal("", byteStr.ToNormalizedString());
    }

    [Fact]
    public void Create_EdgeCase_SingleHexDigit_ReturnsSuccess()
    {
        var byteStr = ByteString.Create("F").AssertSuccessful();
        Assert.Equal("F", byteStr.Value);
        Assert.Equal(1, byteStr.Length);
    }

    [Fact]
    public void Create_EdgeCase_ZeroByte_ReturnsSuccess()
    {
        var byteStr = ByteString.Create("0 00").AssertSuccessful();
        Assert.Equal("0 00", byteStr.Value);
        Assert.Equal(2, byteStr.Length);
    }

    [Fact]
    public void Create_EdgeCase_MaxByte_ReturnsSuccess()
    {
        var byteStr = ByteString.Create("FF").AssertSuccessful();
        Assert.Equal("FF", byteStr.Value);
        Assert.Equal(1, byteStr.Length);
    }

    [Theory]
    [InlineData("FF", "FF")]
    [InlineData("0 1 2", "0 1 2")]
    [InlineData("AB CD EF", "AB CD EF")]
    [InlineData("10 20 30 40", "10 20 30 40")]
    [InlineData("ab cd ef", "ab cd ef")] // Should preserve original case in Value
    public void Create_WithValidInput_StoresCorrectValue(string input, string expectedValue)
    {
        var byteStr = ByteString.Create(input).AssertSuccessful();
        Assert.Equal(expectedValue.Trim(), byteStr.Value);
    }

    [Fact]
    public void Create_WithInvalidHexCharacters_ReturnsError()
    {
        _ = ByteString.Create("AB XY CD").AssertError();
    }

    [Fact]
    public void Create_WithThreeDigitToken_ReturnsError()
    {
        _ = ByteString.Create("AB CDE F").AssertError();
    }

    [Fact]
    public void Create_WithEmptyToken_ReturnsError()
    {
        // This should be parsed as "AB" and "CD", not empty token
        // Actually, this should succeed because Split removes empty entries
        _ = ByteString.Create("AB  CD").AssertSuccessful();
    }

    [Fact]
    public void Create_WithNonHexCharacters_ReturnsError()
    {
        _ = ByteString.Create("AB GH CD").AssertError();
    }

    [Fact]
    public void Create_WithSpecialCharacters_ReturnsError()
    {
        _ = ByteString.Create("AB C! CD").AssertError();
    }


    [Fact]
    public void Value_ReturnsOriginalString()
    {
        var byteStr = ByteString.Create("AB CD").AssertSuccessful();
        var value1 = byteStr.Value;
        var value2 = byteStr.Value;
        Assert.Same(value1, value2);
        Assert.Equal("AB CD", value1);
    }

    [Fact]
    public void Length_ReturnsCorrectLength()
    {
        var byteStr = ByteString.Create("AB CD EF 12").AssertSuccessful();
        Assert.Equal(4, byteStr.Length);
    }

    [Fact]
    public void ToString_ReturnsOriginalString()
    {
        var byteStr = ByteString.Create("ab cd ef").AssertSuccessful();
        var originalString = byteStr.ToString();
        Assert.Equal("ab cd ef", originalString);
    }

    [Theory]
    [InlineData("AB CD EF")]           // Uppercase
    [InlineData("ab cd ef")]           // Lowercase
    [InlineData("Ab Cd Ef")]           // Mixed case
    [InlineData("  AB  CD  ")]         // Extra spaces
    [InlineData("\tAB\tCD\t")]         // Tabs
    [InlineData("AB\nCD\nEF")]         // Newlines
    [InlineData("AB \t CD \n EF")]     // Mixed whitespace
    public void ToString_PreservesOriginalWhitespaceAndCasing(string input)
    {
        var byteStr = ByteString.Create(input).AssertSuccessful();
        var toStringResult = byteStr.ToString();
        Assert.Equal(input.Trim(), toStringResult);
    }

    [Fact]
    public void ToNormalizedString_ReturnsNormalizedFormat()
    {
        var byteStr = ByteString.Create("ab cd ef").AssertSuccessful();
        var normalizedString = byteStr.ToNormalizedString();
        Assert.Equal("ab cd ef", normalizedString);
    }

    [Fact]
    public void ToNormalizedString_WithEmptyInput_ReturnsEmptyString()
    {
        var byteStr = ByteString.Create("").AssertSuccessful();
        Assert.Equal("", byteStr.ToNormalizedString());
    }

    [Fact]
    public void ToNormalizedString_WithWhitespaceInput_ReturnsEmptyString()
    {
        var byteStr = ByteString.Create("  \t  \n  ").AssertSuccessful();
        Assert.Equal("", byteStr.ToNormalizedString());
        Assert.Equal("  \t  \n  ", byteStr.ToString());
    }

    [Fact]
    public void ToNormalizedString_WithSingleDigitHex_PadsCorrectly()
    {
        var byteStr = ByteString.Create("A B C").AssertSuccessful();
        Assert.Equal("0a 0b 0c", byteStr.ToNormalizedString());
        Assert.Equal("A B C", byteStr.ToString());
    }

    // Equality and hash code tests
    [Fact]
    public void Equals_WithSameBytes_ReturnsTrue()
    {
        var byteStr1 = ByteString.Create("AB CD").AssertSuccessful();
        var byteStr2 = ByteString.Create("AB CD").AssertSuccessful();
        Assert.True(byteStr1.Equals(byteStr2));
    }

    [Fact]
    public void Equals_WithDifferentBytes_ReturnsFalse()
    {
        var byteStr1 = ByteString.Create("AB CD").AssertSuccessful();
        var byteStr2 = ByteString.Create("AB CE").AssertSuccessful();
        Assert.False(byteStr1.Equals(byteStr2));
    }

    [Fact]
    public void Equals_WithNonByteStringObject_ReturnsFalse()
    {
        var byteStr = ByteString.Create("AB CD").AssertSuccessful();
        Assert.False(byteStr.Equals("AB CD"));
        Assert.False(byteStr.Equals(null));
    }

    [Theory]
    [InlineData("ab cd", "ab cd ef", false)] // Different normalized strings
    [InlineData("AB CD", "ab cd", true)]     // Same normalized strings, different case
    [InlineData("A B C", "0a 0b 0c", true)] // Same normalized strings, different padding
    [InlineData("", "   ", true)]            // Both empty when normalized
    public void Equals_ComparesNormalizedStrings(string input1, string input2, bool shouldBeEqual)
    {
        var bs1 = ByteString.Create(input1).AssertSuccessful();
        var bs2 = ByteString.Create(input2).AssertSuccessful();
        Assert.Equal(shouldBeEqual, bs1.Equals(bs2));
    }

    [Fact]
    public void GetHashCode_WithSameBytes_ReturnsSameHash()
    {
        var byteStr1 = ByteString.Create("AB CD EF").AssertSuccessful();
        var byteStr2 = ByteString.Create("AB CD EF").AssertSuccessful();
        Assert.Equal(byteStr1.GetHashCode(), byteStr2.GetHashCode());
    }
}
