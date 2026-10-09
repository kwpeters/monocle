using ExperimentalLib;
using FnUtil;

////////////////////////////////////////////////////////////////////////////////
// Creating data: validation happens once, at the boundary.  Every PersonClass
// that comes out of Create() is known to be valid.
////////////////////////////////////////////////////////////////////////////////

(string First, string Last)[] rawInputs =
[
    ("Jane", "Doe"),
    ("John", "Smith"),
    ("  ", "Nobody"),
];

var (people, errors) = rawInputs
    .Select((input) => PersonOps.Create(input.First, input.Last))
    .Partition();

foreach (string error in errors)
{
    Console.WriteLine($"Invalid input: {error}");
}

////////////////////////////////////////////////////////////////////////////////
// Querying data: functions that may not find anything return Option, so the
// caller must handle the "not found" case.
////////////////////////////////////////////////////////////////////////////////

Console.WriteLine(
    people
    .FindByLastName("doe")
    .Match(
        (person) => $"Found: {person.FullName()} ({person.Initials()})",
        () => "Not found"
    )
);

Console.WriteLine(
    people
    .FindByLastName("Jones")
    .Match(
        (person) => $"Found: {person.FullName()}",
        () => "Jones not found"
    )
);

////////////////////////////////////////////////////////////////////////////////
// Transforming data: operations return new instances.  Steps that can fail are
// chained with Bind, and the first failure short-circuits the rest.
////////////////////////////////////////////////////////////////////////////////

var original = PersonOps.Create("Jane", "Doe").ThrowIfError();

original
    .WithLastName("Smith")
    .Match(
        (renamed) => Console.WriteLine($"Renamed: {original.FullName()} -> {renamed.FullName()}"),
        (error) => Console.WriteLine($"Rename failed: {error}")
    );

PersonOps.Create("Jane", "Doe")
    .Bind((person) => person.WithLastName(""))
    .Bind((person) => person.WithLastName("Smith"))
    .Match(
        (person) => Console.WriteLine($"Renamed: {person.FullName()}"),
        (error) => Console.WriteLine($"Rename failed: {error}")
    );

// The original was never modified.
Console.WriteLine($"Original is still: {original.FullName()}");
