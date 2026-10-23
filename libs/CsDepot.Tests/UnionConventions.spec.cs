//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace CsDepot.Tests;


public class UnionConventionsTests
{
    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FindUnions ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void FindUnions_WhenAssemblyHasTopLevelUnion_ReturnsIt()
    {
        IReadOnlyList<Type> unions = UnionConventions.FindUnions(typeof(ValidUnion).Assembly);

        Assert.Equal(typeof(ValidUnion), Assert.Single(unions));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FindAllViolations ────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void FindAllViolations_WhenAllUnionsFollowConventions_ReturnsEmpty()
    {
        IReadOnlyList<string> violations = UnionConventions.FindAllViolations(typeof(ValidUnion).Assembly);

        Assert.Empty(violations);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FindViolations ───────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void FindViolations_WhenUnionFollowsConventions_ReturnsEmpty()
    {
        IReadOnlyList<string> violations = UnionConventions.FindViolations(typeof(ValidUnion));

        Assert.Empty(violations);
    }


    [Theory]
    [InlineData(typeof(BadUnions.MissingPolymorphic), "MissingPolymorphic has no [JsonPolymorphic] attribute.")]
    [InlineData(typeof(BadUnions.WrongDiscriminator), "WrongDiscriminator must use \"kind\" as its type discriminator property.")]
    [InlineData(typeof(BadUnions.ProtectedConstructor), "ProtectedConstructor must have a private parameterless constructor.")]
    [InlineData(typeof(BadUnions.UnregisteredCase), "UnregisteredCase.SquareCase has no [JsonDerivedType] registration.")]
    [InlineData(typeof(BadUnions.StaleRegistration), "StaleRegistration registers SquareCase, which is not one of its nested cases.")]
    [InlineData(typeof(BadUnions.ExtraProperty), "ExtraProperty.CircleCase must have exactly one property, named Value.")]
    [InlineData(typeof(BadUnions.MisnamedCase), "MisnamedCase.RoundCase must be named CircleCase.")]
    [InlineData(typeof(BadUnions.WrongKind), "WrongKind.CircleCase must be registered with the kind \"circle\", not \"round\".")]
    public void FindViolations_WhenUnionBreaksOneConvention_ReturnsOnlyThatViolation(Type union, string expected)
    {
        IReadOnlyList<string> violations = UnionConventions.FindViolations(union);

        Assert.Equal(expected, Assert.Single(violations));
    }


    [Fact]
    public void FindViolations_WhenKindIsRegisteredTwice_ReportsTheDuplicate()
    {
        IReadOnlyList<string> violations = UnionConventions.FindViolations(typeof(BadUnions.DuplicateKind));

        Assert.Contains("DuplicateKind registers the kind \"circle\" more than once.", violations);
    }
}
