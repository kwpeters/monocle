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


public sealed record class PersonClass
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
