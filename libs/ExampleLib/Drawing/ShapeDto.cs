//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Diagnostics;
using System.Text.Json.Serialization;
using FnUtil;

namespace ExampleLib.Drawing;


//
// A DTO union: the JSON shape of the Shape domain union.  It mirrors Shape case
// for case.  Each case wraps the DTO of the domain type that the matching Shape
// case wraps, and is registered with a "kind" that is the lowerCamelCase name of
// that concept, without the Dto suffix ("circle", not "circleDto").
//
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(ShapeDto.CircleCase), "circle")]
[JsonDerivedType(typeof(ShapeDto.RectCase), "rect")]
public abstract record class ShapeDto
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private ShapeDto() { }

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class CircleCase(CircleDto Value) : ShapeDto;


    public sealed record class RectCase(RectDto Value) : ShapeDto;
}


//
// Mapping between the DTO union and the domain union.  The compiler does not
// check that either switch covers every case, so each ends with an arm that
// throws UnreachableException.
//
public static class ShapeDtoExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static ShapeDto FromDomain(Shape shape) => shape switch {
        Shape.CircleCase(Circle circle) => new ShapeDto.CircleCase(CircleDtoExt.FromDomain(circle)),
        Shape.RectCase(Rect rect) => new ShapeDto.RectCase(RectDtoExt.FromDomain(rect)),
        _ => throw new UnreachableException()
    };


    public static Result<Shape, string>
    ToDomain(
        this ShapeDto dto
    ) =>
        dto switch {
            ShapeDto.CircleCase(CircleDto circleDto) =>
                circleDto.ToDomain().MapSuccess(Shape (Circle circle) => new Shape.CircleCase(circle)),
            ShapeDto.RectCase(RectDto rectDto) =>
                rectDto.ToDomain().MapSuccess(Shape (Rect rect) => new Shape.RectCase(rect)),
            _ => throw new UnreachableException()
        };
}
