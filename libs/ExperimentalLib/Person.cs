namespace ExperimentalLib;


public readonly record struct PersonStruct
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}


////////////////////////////////////////////////////////////////////////////////


public sealed record class PersonClass
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
