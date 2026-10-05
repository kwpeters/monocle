//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

using System.Text.RegularExpressions;
using CsDepot;

namespace CsDepot.Tests;


public class StringExtTests
{


    [Fact]
    public void Prepend_WithValidInputs_PrependsCorrectly()
    {
        var result = "world".Prepend("Hello ");
        Assert.Equal("Hello world", result);
    }


    [Fact]
    public void Prepend_WithEmptyPrefix_ReturnsOriginalString()
    {
        var result = "test".Prepend("");
        Assert.Equal("test", result);
    }


    [Fact]
    public void Prepend_WithEmptyTarget_ReturnsPrefixOnly()
    {
        var result = "".Prepend("prefix");
        Assert.Equal("prefix", result);
    }


    [Fact]
    public void Prepend_WithBothEmpty_ReturnsEmptyString()
    {
        var result = "".Prepend("");
        Assert.Equal("", result);
    }


    [Fact]
    public void IntersperseRight_WithValidInputs_ReturnsExpectedString()
    {
        var result = "9876543210".IntersperseRight("_", 2);
        Assert.Equal("98_76_54_32_10", result);
    }


    [Fact]
    public void IntersperseRight_WithGroupSize3_IntersperseCorrectly()
    {
        var result = "123456789".IntersperseRight("_", 3);
        Assert.Equal("123_456_789", result);
    }


    [Fact]
    public void IntersperseRight_WithGroupSize4_IntersperseCorrectly()
    {
        var result = "1111222233334444".IntersperseRight("_", 4);
        Assert.Equal("1111_2222_3333_4444", result);
    }


    [Fact]
    public void IntersperseRight_WithStringExactlyDivisibleByGroupSize_NoLeadingInsert()
    {
        var result = "123456".IntersperseRight("_", 3);
        Assert.Equal("123_456", result);
    }


    [Fact]
    public void IntersperseRight_WithStringNotDivisibleByGroupSize_HandlesRemainder()
    {
        var result = "1234567".IntersperseRight("_", 3);
        Assert.Equal("1_234_567", result);
    }


    [Fact]
    public void IntersperseRight_WithSingleCharacterString_ReturnsUnchanged()
    {
        var result = "A".IntersperseRight("_", 2);
        Assert.Equal("A", result);
    }


    [Fact]
    public void IntersperseRight_WithEmptyString_ReturnsEmptyString()
    {
        var result = "".IntersperseRight("_", 2);
        Assert.Equal("", result);
    }


    [Fact]
    public void IntersperseRight_WithGroupSizeLargerThanString_ReturnsUnchanged()
    {
        var result = "ABC".IntersperseRight("_", 10);
        Assert.Equal("ABC", result);
    }


    [Fact]
    public void IntersperseRight_WithGroupSize1_InsertsAfterEveryChar()
    {
        var result = "ABCD".IntersperseRight("_", 1);
        Assert.Equal("A_B_C_D", result);
    }


    [Fact]
    public void IntersperseRight_WithMultiCharInsert_WorksCorrectly()
    {
        var result = "123456".IntersperseRight(" - ", 2);
        Assert.Equal("12 - 34 - 56", result);
    }


    [Fact]
    public void IntersperseRight_WithHexString_FormatsCorrectly()
    {
        var result = "ABCDEF12".IntersperseRight("_", 2);
        Assert.Equal("AB_CD_EF_12", result);
    }


    [Fact]
    public void IntersperseRight_WithBinaryString_FormatsCorrectly()
    {
        var result = "11110000".IntersperseRight("_", 4);
        Assert.Equal("1111_0000", result);
    }


    [Fact]
    public void MatchesAny_WithMatchingPattern_ReturnsTrue()
    {
        var patterns = new[] {
            new Regex(@"^hello"),
            new Regex(@"^world")
        };
        Assert.True("hello there".MatchesAny(patterns));
        Assert.True("world peace".MatchesAny(patterns));
    }


    [Fact]
    public void MatchesAny_WithNoMatchingPattern_ReturnsFalse()
    {
        var patterns = new[] {
            new Regex(@"^hello"),
            new Regex(@"^world")
        };
        Assert.False("goodbye".MatchesAny(patterns));
    }


    [Fact]
    public void MatchesAny_WithEmptyPatternArray_ReturnsFalse()
    {
        var patterns = Array.Empty<Regex>();
        Assert.False("test".MatchesAny(patterns));
    }


    [Fact]
    public void MatchesAny_WithMultiplePatterns_MatchesAny()
    {
        var patterns = new[] {
            new Regex(@"\d{3}-\d{4}"),  // Phone pattern
            new Regex(@"^[A-Z]{2}\d+"),  // Code pattern
            new Regex(@"test")
        };
        Assert.True("AB123".MatchesAny(patterns));
        Assert.True("123-4567".MatchesAny(patterns));
        Assert.True("testing".MatchesAny(patterns));
        Assert.False("nomatch".MatchesAny(patterns));
    }


    [Fact]
    public void MatchesAny_WithCaseSensitivePattern_RespectsCasing()
    {
        var patterns = new[] { new Regex(@"^Hello") };
        Assert.True("Hello world".MatchesAny(patterns));
        Assert.False("hello world".MatchesAny(patterns));
    }


    [Fact]
    public void MatchesAny_WithCaseInsensitivePattern_IgnoresCasing()
    {
        var patterns = new[] { new Regex(@"^hello", RegexOptions.IgnoreCase) };
        Assert.True("Hello world".MatchesAny(patterns));
        Assert.True("hello world".MatchesAny(patterns));
        Assert.True("HELLO world".MatchesAny(patterns));
    }


}
