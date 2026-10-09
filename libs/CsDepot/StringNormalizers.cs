//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text;

namespace CsDepot;


/// <summary>
/// Reusable string cleanup.  Each normalizer always succeeds and returns a new
/// string.  Apply normalizers before validating with
/// <see cref="StringValidators"/>, so that validation sees the cleaned-up value.
/// </summary>
public static class StringNormalizers
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Converts the value to Unicode Normalization Form C, in which each
    /// character that has a single precomposed code point is represented by it.
    /// For example, "e" followed by a combining acute accent (U+0065 U+0301)
    /// becomes "é" (U+00E9).  Visually identical strings that were typed
    /// differently then compare as equal.
    /// </summary>
    public static string
    NormalizeFormC(string value) => value.Normalize(NormalizationForm.FormC);
}
