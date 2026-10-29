//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace CsDepot.Tests;


public class StringValidatorsTests
{
    //
    // ══════════════════════════════════════════════════════════════════════
    // ── NonBlank ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void NonBlank_WhenGivenText_ReturnsSuccessWithSameValue()
    {
        Result<string, string> result = StringValidators.NonBlank("Doe", "lastName");

        Assert.Equal("Doe", result.GetValue());
    }


    [Fact]
    public void NonBlank_WhenGivenTextWithSurroundingWhitespace_DoesNotTrim()
    {
        Result<string, string> result = StringValidators.NonBlank("  Doe  ", "lastName");

        Assert.Equal("  Doe  ", result.GetValue());
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    [InlineData("\u00A0")]  // No-break space
    public void NonBlank_WhenGivenBlankValue_ReturnsError(string value)
    {
        Result<string, string> result = StringValidators.NonBlank(value, "lastName");

        Assert.Equal("lastName must not be blank.", result.GetError());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── MaxLengthInTextElements ──────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void MaxLengthInTextElements_WhenAtLimit_ReturnsSuccessWithSameValue()
    {
        Result<string, string> result = StringValidators.MaxLengthInTextElements("abc", 3, "lastName");

        Assert.Equal("abc", result.GetValue());
    }


    [Fact]
    public void MaxLengthInTextElements_WhenOverLimit_ReturnsError()
    {
        Result<string, string> result = StringValidators.MaxLengthInTextElements("abcd", 3, "lastName");

        Assert.Equal("lastName must not exceed 3 characters.", result.GetError());
    }


    [Fact]
    public void MaxLengthInTextElements_WhenGivenEmptyStringAndZeroLimit_ReturnsSuccess()
    {
        Result<string, string> result = StringValidators.MaxLengthInTextElements("", 0, "lastName");

        Assert.Equal("", result.GetValue());
    }


    [Theory]
    [InlineData("\U0001F44D\U0001F3FD")]                        // Thumbs up + skin tone modifier: 4 UTF-16 code units
    [InlineData("e\u0301")]                                     // "e" + combining acute accent: 2 UTF-16 code units
    [InlineData("\U0001F468\u200D\U0001F469\u200D\U0001F467")]  // Family emoji joined by zero-width joiners: 8 UTF-16 code units
    public void MaxLengthInTextElements_WhenGivenSinglePerceivedCharacter_CountsItAsOne(string value)
    {
        Result<string, string> result = StringValidators.MaxLengthInTextElements(value, 1, "lastName");

        Assert.True(result.IsSuccess);
    }


    [Fact]
    public void MaxLengthInTextElements_WhenGivenNegativeLimit_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => StringValidators.MaxLengthInTextElements("abc", -1, "lastName"));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── MaxLengthInCodeUnits ─────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void MaxLengthInCodeUnits_WhenAtLimit_ReturnsSuccessWithSameValue()
    {
        Result<string, string> result = StringValidators.MaxLengthInCodeUnits("abc", 3, "lastName");

        Assert.Equal("abc", result.GetValue());
    }


    [Fact]
    public void MaxLengthInCodeUnits_WhenOverLimit_ReturnsError()
    {
        Result<string, string> result = StringValidators.MaxLengthInCodeUnits("abcd", 3, "lastName");

        Assert.Equal("lastName must not exceed 3 UTF-16 code units.", result.GetError());
    }


    [Fact]
    public void MaxLengthInCodeUnits_WhenGivenSurrogatePairs_CountsEachCodeUnit()
    {
        // Thumbs up + skin tone modifier is a single perceived character made of 4 UTF-16 code units.
        string thumbsUp = "\U0001F44D\U0001F3FD";

        Assert.True(StringValidators.MaxLengthInCodeUnits(thumbsUp, 4, "lastName").IsSuccess);
        Assert.True(StringValidators.MaxLengthInCodeUnits(thumbsUp, 3, "lastName").IsError);
    }


    [Fact]
    public void MaxLengthInCodeUnits_WhenGivenNegativeLimit_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => StringValidators.MaxLengthInCodeUnits("abc", -1, "lastName"));
    }
}
