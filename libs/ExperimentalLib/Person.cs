using System.Text;
using CsDepot;
using FnUtil;
using static FnUtil.F;

namespace ExperimentalLib;

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
// Data: an immutable record with no behavior.
//
public sealed record class Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}


//
// Operations: pure functions that take data in and return new data out.  They
// never mutate their inputs.  Operations that can fail return a Result instead
// of throwing.  Declaring them as extension methods allows them to be called
// either as functions (PersonOps.FullName(p)) or fluently (p.FullName()).
//
public static class PersonOps
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
    Create(string firstName, string lastName) =>
        from first in ValidateName(firstName, nameof(firstName))
        from last in ValidateName(lastName, nameof(lastName))
        select new Person { FirstName = first, LastName = last };


    public static string
    FullName(this Person person) => $"{person.FirstName} {person.LastName}";


    public static string
    Initials(this Person person) => $"{person.FirstName[0]}{person.LastName[0]}";


    /// <summary>
    /// "Changes" the last name by returning a new Person.  The original
    /// instance is left untouched.
    /// </summary>
    public static Result<Person, string>
    WithLastName(this Person person, string lastName) =>
        ValidateName(lastName, nameof(lastName))
        .MapSuccess((last) => person with { LastName = last });


    public static Option<Person>
    FindByLastName(this IEnumerable<Person> people, string lastName) =>
        people.FirstOrDefault((p) => string.Equals(p.LastName, lastName, StringComparison.OrdinalIgnoreCase)) is { } found ?
            Some(found) :
            none;


    /// <summary>
    /// Cleans up a name, then validates it.  Returns the cleaned-up name.
    /// </summary>
    private static Result<string, string>
    ValidateName(string value, string fieldName)
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
    LettersAndSeparators(string value, string fieldName)
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


    private static bool
    IsIllegalNeighborPair(Rune prev, Rune next) =>
        // Two joiners in a row: "--", "' ", "  "
        (IsJoiner(prev) && IsJoiner(next)) ||
        // A joiner followed by a period: "-.", " ."
        (IsJoiner(prev) && IsPeriod(next)) ||
        // Two periods in a row: ".."
        (IsPeriod(prev) && IsPeriod(next));


    // Characters that may join two parts of a name.  Accepts both the ASCII
    // apostrophe and the typographic apostrophe (U+2019), which phones and word
    // processors often substitute automatically.
    private static bool
    IsJoiner(Rune rune) => rune.Value is '\'' or '’' or '-' or ' ';


    private static bool
    IsPeriod(Rune rune) => rune.Value == '.';
}
