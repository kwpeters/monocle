//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;
using CsDepot;
using ExampleLib.Drawing;

namespace ExampleLib.Tests.Drawing;


public class DrawableDtoTests
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    // One sample per case.  Add a sample whenever a case is added.
    public static TheoryData<DrawableDto> Samples() =>
        new(
            new DrawableDto.CircleCase(new CircleDto { Radius = 2 }),
            new DrawableDto.LabelCase(new LabelDto { Content = "Hello" })
        );

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Serialization ────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Theory]
    [MemberData(nameof(Samples))]
    public void Serialize_WhenDeserialized_RoundTrips(DrawableDto dto)
    {
        string json = JsonSerializer.Serialize<DrawableDto>(dto, JsonOptions.Default);
        Option<DrawableDto> copy = JsonSerializer.Deserialize<DrawableDto>(json, JsonOptions.Default).ToOption();

        Assert.Equal(Some(dto), copy);
    }


    [Fact]
    public void Serialize_WhenGivenCircle_UsesTheSameKindAsShapeDto()
    {
        CircleDto circle = new() { Radius = 2 };

        string drawableJson = JsonSerializer.Serialize<DrawableDto>(new DrawableDto.CircleCase(circle), JsonOptions.Default);
        string shapeJson = JsonSerializer.Serialize<ShapeDto>(new ShapeDto.CircleCase(circle), JsonOptions.Default);

        Assert.Equal(shapeJson, drawableJson);
    }


    [Theory]
    [InlineData("""{"kind":"label"}""")]                  // missing value
    [InlineData("""{"kind":"label","value":null}""")]     // null value
    [InlineData("""{"kind":"hexagon","value":{}}""")]     // unknown kind
    public void Deserialize_WhenInputIsMalformed_Throws(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<DrawableDto>(json, JsonOptions.Default));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FromDomain / ToDomain ────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    // Driven by the domain union's samples, so a Drawable case with no matching
    // mapping arm fails here with UnreachableException.
    [Theory]
    [MemberData(nameof(DrawableTests.Samples), MemberType = typeof(DrawableTests))]
    public void FromDomain_WhenSerializedAndMappedBack_RoundTrips(Drawable drawable)
    {
        string json = JsonSerializer.Serialize<DrawableDto>(DrawableDtoExt.FromDomain(drawable), JsonOptions.Default);
        DrawableDto dto = JsonSerializer.Deserialize<DrawableDto>(json, JsonOptions.Default).ToOption().ThrowIfNone();

        Assert.Equal(drawable, dto.ToDomain().GetValue());
    }
}
