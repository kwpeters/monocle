//
// At least a portion of the code below was created using AI tool Claude Code.
//

using FnUtil;
using static FnUtil.F;

namespace ExampleLib.Drawing;


public sealed record class RectDto
{
    public required double Width { get; init; }
    public required double Height { get; init; }
}


public static class RectDtoExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static RectDto FromDomain(Rect rect) => new() { Width = rect.Width, Height = rect.Height };


    public static Result<Rect, string> ToDomain(this RectDto dto) =>
        Success(new Rect { Width = dto.Width, Height = dto.Height });
}
