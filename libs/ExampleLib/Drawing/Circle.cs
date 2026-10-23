//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace ExampleLib.Drawing;


//
// A domain type.  It knows nothing about the unions that contain it, so it can
// appear in any number of them (see Shape and Drawable).
//
public sealed record class Circle
{
    public required double Radius { get; init; }
}


public static class CircleExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    // A computed value is an operation, not a property on the record, so that it
    // is not serialized into JSON.
    public static double Area(this Circle circle) => Math.PI * circle.Radius * circle.Radius;
}
