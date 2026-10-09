//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Globalization;
using FnUtil;
using static FnUtil.F;

namespace CsDepot;


/// <summary>
/// Reusable string checks.  Each validator only checks its input; it never
/// modifies it.  On success the input is returned unchanged so validators can
/// be chained with Bind.  To clean up input before validating it, see
/// <see cref="StringNormalizers"/>.
/// </summary>
public static class StringValidators
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Fails if the value is empty or consists only of whitespace.
    /// </summary>
    /// <param name="value">The string to check</param>
    /// <param name="fieldName">The name used to identify the value in the error message</param>
    public static Result<string, string>
    NonBlank(string value, string fieldName) =>
        string.IsNullOrWhiteSpace(value) ?
            Error($"{fieldName} must not be blank.") :
            Success(value);


    /// <summary>
    /// Fails if the value contains more than <paramref name="maxLength"/>
    /// user-perceived characters (extended grapheme clusters).  For example,
    /// "👍🏽" and "e" followed by a combining accent each count as one.
    ///
    /// Use this for limits that are communicated to people, such as "at most
    /// 40 characters."
    /// </summary>
    /// <param name="value">The string to check</param>
    /// <param name="maxLength">The maximum number of text elements allowed</param>
    /// <param name="fieldName">The name used to identify the value in the error message</param>
    public static Result<string, string>
    MaxLengthInTextElements(string value, int maxLength, string fieldName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

        return new StringInfo(value).LengthInTextElements > maxLength ?
            Error(string.Create(CultureInfo.InvariantCulture, $"{fieldName} must not exceed {maxLength} characters.")) :
            Success(value);
    }


    /// <summary>
    /// Fails if the value contains more than <paramref name="maxLength"/>
    /// UTF-16 code units (that is, if <c>value.Length</c> exceeds the limit).
    /// For example, "👍🏽" counts as four.
    ///
    /// Use this for limits imposed by storage that measures length in UTF-16
    /// code units, such as .NET buffers or SQL Server nvarchar columns.
    /// </summary>
    /// <param name="value">The string to check</param>
    /// <param name="maxLength">The maximum number of UTF-16 code units allowed</param>
    /// <param name="fieldName">The name used to identify the value in the error message</param>
    public static Result<string, string>
    MaxLengthInCodeUnits(string value, int maxLength, string fieldName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

        return value.Length > maxLength ?
            Error(string.Create(CultureInfo.InvariantCulture, $"{fieldName} must not exceed {maxLength} UTF-16 code units.")) :
            Success(value);
    }
}
