//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Reflection;
using System.Text.Json.Serialization;
using FnUtil;

namespace CsDepot;


/// <summary>
/// Checks that discriminated unions follow the conventions in
/// docs/csharp_data_modeling.md.  [JsonDerivedType] is only metadata that the
/// compiler does not check against a union's nested cases, so every assembly
/// that defines unions runs these checks from a unit test.
/// </summary>
/// <remarks>
/// A union whose name ends in "Dto" is a DTO union.  It is serialized, so it
/// must carry the JSON attributes, and its cases must wrap DTOs.  Any other
/// union is a domain union.  Domain unions are never serialized, so they must
/// not carry JSON attributes, and their cases must not wrap DTOs.
/// </remarks>
public static class UnionConventions
{
    //------------------------------------------------------------------------------
    // Constants
    //------------------------------------------------------------------------------

    private const BindingFlags AllInstanceMembers =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private const BindingFlags AllNestedTypes = BindingFlags.Public | BindingFlags.NonPublic;

    private const BindingFlags DeclaredPublicProperties =
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private const string DtoSuffix = "Dto";

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Finds the unions defined in an assembly.  Unions are found by their shape
    /// (a top-level abstract class with nested types derived from it), not by
    /// attribute, so that a DTO union missing its [JsonPolymorphic] attribute is
    /// still found.
    /// </summary>
    public static IReadOnlyList<Type> FindUnions(Assembly assembly) =>
        assembly.GetTypes()
        .Where((type) => type.DeclaringType is null && IsUnionShaped(type))
        .ToList();


    /// <summary>
    /// Checks every union in an assembly.  Returns a description of each
    /// violation found, or an empty list when all unions follow the conventions.
    /// </summary>
    public static IReadOnlyList<string> FindAllViolations(Assembly assembly) =>
        FindUnions(assembly).SelectMany(FindViolations).ToList();


    /// <summary>
    /// Checks one union.  Returns a description of each violation found, or an
    /// empty list when the union follows the conventions.
    /// </summary>
    public static IReadOnlyList<string> FindViolations(Type union) =>
        IsDto(union) ? DtoUnionViolations(union) : DomainUnionViolations(union);


    private static List<string> DtoUnionViolations(Type union)
    {
        List<Type> cases = NestedCases(union);
        List<JsonDerivedTypeAttribute> registrations =
            [.. union.GetCustomAttributes<JsonDerivedTypeAttribute>()];

        return
        [
            .. DiscriminatorViolations(union),
            .. ConstructorViolations(union),
            .. RegistrationViolations(union, cases, registrations),
            .. KindViolations(union, registrations),
            .. cases.SelectMany((caseType) => DtoCaseViolations(union, caseType, registrations)),
        ];
    }


    private static List<string> DomainUnionViolations(Type union) =>
    [
        .. NoJsonAttributeViolations(union),
        .. ConstructorViolations(union),
        .. NestedCases(union).SelectMany((caseType) => DomainCaseViolations(union, caseType)),
    ];


    private static bool IsUnionShaped(Type type) =>
        type.IsClass && type.IsAbstract && !type.IsSealed && NestedCases(type).Count > 0;


    private static List<Type> NestedCases(Type union) =>
        [.. union.GetNestedTypes(AllNestedTypes).Where((nested) => nested.IsSubclassOf(union))];


    private static bool IsDto(Type type) => type.Name.EndsWith(DtoSuffix, StringComparison.Ordinal);


    // The name of the concept a type models.  "CircleDto" and "Circle" are both
    // "Circle".
    private static string ConceptName(Type type) =>
        IsDto(type) ? type.Name[..^DtoSuffix.Length] : type.Name;


    // A DTO union is configured for polymorphic JSON with the "kind" discriminator.
    private static IEnumerable<string> DiscriminatorViolations(Type union)
    {
        if (union.GetCustomAttribute<JsonPolymorphicAttribute>() is not { } polymorphic)
        {
            yield return $"{union.Name} has no [JsonPolymorphic] attribute.";
        }
        else if (polymorphic.TypeDiscriminatorPropertyName != "kind")
        {
            yield return $"{union.Name} must use \"kind\" as its type discriminator property.";
        }
    }


    // A domain union is never serialized, so it has no JSON attributes.  Its DTO
    // union carries them instead.
    private static IEnumerable<string> NoJsonAttributeViolations(Type union)
    {
        if (union.IsDefined(typeof(JsonPolymorphicAttribute)) ||
            union.IsDefined(typeof(JsonDerivedTypeAttribute)))
        {
            yield return
                $"{union.Name} is a domain union and must not have JSON attributes. " +
                $"Serialize it through a DTO union named {union.Name}{DtoSuffix}.";
        }
    }


    // The union is closed: its parameterless constructor is private, so only
    // nested types can derive from it.
    private static IEnumerable<string> ConstructorViolations(Type union)
    {
        Option<ConstructorInfo> parameterless = union
            .GetConstructor(AllInstanceMembers, binder: null, Type.EmptyTypes, modifiers: null)
            .ToOption();

        if (!parameterless.Exists((constructor) => constructor.IsPrivate))
        {
            yield return $"{union.Name} must have a private parameterless constructor.";
        }
    }


