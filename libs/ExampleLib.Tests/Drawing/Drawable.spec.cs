//
// At least a portion of the code below was created using AI tool Claude Code.
//

using ExampleLib.Drawing;

namespace ExampleLib.Tests.Drawing;


public static class DrawableTests
{
    // One sample per case.  Add a sample whenever a case is added.  Drawable has
    // no behavior of its own to test; DrawableDtoTests uses these samples to
    // check that every case maps to and from its DTO.
    public static TheoryData<Drawable>
    Samples() =>
        new(
            new Drawable.CircleCase(new Circle { Radius = 2 }),
            new Drawable.LabelCase(new Label { Content = "Hello" })
        );
}
