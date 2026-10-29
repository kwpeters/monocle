//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace ExampleLib.Drawing;


//
// A second domain union that also contains Circle.  Neither union knows about
// the other.
//
public abstract record class Drawable
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private Drawable() { }

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class CircleCase(Circle Value) : Drawable;


    public sealed record class LabelCase(Label Value) : Drawable;
}
