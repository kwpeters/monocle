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

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Finds the unions defined in an assembly.  Unions are found by their shape
    /// (a top-level abstract class with nested types derived from it), not by
    /// attribute, so that a union missing its [JsonPolymorphic] attribute is
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
    public static IReadOnlyList<string> FindViolations(Type union)
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
            .. cases.SelectMany((caseType) => CaseViolations(union, caseType, registrations)),
        ];
    }


    private static bool IsUnionShaped(Type type) =>
        type.IsClass && type.IsAbstract && !type.IsSealed && NestedCases(type).Count > 0;


    private static List<Type> NestedCases(Type union) =>
        [.. union.GetNestedTypes(AllNestedTypes).Where((nested) => nested.IsSubclassOf(union))];


    // The union is configured for polymorphic JSON with the "kind" discriminator.
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


    // A case is a public sealed nested record with exactly one property, named
    // Value.  Its name is {DomainType}Case, and its kind is the lowerCamelCase
    // name of the domain type.
    private static IEnumerable<string>
    CaseViolations(Type union, Type caseType, List<JsonDerivedTypeAttribute> registrations)
    {
        string name = $"{union.Name}.{caseType.Name}";

        if (!caseType.IsNestedPublic || !caseType.IsSealed)
        {
            yield return $"{name} must be a public sealed nested type.";
        }

        PropertyInfo[] properties = caseType.GetProperties(DeclaredPublicProperties);

        if (properties is not [{ Name: "Value" } value])
        {
            yield return $"{name} must have exactly one property, named Value.";
            yield break;
        }

        string domainName = value.PropertyType.Name;

        if (caseType.Name != domainName + "Case")
        {
            yield return $"{name} must be named {domainName}Case.";
        }

        string expectedKind = char.ToLowerInvariant(domainName[0]) + domainName[1..];
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
}
