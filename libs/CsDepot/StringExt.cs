//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

using System.Text.RegularExpressions;

namespace CsDepot;


/// <summary>
/// Extension methods for string manipulation.
/// </summary>
public static class StringExt
{


    /// <summary>
    /// Prepends a prefix to the target string.
    /// </summary>
    /// <param name="target">The target string</param>
    /// <param name="prefix">The prefix to prepend</param>
    /// <returns>The concatenated string with prefix before target</returns>
    public static string Prepend(this string target, string prefix) =>
        $"{prefix}{target}";


    /// <summary>
    /// Intersperses a string at regular intervals from the right side of the
    /// target string.
    /// </summary>
    /// <param name="target">The target string to intersperse</param>
    /// <param name="inserted">The string to insert between groups</param>
    /// <param name="groupSize">The size of each group from the right</param>
    /// <returns>The string with inserted separators at regular intervals from
    /// the right</returns>
    public static string IntersperseRight(this string target, string inserted, uint groupSize)
    {
        int inGroup = 0;
        int curIndex = target.Length - 1;
        for (; curIndex > 0; curIndex--)
        {
            inGroup++;
            if (inGroup == groupSize)
            {
                target = target[..curIndex] + inserted + target[curIndex..];
                inGroup = 0;
            }
        }
        return target;
    }


    /// <summary>
    /// Determines whether the string matches any of the provided regular
    /// expression patterns.
    /// </summary>
    /// <param name="str">The string to test against the patterns.</param>
    /// <param name="patterns">
    /// An array of regular expression patterns to test. The string is
    /// considered a match if it matches at least one pattern.
    /// </param>
    /// <returns>
    /// True if the string matches at least one of the patterns; otherwise,
    /// false.
    /// </returns>
    public static bool MatchesAny(this string str, Regex[] patterns)
 => patterns.Any((pattern) => pattern.Match(str, 0).Success);


}
