//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;
using CsDepot;
using ExampleLib.Drawing;

namespace ExampleLib.Tests.Drawing;


public class DrawableTests
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    // One sample per case.  Add a sample whenever a case is added.
    public static TheoryData<Drawable> Samples() =>
        new(
            new Drawable.CircleCase(new Circle { Radius = 2 }),
            new Drawable.LabelCase(new Label { Content = "Hello" })
        );

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(Samples))]
    public void Serialize_WhenDeserialized_RoundTrips(Drawable drawable)
    {
        string json = JsonSerializer.Serialize<Drawable>(drawable, JsonOptions.Default);
        Option<Drawable> copy = JsonSerializer.Deserialize<Drawable>(json, JsonOptions.Default).ToOption();

        Assert.Equal(Some(drawable), copy);
    }


    [Theory]
    [InlineData("""{"kind":"label"}""")]                  // missing value
    [InlineData("""{"kind":"label","value":null}""")]     // null value
    [InlineData("""{"kind":"hexagon","value":{}}""")]     // unknown kind
    public void Deserialize_WhenInputIsMalformed_Throws(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<Drawable>(json, JsonOptions.Default));
    }
}
