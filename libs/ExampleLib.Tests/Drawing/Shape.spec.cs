//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;
using CsDepot;
using ExampleLib.Drawing;

namespace ExampleLib.Tests.Drawing;


public class ShapeTests
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    // One sample per case.  Add a sample whenever a case is added.
    public static TheoryData<Shape> Samples() =>
        new(
            new Shape.CircleCase(new Circle { Radius = 2 }),
            new Shape.RectCase(new Rect { Width = 3, Height = 4 })
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
    public void Serialize_WhenDeserialized_RoundTrips(Shape shape)
    {
        string json = JsonSerializer.Serialize<Shape>(shape, JsonOptions.Default);
        Option<Shape> copy = JsonSerializer.Deserialize<Shape>(json, JsonOptions.Default).ToOption();

        Assert.Equal(Some(shape), copy);
    }


    [Fact]
    public void Serialize_WhenGivenCase_WritesKindAndValue()
    {
        Shape shape = new Shape.CircleCase(new Circle { Radius = 2 });

        string json = JsonSerializer.Serialize<Shape>(shape, JsonOptions.Default);

        Assert.Equal("""{"kind":"circle","value":{"radius":2}}""", json);
    }


    [Theory]
    [InlineData("""{"kind":"circle"}""")]                 // missing value
    [InlineData("""{"kind":"circle","value":null}""")]    // null value
    [InlineData("""{"kind":"hexagon","value":{}}""")]     // unknown kind
    public void Deserialize_WhenInputIsMalformed_Throws(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<Shape>(json, JsonOptions.Default));
    }


    [Fact]
    public void Deserialize_WhenKindIsNotFirst_Succeeds()
    {
        Option<Shape> shape = JsonSerializer.Deserialize<Shape>(
            """{"value":{"radius":2},"kind":"circle"}""", JsonOptions.Default).ToOption();

        Assert.Equal(Some<Shape>(new Shape.CircleCase(new Circle { Radius = 2 })), shape);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Describe ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Describe_WhenGivenCircle_DescribesRadius()
    {
        Shape shape = new Shape.CircleCase(new Circle { Radius = 2 });

        Assert.Equal("Circle, radius 2", shape.Describe());
    }


    [Fact]
    public void Describe_WhenGivenRect_DescribesDimensions()
    {
        Shape shape = new Shape.RectCase(new Rect { Width = 3, Height = 4 });

        Assert.Equal("3 x 4 rectangle", shape.Describe());
    }
}
