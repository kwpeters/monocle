//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;
using CsDepot;
using ExampleLib.Drawing;

namespace ExampleLib.Tests.Drawing;


public class ShapeDtoTests
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    // One sample per case.  Add a sample whenever a case is added.
    public static TheoryData<ShapeDto> Samples() =>
        new(
            new ShapeDto.CircleCase(new CircleDto { Radius = 2 }),
            new ShapeDto.RectCase(new RectDto { Width = 3, Height = 4 })
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
    public void Serialize_WhenDeserialized_RoundTrips(ShapeDto dto)
    {
        string json = JsonSerializer.Serialize<ShapeDto>(dto, JsonOptions.Default);
        Option<ShapeDto> copy = JsonSerializer.Deserialize<ShapeDto>(json, JsonOptions.Default).ToOption();

        Assert.Equal(Some(dto), copy);
    }


    [Fact]
    public void Serialize_WhenGivenCase_WritesKindAndValue()
    {
        ShapeDto dto = new ShapeDto.CircleCase(new CircleDto { Radius = 2 });

        string json = JsonSerializer.Serialize<ShapeDto>(dto, JsonOptions.Default);

        Assert.Equal("""{"kind":"circle","value":{"radius":2}}""", json);
    }


    [Theory]
    [InlineData("""{"kind":"circle"}""")]                 // missing value
    [InlineData("""{"kind":"circle","value":null}""")]    // null value
    [InlineData("""{"kind":"hexagon","value":{}}""")]     // unknown kind
    public void Deserialize_WhenInputIsMalformed_Throws(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<ShapeDto>(json, JsonOptions.Default));
    }


    [Fact]
    public void Deserialize_WhenKindIsNotFirst_Succeeds()
    {
        Option<ShapeDto> dto = JsonSerializer.Deserialize<ShapeDto>(
            """{"value":{"radius":2},"kind":"circle"}""", JsonOptions.Default).ToOption();

        Assert.Equal(Some<ShapeDto>(new ShapeDto.CircleCase(new CircleDto { Radius = 2 })), dto);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FromDomain / ToDomain ────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    // Driven by the domain union's samples, so a Shape case with no matching
    // mapping arm fails here with UnreachableException.
    [Theory]
    [MemberData(nameof(ShapeTests.Samples), MemberType = typeof(ShapeTests))]
    public void FromDomain_WhenSerializedAndMappedBack_RoundTrips(Shape shape)
    {
        string json = JsonSerializer.Serialize<ShapeDto>(ShapeDtoExt.FromDomain(shape), JsonOptions.Default);
        ShapeDto dto = JsonSerializer.Deserialize<ShapeDto>(json, JsonOptions.Default).ToOption().ThrowIfNone();

        Assert.Equal(shape, dto.ToDomain().GetValue());
    }


    [Fact]
    public void FromDomain_WhenGivenCircle_MapsToCircleCase()
    {
        Shape shape = new Shape.CircleCase(new Circle { Radius = 2 });

        ShapeDto dto = ShapeDtoExt.FromDomain(shape);

        Assert.Equal(new ShapeDto.CircleCase(new CircleDto { Radius = 2 }), dto);
    }


    [Fact]
    public void ToDomain_WhenGivenRectCase_MapsToRectCase()
    {
        ShapeDto dto = new ShapeDto.RectCase(new RectDto { Width = 3, Height = 4 });

        Result<Shape, string> shape = dto.ToDomain();

        Assert.Equal(new Shape.RectCase(new Rect { Width = 3, Height = 4 }), shape.GetValue());
    }
}