    // The registered [JsonDerivedType] entries exactly match the nested cases.
    // This catches a case with no registration (the common mistake) and a
    // registration that points at a removed case.
    private static IEnumerable<string>
    RegistrationViolations(
        Type union,
        List<Type> cases,
        List<JsonDerivedTypeAttribute> registrations
    )
    {
        HashSet<Type> registered = [.. registrations.Select((registration) => registration.DerivedType)];

        foreach (Type caseType in cases.Where((caseType) => !registered.Contains(caseType)))
        {
            yield return $"{union.Name}.{caseType.Name} has no [JsonDerivedType] registration.";
        }

        foreach (Type derived in registered.Where((derived) => !cases.Contains(derived)))
        {
            yield return $"{union.Name} registers {derived.Name}, which is not one of its nested cases.";
        }
    }


    // Every registration has a unique, non-blank string "kind".
    private static IEnumerable<string>
    KindViolations(Type union, List<JsonDerivedTypeAttribute> registrations)
    {
        if (registrations.Any(HasNoKind))
        {
            yield return $"{union.Name} has a [JsonDerivedType] registration without a string kind.";
        }

        IEnumerable<string> duplicates = registrations
            .Select((registration) => registration.TypeDiscriminator)
            .OfType<string>()
            .GroupBy((kind) => kind)
            .Where((group) => group.Count() > 1)
            .Select((group) => group.Key);

        foreach (string duplicate in duplicates)
        {
            yield return $"{union.Name} registers the kind \"{duplicate}\" more than once.";
        }
    }


    private static bool HasNoKind(JsonDerivedTypeAttribute registration) =>
        registration.TypeDiscriminator is not string kind || string.IsNullOrWhiteSpace(kind);


    // A DTO union case follows the shared case rules, wraps a DTO, and is
    // registered with the lowerCamelCase name of its concept.  A case wrapping
    // CircleDto is registered as "circle".
    private static IEnumerable<string>
    DtoCaseViolations(Type union, Type caseType, List<JsonDerivedTypeAttribute> registrations)
    {
        string name = $"{union.Name}.{caseType.Name}";

        foreach (string violation in SharedCaseViolations(name, caseType))
        {
            yield return violation;
        }

        Option<PropertyInfo> value = CaseValue(caseType);
        if (value.IsNone)
        {
            yield break;
        }

        Type valueType = value.GetValue().PropertyType;

        if (!IsDto(valueType))
        {
            yield return $"{name} wraps {valueType.Name}, which is not a DTO. A DTO union's cases wrap DTOs.";
        }

        string conceptName = ConceptName(valueType);
        string expectedKind = char.ToLowerInvariant(conceptName[0]) + conceptName[1..];
        IEnumerable<string> wrongKinds = registrations
            .Where((registration) => registration.DerivedType == caseType)
            .Select((registration) => registration.TypeDiscriminator)
            .OfType<string>()
            .Where((kind) => kind != expectedKind);

        foreach (string kind in wrongKinds)
        {
            yield return $"{name} must be registered with the kind \"{expectedKind}\", not \"{kind}\".";
        }
    }


    // A domain union case follows the shared case rules and wraps a domain type.
    private static IEnumerable<string> DomainCaseViolations(Type union, Type caseType)
    {
        string name = $"{union.Name}.{caseType.Name}";

        foreach (string violation in SharedCaseViolations(name, caseType))
        {
            yield return violation;
        }

        if (CaseValue(caseType).Exists((value) => IsDto(value.PropertyType)))
        {
            Type valueType = CaseValue(caseType).GetValue().PropertyType;
            yield return $"{name} wraps {valueType.Name}, which is a DTO. A domain union's cases wrap domain types.";
        }
    }


    // Every case is a public sealed nested record with exactly one property,
    // named Value.  Its name is {Concept}Case, where Concept is the wrapped
    // type's name without any "Dto" suffix.
    private static IEnumerable<string> SharedCaseViolations(string name, Type caseType)
    {
        if (!caseType.IsNestedPublic || !caseType.IsSealed)
        {
            yield return $"{name} must be a public sealed nested type.";
        }

        Option<PropertyInfo> value = CaseValue(caseType);
        if (value.IsNone)
        {
            yield return $"{name} must have exactly one property, named Value.";
            yield break;
        }

        string expectedName = ConceptName(value.GetValue().PropertyType) + "Case";

        if (caseType.Name != expectedName)
        {
            yield return $"{name} must be named {expectedName}.";
        }
    }


    // The case's single property, when it has exactly one and it is named Value.
    private static Option<PropertyInfo> CaseValue(Type caseType) =>
        caseType.GetProperties(DeclaredPublicProperties) is [{ Name: "Value" } value] ?
            F.Some(value) :
            F.none;
}
