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
public sealed record class PersonClass
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
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Validating factory ("smart constructor").  Produces a PersonClass only
    /// when all inputs are valid.
    /// </summary>
    public static Result<PersonClass, string>
    Create(string firstName, string lastName) =>
        NonBlank(firstName, nameof(firstName))
        .Bind((first) =>
            NonBlank(lastName, nameof(lastName))
            .MapSuccess((last) => new PersonClass { FirstName = first, LastName = last }));


    public static string
    FullName(this PersonClass person) => $"{person.FirstName} {person.LastName}";


    public static string
    Initials(this PersonClass person) => $"{person.FirstName[0]}{person.LastName[0]}";


    /// <summary>
    /// "Changes" the last name by returning a new PersonClass.  The original
    /// instance is left untouched.
    /// </summary>
    public static Result<PersonClass, string>
    WithLastName(this PersonClass person, string lastName) =>
        NonBlank(lastName, nameof(lastName))
        .MapSuccess((last) => person with { LastName = last });


    public static Option<PersonClass>
    FindByLastName(this IEnumerable<PersonClass> people, string lastName) =>
        people.FirstOrDefault((p) => string.Equals(p.LastName, lastName, StringComparison.OrdinalIgnoreCase)) is { } found ?
            Some(found) :
            none;


    private static Result<string, string>
    NonBlank(string value, string fieldName) =>
        string.IsNullOrWhiteSpace(value) ?
            Error($"{fieldName} must not be blank.") :
            Success(value.Trim());
}
