//
// At least a portion of the code below was created using AI tool Claude Code.
//

using FnUtil;
using static FnUtil.F;

namespace ExampleLib.Drawing;


public sealed record class LabelDto
{
    public required string Content { get; init; }
}


public static class LabelDtoExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static LabelDto FromDomain(Label label) => new() { Content = label.Content };


    public static Result<Label, string> ToDomain(this LabelDto dto) =>
        Success(new Label { Content = dto.Content });
}
