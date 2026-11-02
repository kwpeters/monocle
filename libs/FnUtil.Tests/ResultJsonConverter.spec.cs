//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;

namespace FnUtil.Tests;


public class ResultJsonConverterTests
{
    //------------------------------------------------------------------------------
    // Static fields
    //------------------------------------------------------------------------------

    // FnUtil cannot reference CsDepot (CsDepot references FnUtil), so these tests
    // cannot use CsDepot.JsonOptions.Default.  These options match its strict
    // handling of missing and null values.
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true
    };

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static TheoryData<string, Result<int, string>>
    Results() =>
        new()
        {
            { """{"kind":"success","value":42}""", SuccessE<int, string>(42) },
            { """{"kind":"error","value":"Not found."}""", ErrorE<int, string>("Not found.") },
        };


    public static TheoryData<Result<Option<int>, string>>
    NestedResults() =>
        new(
            SuccessE<Option<int>, string>(Some(1)),
            SuccessE<Option<int>, string>(none),
            ErrorE<Option<int>, string>("Not found.")
        );


    private static T
    Deserialize<T>(
        string json
    ) =>
        JsonSerializer.Deserialize<T>(json, _options) ??
            throw new InvalidOperationException($"Deserialized a null {typeof(T)}.");

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ResultJsonConverterFactory.CanConvert ────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Theory]
    [InlineData(typeof(Result<int, string>), true)]
    [InlineData(typeof(Option<int>), false)]
    [InlineData(typeof(int), false)]
    public void CanConvert_WhenGivenType_ReturnsWhetherItIsAResult(Type type, bool expected)
    {
        Assert.Equal(expected, new ResultJsonConverterFactory().CanConvert(type));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ResultJsonConverter.Read ─────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Theory]
    [MemberData(nameof(Results))]
    public void Read_WhenKindIsValid_ReturnsThatCase(string json, Result<int, string> expected)
    {
        Assert.Equal(expected, Deserialize<Result<int, string>>(json));
    }


    [Fact]
    public void Read_WhenKindIsNotFirst_ReturnsThatCase()
    {
        Result<int, string> result = Deserialize<Result<int, string>>("""{"value":42,"kind":"success"}""");

        Assert.Equal(SuccessE<int, string>(42), result);
    }


    [Theory]
    [InlineData("null")]                                    // null instead of an object
    [InlineData("42")]                                      // bare value
    [InlineData("""{}""")]                                  // missing kind
    [InlineData("""{"kind":"some","value":42}""")]          // unknown kind
    [InlineData("""{"kind":"error"}""")]                    // missing value
    [InlineData("""{"kind":"error","value":null}""")]       // null value
    public void Read_WhenInputIsMalformed_ThrowsJsonException(string json)
    {
        Assert.Throws<JsonException>(() => Deserialize<Result<int, string>>(json));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ResultJsonConverter.Write ────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Theory]
    [MemberData(nameof(Results))]
    public void
    Write_WhenGivenResult_WritesKindAndValue(
        string expected,
        Result<int, string> result
    )
    {
        Assert.Equal(expected, JsonSerializer.Serialize(result, _options));
    }


    // A null value could not be read back, so it is rejected when written.
    [Fact]
    public void Write_WhenValueIsNull_ThrowsJsonException()
    {
        Result<string?, string> result = SuccessE<string?, string>(null);

        Assert.Throws<JsonException>(() => JsonSerializer.Serialize(result, _options));
    }


    [Theory]
    [MemberData(nameof(NestedResults))]
    public void Write_WhenReadBack_RoundTrips(Result<Option<int>, string> result)
    {
        string json = JsonSerializer.Serialize(result, _options);

        Assert.Equal(result, Deserialize<Result<Option<int>, string>>(json));
    }
}
