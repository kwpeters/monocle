//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ExperimentalLib.Drawing;


//
// A union: a closed set of cases.  The private constructor means only nested
// types can derive from it.  Each case wraps exactly one domain type in a
// property named Value, and is registered with a "kind" that is the
// lowerCamelCase name of that domain type.
//
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(Shape.CircleCase), "circle")]
[JsonDerivedType(typeof(Shape.RectCase), "rect")]
public abstract record class Shape
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private Shape() { }

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class CircleCase(Circle Value) : Shape;


    public sealed record class RectCase(Rect Value) : Shape;
}


//
// Behavior that spans a union's cases is a switch expression in the union's Ext
// class.  The compiler does not check that the switch is exhaustive, so it ends
// with an arm that throws UnreachableException.
//
public static class ShapeExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static string Describe(this Shape shape) => shape switch
    {
        Shape.CircleCase({ Radius: double r }) => $"Circle, radius {r}",
        Shape.RectCase({ Width: double w, Height: double h }) => $"{w} x {h} rectangle",
        _ => throw new UnreachableException()
    };
}
