//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json.Serialization;

namespace CsDepot.Tests;


//
// Fixtures for UnionConventions.spec.cs.  ValidUnion is the only top-level union
// in this assembly.  The unions in BadUnions each break one convention; they are
// nested, so UnionConventions.FindUnions() does not find them.
//

public sealed record class Circle
{
    public required double Radius { get; init; }
}


public sealed record class Square
{
    public required double Side { get; init; }
}


[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(ValidUnion.CircleCase), "circle")]
[JsonDerivedType(typeof(ValidUnion.SquareCase), "square")]
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


public static class BadUnions
{
    //------------------------------------------------------------------------------
    // Nested types
    //------------------------------------------------------------------------------

    [JsonDerivedType(typeof(MissingPolymorphic.CircleCase), "circle")]
    public abstract record class MissingPolymorphic
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private MissingPolymorphic() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : MissingPolymorphic;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(WrongDiscriminator.CircleCase), "circle")]
    public abstract record class WrongDiscriminator
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private WrongDiscriminator() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : WrongDiscriminator;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(ProtectedConstructor.CircleCase), "circle")]
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


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(UnregisteredCase.CircleCase), "circle")]
    public abstract record class UnregisteredCase
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private UnregisteredCase() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : UnregisteredCase;


        public sealed record class SquareCase(Square Value) : UnregisteredCase;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(StaleRegistration.CircleCase), "circle")]
    [JsonDerivedType(typeof(ValidUnion.SquareCase), "square")]
    public abstract record class StaleRegistration
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private StaleRegistration() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : StaleRegistration;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(DuplicateKind.CircleCase), "circle")]
    [JsonDerivedType(typeof(DuplicateKind.SquareCase), "circle")]
    public abstract record class DuplicateKind
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private DuplicateKind() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : DuplicateKind;


        public sealed record class SquareCase(Square Value) : DuplicateKind;
    }


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(ExtraProperty.CircleCase), "circle")]
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


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(MisnamedCase.RoundCase), "circle")]
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


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(WrongKind.CircleCase), "round")]
    public abstract record class WrongKind
    {
        //------------------------------------------------------------------------------
        // Constructors
        //------------------------------------------------------------------------------

        private WrongKind() { }

        //------------------------------------------------------------------------------
        // Nested types
        //------------------------------------------------------------------------------

        public sealed record class CircleCase(Circle Value) : WrongKind;
    }
}
