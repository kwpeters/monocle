//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Diagnostics;

namespace ExampleLib.Drawing;


//
// A domain union: a closed set of cases.  The private constructor means only
// nested types can derive from it.  Each case wraps exactly one domain type in a
// property named Value.
//
// Domain unions are never serialized, so they carry no JSON attributes.
// ShapeDto is its JSON shape.
//
public abstract record class Shape
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private Shape() { }

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    // Behavior that spans the cases is a switch expression on the union.  The
    // compiler does not check that the switch is exhaustive, so it ends with an
    // arm that throws UnreachableException.
    public string Describe() => this switch {
        Shape.CircleCase({ Radius: double r }) => $"Circle, radius {r}",
        Shape.RectCase({ Width: double w, Height: double h }) => $"{w} x {h} rectangle",
        _ => throw new UnreachableException()
    };

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class CircleCase(Circle Value) : Shape;


    public sealed record class RectCase(Rect Value) : Shape;
}
