//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;

namespace FnUtil;


/// <summary>
/// Reads and writes the tagged JSON format shared by Option and Result.  Each
/// value is an object with a "kind" property naming the case and, for cases that
/// carry a value, a "value" property:
/// <code>
/// { "kind": "some", "value": 42 }
/// { "kind": "none" }
/// </code>
/// This is the same format used for unions elsewhere in this repo.  "kind" may
/// appear in any position, and values nest to any depth.
/// </summary>
internal static class TaggedJson
{
    //------------------------------------------------------------------------------
    // Constants
    //------------------------------------------------------------------------------

    public const string KindProperty = "kind";

    public const string ValueProperty = "value";

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Reads the whole object at the reader, so that its properties can be read in
    /// any order.  The caller disposes the returned document.
    /// </summary>
    public static JsonDocument ParseObject(ref Utf8JsonReader reader, Type typeToConvert)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException(
                $"Expected a JSON object with a \"{KindProperty}\" property for {typeToConvert}, " +
                $"not {reader.TokenType}.");
        }

        return JsonDocument.ParseValue(ref reader);
    }


    /// <summary>
    /// Returns the object's "kind" property, which must be a string.
    /// </summary>
    public static JsonElement Kind(JsonElement tagged, Type typeToConvert)
    {
        bool hasKind =
            tagged.TryGetProperty(KindProperty, out JsonElement kind) &&
            kind.ValueKind == JsonValueKind.String;

        return hasKind ?
            kind :
            throw new JsonException(
                $"A JSON {typeToConvert} has no string \"{KindProperty}\" property.");
    }


    public static JsonException UnknownKind(JsonElement kind, Type typeToConvert) =>
        new($"\"{kind.GetString()}\" is not a valid \"{KindProperty}\" for {typeToConvert}.");


    /// <summary>
    /// Returns the object's "value" property deserialized as T.  The value must be
    /// present and must not be null.
    /// </summary>
    public static T
    ReadValue<T>(JsonElement tagged, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!tagged.TryGetProperty(ValueProperty, out JsonElement valueElement))
        {
            throw new JsonException($"A JSON {typeToConvert} has no \"{ValueProperty}\" property.");
        }

        return valueElement.Deserialize<T>(options) is { } value ?
            value :
            throw new JsonException(
                $"The \"{ValueProperty}\" of a JSON {typeToConvert} must not be null.");
    }


    /// <summary>
    /// Writes a case that carries no value: { "kind": kind }
    /// </summary>
    public static void WriteCase(Utf8JsonWriter writer, string kind)
    {
        writer.WriteStartObject();
        writer.WriteString(KindProperty, kind);
        writer.WriteEndObject();
    }


    /// <summary>
    /// Writes a case that carries a value: { "kind": kind, "value": value }.  A null
    /// value is rejected, because ReadValue() could not read it back.
    /// </summary>
    public static void
    WriteCase<T>(Utf8JsonWriter writer, string kind, T value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            throw new JsonException($"Cannot write a null \"{ValueProperty}\" for \"{kind}\".");
        }

        writer.WriteStartObject();
        writer.WriteString(KindProperty, kind);
        writer.WritePropertyName(ValueProperty);
        JsonSerializer.Serialize(writer, value, options);
        writer.WriteEndObject();
    }
}
