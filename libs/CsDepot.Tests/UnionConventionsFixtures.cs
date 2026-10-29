//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json.Serialization;

namespace CsDepot.Tests;


//
// Fixtures for UnionConventions.spec.cs.  ValidUnion (a domain union) and
// ValidUnionDto (its DTO union) are the only top-level unions in this assembly.
// The unions in BadUnions each break one convention; they are nested, so
// UnionConventions.FindUnions() does not find them.
//

public sealed record class Circle
{
    public required double Radius { get; init; }
}


public sealed record class Square
{
    public required double Side { get; init; }
}


public sealed record class CircleDto
{
    public required double Radius { get; init; }
}


public sealed record class SquareDto
{
    public required double Side { get; init; }
}


public abstract record class ValidUnion
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private ValidUnion() { }

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class CircleCase(Circle Value) : ValidUnion;


    public sealed record class SquareCase(Square Value) : ValidUnion;
}


[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(ValidUnionDto.CircleCase), "circle")]
[JsonDerivedType(typeof(ValidUnionDto.SquareCase), "square")]
public abstract record class ValidUnionDto
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private ValidUnionDto() { }

    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    public sealed record class CircleCase(CircleDto Value) : ValidUnionDto;


    public sealed record class SquareCase(SquareDto Value) : ValidUnionDto;
}


public static class BadUnions
{
    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    //
    // Domain unions
    //

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(DomainWithJson.CircleCase), "circle")]
    public abstract record class DomainWithJson
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private DomainWithJson() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : DomainWithJson;
    }


    public abstract record class DomainWithDtoPayload
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private DomainWithDtoPayload() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(CircleDto Value) : DomainWithDtoPayload;
    }


    public abstract record class ProtectedConstructor
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        protected ProtectedConstructor() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : ProtectedConstructor;
    }


    public abstract record class ExtraProperty
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private ExtraProperty() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value, string Color) : ExtraProperty;
    }


    public abstract record class MisnamedCase
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private MisnamedCase() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class RoundCase(Circle Value) : MisnamedCase;
    }

    //
    // DTO unions
    //

    [JsonDerivedType(typeof(MissingPolymorphicDto.CircleCase), "circle")]
    public abstract record class MissingPolymorphicDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private MissingPolymorphicDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(CircleDto Value) : MissingPolymorphicDto;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(WrongDiscriminatorDto.CircleCase), "circle")]
    public abstract record class WrongDiscriminatorDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private WrongDiscriminatorDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(CircleDto Value) : WrongDiscriminatorDto;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(UnregisteredCaseDto.CircleCase), "circle")]
    public abstract record class UnregisteredCaseDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private UnregisteredCaseDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(CircleDto Value) : UnregisteredCaseDto;


        public sealed record class SquareCase(SquareDto Value) : UnregisteredCaseDto;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(StaleRegistrationDto.CircleCase), "circle")]
    [JsonDerivedType(typeof(ValidUnionDto.SquareCase), "square")]
    public abstract record class StaleRegistrationDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private StaleRegistrationDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(CircleDto Value) : StaleRegistrationDto;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(DuplicateKindDto.CircleCase), "circle")]
    [JsonDerivedType(typeof(DuplicateKindDto.SquareCase), "circle")]
    public abstract record class DuplicateKindDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private DuplicateKindDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(CircleDto Value) : DuplicateKindDto;


        public sealed record class SquareCase(SquareDto Value) : DuplicateKindDto;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(DomainPayloadDto.CircleCase), "circle")]
    public abstract record class DomainPayloadDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private DomainPayloadDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : DomainPayloadDto;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(MisnamedCaseDto.CircleDtoCase), "circle")]
    public abstract record class MisnamedCaseDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private MisnamedCaseDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleDtoCase(CircleDto Value) : MisnamedCaseDto;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(WrongKindDto.CircleCase), "round")]
    public abstract record class WrongKindDto
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private WrongKindDto() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(CircleDto Value) : WrongKindDto;
    }
}
