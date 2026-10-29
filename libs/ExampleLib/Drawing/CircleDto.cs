//
// At least a portion of the code below was created using AI tool Claude Code.
//

using FnUtil;
using static FnUtil.F;

namespace ExampleLib.Drawing;


//
// The JSON shape of a Circle.  Data only.
//
public sealed record class CircleDto
{
    public required double Radius { get; init; }
}


public static class CircleDtoExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static CircleDto FromDomain(Circle circle) => new() { Radius = circle.Radius };


    // Circle has no invariants, so this cannot fail today.  It still returns a
    // Result, so adding an invariant later does not change its signature.
    public static Result<Circle, string> ToDomain(this CircleDto dto) =>
        Success(new Circle { Radius = dto.Radius });
}
