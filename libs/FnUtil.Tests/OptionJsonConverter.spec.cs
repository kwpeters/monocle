//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;

namespace FnUtil.Tests;


public class OptionJsonConverterTests
{
    //------------------------------------------------------------------------------
    // Static fields
    //------------------------------------------------------------------------------

    // FnUtil cannot reference CsDepot (CsDepot references FnUtil), so these tests
    // cannot use CsDepot.JsonOptions.Default.  These options match its strict
    // handling of missing and null values.
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web) {
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true
    };

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static TheoryData<Contact>
    Contacts() =>
        new(
            new Contact { Name = "Jane", MiddleName = Some("Q"), Age = Some(42) },
            new Contact { Name = "Jane", MiddleName = none, Age = Some(0) },
            new Contact { Name = "Jane", MiddleName = Some(""), Age = none }
        );


    public static TheoryData<string, Option<Option<int>>>
    NestedOptions() =>
        new()
        {
            { """{"kind":"none"}""", none },
            { """{"kind":"some","value":{"kind":"none"}}""", Some(NoneE<int>()) },
            { """{"kind":"some","value":{"kind":"some","value":1}}""", Some(Some(1)) },
        };


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
    // ── OptionJsonConverterFactory.CanConvert ────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Theory]
    [InlineData(typeof(Option<int>), true)]
    [InlineData(typeof(Option<Option<string>>), true)]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(List<int>), false)]
    public void CanConvert_WhenGivenType_ReturnsWhetherItIsAnOption(Type type, bool expected)
    {
        Assert.Equal(expected, new OptionJsonConverterFactory().CanConvert(type));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── OptionJsonConverter.Read ─────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Read_WhenKindIsSome_ReturnsSome()
    {
        Contact contact = Deserialize<Contact>(
            """{"name":"Jane","middleName":{"kind":"some","value":"Q"},"age":{"kind":"some","value":42}}""");

        Assert.Equal(Some("Q"), contact.MiddleName);
        Assert.Equal(Some(42), contact.Age);
    }


    [Fact]
    public void Read_WhenKindIsNone_ReturnsNone()
    {
        Contact contact = Deserialize<Contact>(
            """{"name":"Jane","middleName":{"kind":"none"},"age":{"kind":"none"}}""");

        Assert.Equal(NoneE<string>(), contact.MiddleName);
        Assert.Equal(NoneE<int>(), contact.Age);
    }


    [Fact]
    public void Read_WhenKindIsNotFirst_ReturnsSome()
    {
        Option<string> option = Deserialize<Option<string>>("""{"value":"Q","kind":"some"}""");

        Assert.Equal(Some("Q"), option);
    }


    [Theory]
    [MemberData(nameof(NestedOptions))]
    public void Read_WhenOptionsAreNested_ReadsEveryLevel(string json, Option<Option<int>> expected)
    {
        Assert.Equal(expected, Deserialize<Option<Option<int>>>(json));
    }


    [Theory]
    [InlineData("null")]                                    // null instead of an object
    [InlineData("\"Q\"")]                                   // bare value
    [InlineData("""{}""")]                                  // missing kind
    [InlineData("""{"kind":1}""")]                          // kind is not a string
    [InlineData("""{"kind":"maybe"}""")]                    // unknown kind
    [InlineData("""{"kind":"some"}""")]                     // missing value
    [InlineData("""{"kind":"some","value":null}""")]        // null value
    [InlineData("""{"kind":"some","value":7}""")]           // value of the wrong type
    public void Read_WhenInputIsMalformed_ThrowsJsonException(string json)
    {
        Assert.Throws<JsonException>(() => Deserialize<Option<string>>(json));
    }


    // A required Option property must appear in the JSON, even when it is None.
    [Fact]
    public void Read_WhenRequiredPropertyIsMissing_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(
            () => Deserialize<Contact>("""{"name":"Jane","age":{"kind":"none"}}"""));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── OptionJsonConverter.Write ────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Write_WhenSome_WritesKindAndValue()
    {
        string json = JsonSerializer.Serialize(Some("Q"), _options);

        Assert.Equal("""{"kind":"some","value":"Q"}""", json);
    }


    [Fact]
    public void Write_WhenNone_WritesKindOnly()
    {
        string json = JsonSerializer.Serialize(NoneE<string>(), _options);

        Assert.Equal("""{"kind":"none"}""", json);
    }


    [Theory]
    [MemberData(nameof(NestedOptions))]
    public void
    Write_WhenOptionsAreNested_WritesEveryLevel(
        string expected,
        Option<Option<int>> option
    )
    {
        Assert.Equal(expected, JsonSerializer.Serialize(option, _options));
    }


    [Theory]
    [MemberData(nameof(Contacts))]
    public void Write_WhenReadBack_RoundTrips(Contact contact)
    {
        string json = JsonSerializer.Serialize(contact, _options);

        Assert.Equal(contact, Deserialize<Contact>(json));
    }

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class Contact
    {
        public required string Name { get; init; }
        public required Option<string> MiddleName { get; init; }
        public required Option<int> Age { get; init; }
    }
}
