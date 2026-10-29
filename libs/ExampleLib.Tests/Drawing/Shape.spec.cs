//
// At least a portion of the code below was created using AI tool Claude Code.
//

using ExampleLib.Drawing;

namespace ExampleLib.Tests.Drawing;


public class ShapeTests
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    // One sample per case.  Add a sample whenever a case is added.  ShapeDtoTests
    // also uses these samples to check that every case maps to and from its DTO.
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
