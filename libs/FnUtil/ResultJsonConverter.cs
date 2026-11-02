//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using static FnUtil.F;

namespace FnUtil;


/// <summary>
/// Creates the JSON converter for each Result&lt;TSuccess, TError&gt;.  It is
/// attached to Result&lt;TSuccess, TError&gt; with [JsonConverter], so it works
/// with any JsonSerializerOptions without being registered.
///
/// Results use the tagged format described in <see cref="TaggedJson"/>:
/// <code>
/// { "kind": "success", "value": 42 }
/// { "kind": "error", "value": "Not found." }
/// </code>
/// </summary>
public sealed class ResultJsonConverterFactory : JsonConverterFactory
{
    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Result<,>);


    public override JsonConverter
    CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        Type converterType =
            typeof(ResultJsonConverter<,>).MakeGenericType(typeToConvert.GetGenericArguments());

        return Activator.CreateInstance(converterType) as JsonConverter ??
            throw new UnreachableException($"Could not create {converterType}.");
    }
}


/// <summary>
/// Reads and writes one Result&lt;TSuccess, TError&gt;.  Neither the success nor the
/// error value may be null.
/// </summary>
#pragma warning disable CA1812      // Created through reflection by ResultJsonConverterFactory.
internal sealed class ResultJsonConverter<TSuccess, TError> : JsonConverter<Result<TSuccess, TError>>
#pragma warning restore CA1812
{
    //------------------------------------------------------------------------------
    // Constants
    //------------------------------------------------------------------------------

    private const string SuccessKind = "success";

    private const string ErrorKind = "error";

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    // Have Read() and Write() see null, instead of the serializer, so that a JSON
    // null is rejected rather than becoming a null Result.
    public override bool HandleNull => true;

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    public override Result<TSuccess, TError>
    Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = TaggedJson.ParseObject(ref reader, typeToConvert);
        JsonElement tagged = document.RootElement;
        JsonElement kind = TaggedJson.Kind(tagged, typeToConvert);

        if (kind.ValueEquals(SuccessKind))
        {
            TSuccess success = TaggedJson.ReadValue<TSuccess>(tagged, typeToConvert, options);
            return SuccessE<TSuccess, TError>(success);
        }

        if (kind.ValueEquals(ErrorKind))
        {
            TError error = TaggedJson.ReadValue<TError>(tagged, typeToConvert, options);
            return ErrorE<TSuccess, TError>(error);
        }

        throw TaggedJson.UnknownKind(kind, typeToConvert);
    }


    public override void
    Write(Utf8JsonWriter writer, Result<TSuccess, TError> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(value);

        value.Match(
            (success) => TaggedJson.WriteCase(writer, SuccessKind, success, options),
            (error) => TaggedJson.WriteCase(writer, ErrorKind, error, options));
    }
}
