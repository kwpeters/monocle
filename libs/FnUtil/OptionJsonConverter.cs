//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using static FnUtil.F;

namespace FnUtil;


/// <summary>
/// Creates the JSON converter for each Option&lt;T&gt;.  It is attached to
/// Option&lt;T&gt; with [JsonConverter], so it works with any
/// JsonSerializerOptions without being registered.
///
/// Options use the tagged format described in <see cref="TaggedJson"/>:
/// <code>
/// { "kind": "some", "value": 42 }
/// { "kind": "none" }
/// </code>
/// </summary>
public sealed class OptionJsonConverterFactory : JsonConverterFactory
{
    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);


    public override JsonConverter
    CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        Type converterType =
            typeof(OptionJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments());

        return Activator.CreateInstance(converterType) as JsonConverter ??
            throw new UnreachableException($"Could not create {converterType}.");
    }
}


/// <summary>
/// Reads and writes one Option&lt;T&gt;.
/// </summary>
#pragma warning disable CA1812      // Created through reflection by OptionJsonConverterFactory.
internal sealed class OptionJsonConverter<T> : JsonConverter<Option<T>>
#pragma warning restore CA1812
{
    //------------------------------------------------------------------------------
    // Constants
    //------------------------------------------------------------------------------

    private const string SomeKind = "some";

    private const string NoneKind = "none";

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    // Have Read() and Write() see null, instead of the serializer, so that a JSON
    // null is rejected rather than becoming a null Option.
    public override bool HandleNull => true;

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    public override Option<T>
    Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = TaggedJson.ParseObject(ref reader, typeToConvert);
        JsonElement tagged = document.RootElement;
        JsonElement kind = TaggedJson.Kind(tagged, typeToConvert);

        if (kind.ValueEquals(NoneKind)) return none;
        if (kind.ValueEquals(SomeKind))
            return Some(TaggedJson.ReadValue<T>(tagged, typeToConvert, options));

        throw TaggedJson.UnknownKind(kind, typeToConvert);
    }


    public override void
    Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(value);

        value.Match(
            (some) => TaggedJson.WriteCase(writer, SomeKind, some, options),
            () => TaggedJson.WriteCase(writer, NoneKind));
    }
}
