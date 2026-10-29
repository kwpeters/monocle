//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace ExampleLib.Drawing;


//
// A domain type with no invariants, so it is publicly constructible with
// required init properties.  It knows nothing about the unions that contain it,
// so it can appear in any number of them (see Shape and Drawable).
//
public sealed record class Circle
{
    public required double Radius { get; init; }

    // A cheap derived value may be a computed property, because domain types
    // are never serialized.
    public double Area => Math.PI * Radius * Radius;
}
