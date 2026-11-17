//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Diagnostics;
using System.Text.Json.Serialization;
using FnUtil;

namespace ExampleLib.Drawing;


//
// The JSON shape of the Drawable domain union.  CircleDto is registered with
// the same "circle" kind here as in ShapeDto.
//
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(DrawableDto.CircleCase), "circle")]
[JsonDerivedType(typeof(DrawableDto.LabelCase), "label")]
public abstract record class DrawableDto
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private DrawableDto() { }

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class CircleCase(CircleDto Value) : DrawableDto;


    public sealed record class LabelCase(LabelDto Value) : DrawableDto;
}


public static class DrawableDtoExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static DrawableDto
    FromDomain(
        Drawable drawable
    ) =>
        drawable switch {
            Drawable.CircleCase(Circle circle) => new DrawableDto.CircleCase(CircleDtoExt.FromDomain(circle)),
            Drawable.LabelCase(Label label) => new DrawableDto.LabelCase(LabelDtoExt.FromDomain(label)),
            _ => throw new UnreachableException()
        };


    public static Result<Drawable, string>
    ToDomain(
        this DrawableDto dto
    ) =>
        dto switch {
            DrawableDto.CircleCase(CircleDto circleDto) =>
                circleDto.ToDomain().MapSuccess(Drawable (Circle circle) => new Drawable.CircleCase(circle)),
            DrawableDto.LabelCase(LabelDto labelDto) =>
                labelDto.ToDomain().MapSuccess(Drawable (Label label) => new Drawable.LabelCase(label)),
            _ => throw new UnreachableException()
        };
}
