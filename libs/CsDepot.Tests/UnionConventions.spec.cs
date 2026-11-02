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
    public void FindUnions_WhenAssemblyHasTopLevelUnions_ReturnsDomainAndDtoUnions()
    {
        IReadOnlyList<Type> unions = UnionConventions.FindUnions(typeof(ValidUnion).Assembly);

        Assert.Equal([typeof(ValidUnion), typeof(ValidUnionDto)], unions.OrderBy((union) => union.Name));
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

    [Theory]
    [InlineData(typeof(ValidUnion))]
    [InlineData(typeof(ValidUnionDto))]
    public void FindViolations_WhenUnionFollowsConventions_ReturnsEmpty(Type union)
    {
        IReadOnlyList<string> violations = UnionConventions.FindViolations(union);

        Assert.Empty(violations);
    }


    [Theory]
    // Domain unions
    [InlineData(typeof(BadUnions.DomainWithJson), "DomainWithJson is a domain union and must not have JSON attributes. Serialize it through a DTO union named DomainWithJsonDto.")]
    [InlineData(typeof(BadUnions.DomainWithDtoPayload), "DomainWithDtoPayload.CircleCase wraps CircleDto, which is a DTO. A domain union's cases wrap domain types.")]
    [InlineData(typeof(BadUnions.ProtectedConstructor), "ProtectedConstructor must have a private parameterless constructor.")]
    [InlineData(typeof(BadUnions.ExtraProperty), "ExtraProperty.CircleCase must have exactly one property, named Value.")]
    [InlineData(typeof(BadUnions.MisnamedCase), "MisnamedCase.RoundCase must be named CircleCase.")]
    // DTO unions
    [InlineData(typeof(BadUnions.MissingPolymorphicDto), "MissingPolymorphicDto has no [JsonPolymorphic] attribute.")]
    [InlineData(typeof(BadUnions.WrongDiscriminatorDto), "WrongDiscriminatorDto must use \"kind\" as its type discriminator property.")]
    [InlineData(typeof(BadUnions.UnregisteredCaseDto), "UnregisteredCaseDto.SquareCase has no [JsonDerivedType] registration.")]
    [InlineData(typeof(BadUnions.StaleRegistrationDto), "StaleRegistrationDto registers SquareCase, which is not one of its nested cases.")]
    [InlineData(typeof(BadUnions.DomainPayloadDto), "DomainPayloadDto.CircleCase wraps Circle, which is not a DTO. A DTO union's cases wrap DTOs.")]
    [InlineData(typeof(BadUnions.MisnamedCaseDto), "MisnamedCaseDto.CircleDtoCase must be named CircleCase.")]
    [InlineData(typeof(BadUnions.WrongKindDto), "WrongKindDto.CircleCase must be registered with the kind \"circle\", not \"round\".")]
    public void
    FindViolations_WhenUnionBreaksOneConvention_ReturnsOnlyThatViolation(
        Type union,
        string expected
    )
    {
        IReadOnlyList<string> violations = UnionConventions.FindViolations(union);

        Assert.Equal(expected, Assert.Single(violations));
    }


    [Fact]
    public void FindViolations_WhenKindIsRegisteredTwice_ReportsTheDuplicate()
    {
        IReadOnlyList<string> violations = UnionConventions.FindViolations(typeof(BadUnions.DuplicateKindDto));

        Assert.Contains("DuplicateKindDto registers the kind \"circle\" more than once.", violations);
    }
}
