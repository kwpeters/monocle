//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json.Serialization;

namespace ExperimentalLib.Drawing;


//
// A second union that also contains Circle.  Neither union knows about the
// other, and Circle is registered with the same "circle" kind in both.
//
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(Drawable.CircleCase), "circle")]
[JsonDerivedType(typeof(Drawable.LabelCase), "label")]
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
