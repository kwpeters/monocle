using System.Text;
using CsDepot;
using FnUtil;
using static FnUtil.F;

namespace ExampleLib;

//
// Problems with record struct
// - Default can be used to create an instance with uninitialized properties
// - When used as the element type of an array, it can lead to unexpected default values
// - Mutable by default.  You must remember `readonly record struct`.
//
// public readonly record struct PersonStruct
// {
//     public required string FirstName { get; init; }
//     public required string LastName { get; init; }
// }


////////////////////////////////////////////////////////////////////////////////


//
// A domain type with invariants: an immutable record whose operations live on
// the type.
//
// - The constructor is private, so the only way to get a Person is through
//   Create, which validates.  Every Person that exists is valid.
// - The properties have private init accessors, so `with` works only inside
//   the type.  Its own "modifying" operations validate first, then copy.
// - Operations are pure.  They never mutate the instance; "modifying"
//   operations return a new Person.  Operations that can fail return a Result
//   instead of throwing.
//
// Person is never serialized.  PersonDto is its JSON shape.
//
public sealed record class Person
{
    //------------------------------------------------------------------------------
    // Constants
    //------------------------------------------------------------------------------

    private const int MaxNameLength = 40;

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Validating factory ("smart constructor").  Produces a Person only
    /// when all inputs are valid.
    /// </summary>
    public static Result<Person, string>
    Create(
        string firstName,
        string lastName
    ) =>
        from first in ValidateName(firstName, nameof(firstName))
        from last in ValidateName(lastName, nameof(lastName))
        select new Person(first, last);


    /// <summary>
    /// Cleans up a name, then validates it.  Returns the cleaned-up name.
    /// </summary>
    private static Result<string, string>
    ValidateName(
        string value,
        string fieldName
    )
    {
        // Normalize so that accented letters typed as a base letter followed by
        // a combining mark (e.g. "e" + U+0301) become a single letter (é).
        string cleaned = StringNormalizers.NormalizeFormC(value.Trim());

        return
            StringValidators.NonBlank(cleaned, fieldName)
            .Bind((name) => StringValidators.MaxLengthInTextElements(name, MaxNameLength, fieldName))
            .Bind((name) => LettersAndSeparators(name, fieldName));
    }


    /// <summary>
    /// Requires that a name consist of letters (in any language), optionally
    /// joined by single apostrophes, hyphens or spaces, and optionally followed
    /// by periods.  The name must start with a letter.  For example, "O'Brien",
    /// "Mary-Jane", "Van Dyke", "St. John", "Jr." and "J.R." are accepted, but
    /// "-Doe", "Doe'", "Van  Dyke", ".Doe" and "Doe.." are not.
    /// </summary>
    // Enumerates by Rune rather than char so that characters outside the Basic
    // Multilingual Plane (such as emoji) are examined as a whole, not as two
    // surrogate halves.
    private static Result<string, string>
    LettersAndSeparators(
        string value,
        string fieldName
    )
    {
        Rune[] runes = [.. value.EnumerateRunes()];

        bool hasDisallowedRune = runes.Any((rune) => !Rune.IsLetter(rune) && !IsJoiner(rune) && !IsPeriod(rune));
        bool startsWithNonLetter = runes.Length == 0 || !Rune.IsLetter(runes[0]);
        bool endsWithJoiner = runes.Length > 0 && IsJoiner(runes[^1]);
        bool hasIllegalNeighbors = runes
            .Zip(runes.Skip(1))
            .Any((pair) => IsIllegalNeighborPair(pair.First, pair.Second));

        return hasDisallowedRune || startsWithNonLetter || endsWithJoiner || hasIllegalNeighbors ?
            Error($"{fieldName} must contain only letters, with single apostrophes, hyphens or spaces between them and periods after them.") :
            Success(value);
    }


    private static bool IsIllegalNeighborPair(Rune prev, Rune next) =>
        // Two joiners in a row: "--", "' ", "  "
        (IsJoiner(prev) && IsJoiner(next)) ||
        // A joiner followed by a period: "-.", " ."
        (IsJoiner(prev) && IsPeriod(next)) ||
        // Two periods in a row: ".."
        (IsPeriod(prev) && IsPeriod(next));


    // Characters that may join two parts of a name.  Accepts both the ASCII
    // apostrophe and the typographic apostrophe (U+2019), which phones and word
    // processors often substitute automatically.
    private static bool IsJoiner(Rune rune) => rune.Value is '\'' or '’' or '-' or ' ';


    private static bool IsPeriod(Rune rune) => rune.Value == '.';

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    // Taking every value as a parameter means the compiler makes Create supply
    // them all.
    private Person(string firstName, string lastName)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
    }

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    public string FirstName { get; private init; }
    public string LastName { get; private init; }

    // Cheap derived values may be computed properties, because domain types are
    // never serialized.
    public string FullName => $"{this.FirstName} {this.LastName}";
    public string Initials => $"{this.FirstName[0]}{this.LastName[0]}";

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// "Changes" the last name by returning a new Person.  The original
    /// instance is left untouched.
    /// </summary>
    public Result<Person, string>
    WithLastName(
        string lastName
    ) =>
        ValidateName(lastName, nameof(lastName))
        .MapSuccess((last) => this with { LastName = last });
}


//
// Operations on a type that cannot be given members, such as a collection of
// Person, go in an Ext class.  Declaring them as extension methods allows them
// to be called either as functions (PersonExt.FindByLastName(people, "Doe")) or
// fluently (people.FindByLastName("Doe")).
//
public static class PersonExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static Option<Person>
    FindByLastName(
        this IEnumerable<Person> people,
        string lastName
    ) =>
        people.FirstOrDefault((p) => string.Equals(p.LastName, lastName, StringComparison.OrdinalIgnoreCase)) is { } found ?
            Some(found) :
            none;
}
