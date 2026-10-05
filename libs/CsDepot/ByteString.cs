using System.Text.RegularExpressions;

namespace CsDepot;

/// <summary>
/// Represents a validated hexadecimal byte string.
/// </summary>
public class ByteString
{
    /// <summary>
    /// Private constructor to enforce validation through Create method.
    /// </summary>
    /// <param name="hexString">The validated hex string</param>
    private ByteString(string hexString)
    {
        this.Value = hexString;
    }

    /// <summary>
    /// Gets the original validated hex string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the number of hex tokens (bytes) in the string.
    /// </summary>
    public int Length => this.Value.Split(
        Array.Empty<char>(),
        StringSplitOptions.RemoveEmptyEntries
    ).Length;

    /// <summary>
    /// Creates a new ByteString from a hexadecimal string representation.
    /// </summary>
    /// <param name="hexString">
    /// A string containing hexadecimal bytes separated by whitespace.
    /// Each byte may be represented by 1 or 2 hexadecimal characters.
    /// </param>
    /// <returns>
    /// A Result containing the ByteString on success, or an error message on failure.
    /// </returns>
    public static Result<ByteString, string> Create(string hexString)
    {
        // Accept empty or whitespace-only strings as valid (zero-length byte string)
        if (string.IsNullOrWhiteSpace(hexString))
        {
            return Success(new ByteString(hexString));
        }

        try
        {
            // Split by whitespace and remove empty entries
            var hexTokens = hexString.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

            if (hexTokens.Length == 0)
            {
                return Error("No valid hex tokens found in input string.");
            }

            foreach (var token in hexTokens)
            {
                // Validate that token contains only hex characters and is 1-2 characters long
                if (token.Length is < 1 or > 2)
                {
                    return Error($"Invalid hex token '{token}': must be 1 or 2 characters long.");
                }

                if (!IsValidHexToken(token))
                {
                    return Error($"Invalid hex token '{token}': contains non-hexadecimal characters.");
                }

                // Validate that it can be parsed as a byte (this ensures it's valid hex)
                if (!byte.TryParse(token, System.Globalization.NumberStyles.HexNumber, null, out _))
                {
                    return Error($"Invalid hex token '{token}': cannot be parsed as a valid byte value.");
                }
            }

            // Store the original validated hex string
            return Success(new ByteString(hexString.Trim()));
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
        {
            return Error($"Error validating hex string: {ex.Message}");
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>
    /// Validates that a token contains only valid hexadecimal characters.
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>True if the token is valid hex, false otherwise</returns>
    private static bool IsValidHexToken(string token) => Regex.IsMatch(token, @"^[0-9A-Fa-f]+$");

    /// <summary>
    /// Returns the original input string that was used to create this ByteString.
    /// </summary>
    /// <returns>The original hex string as provided to Create method</returns>
    public override string ToString() => this.Value;

    /// <summary>
    /// Returns a normalized string representation of the ByteString.
    /// </summary>
    /// <returns>
    /// A normalized hex string representation with bytes separated by a single space character
    /// and lowercase hexadecimal digits
    /// </returns>
    public string ToNormalizedString()
    {
        // Handle empty/whitespace strings
        var tokens = this.Value.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
        {
            return string.Empty;
        }

        // Normalize the hex string: split tokens, convert to lowercase, join with spaces
#pragma warning disable CA1308 // We want to normalize to lowercase for readability
        return string.Join(" ",
            tokens.Select(token => token.PadLeft(2, '0').ToLowerInvariant()));
#pragma warning restore CA1308
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current ByteString.
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object? obj)
    {
        if (obj is ByteString other)
        {
            // Compare normalized representations to handle case differences and spacing
            return this.ToNormalizedString().Equals(
                other.ToNormalizedString(), StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for the current ByteString.
    /// </summary>
    /// <returns>A hash code for the current ByteString</returns>
    public override int GetHashCode() =>
        // Use the normalized string representation for consistent hashing
        this.ToNormalizedString().GetHashCode(StringComparison.OrdinalIgnoreCase);
}
