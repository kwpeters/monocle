//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace ExampleLib.Drawing;


public sealed record class Rect
{
    public required double Width { get; init; }
    public required double Height { get; init; }

    public double Area => Width * Height;
}
