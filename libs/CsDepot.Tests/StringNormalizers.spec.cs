//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace CsDepot.Tests;


public class StringNormalizersTests
{
    //
    // ══════════════════════════════════════════════════════════════════════
    // ── NormalizeFormC ───────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Theory]
    [InlineData("Jose\u0301", "Jos\u00E9")]        // e + combining acute -> precomposed e-acute
    [InlineData("Pen\u0303a", "Pe\u00F1a")]        // n + combining tilde -> precomposed n-tilde
    [InlineData("Mu\u0308ller", "M\u00FCller")]    // u + combining diaeresis -> precomposed u-umlaut
    public void NormalizeFormC_WhenGivenCombiningMarks_ComposesThem(string value, string expected)
    {
        string normalized = StringNormalizers.NormalizeFormC(value);

        Assert.Equal(expected, normalized);
        Assert.Equal(expected.Length, normalized.Length);
    }


    [Theory]
    [InlineData("Doe")]
    [InlineData("Pe\u00F1a")]                      // Already composed
    [InlineData("")]
    public void NormalizeFormC_WhenAlreadyNormalized_ReturnsEqualString(string value)
    {
        Assert.Equal(value, StringNormalizers.NormalizeFormC(value));
    }


    [Fact]
    public void NormalizeFormC_MakesDifferentlyTypedStringsEqual()
    {
        string composed = "Pe\u00F1a";
        string decomposed = "Pen\u0303a";

        Assert.NotEqual(composed, decomposed);
        Assert.Equal(
            StringNormalizers.NormalizeFormC(composed),
            StringNormalizers.NormalizeFormC(decomposed));
    }
}
