//
// At least a portion of the code below was created using AI tool Claude Code.
//

using FnUtil;

namespace ExampleLib;


//
// A DTO: the JSON shape of a Person.  It holds data only, with public required
// init properties, so the serializer can create it from any well-formed JSON.
// Nothing is validated here; validation happens when it is mapped to a Person.
//
public sealed record class PersonDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}


//
// Mapping between the DTO and its domain type.  It lives with the DTO, so the
// domain type never depends on its wire format.  ToDomain always returns a
// Result, because the JSON may describe an invalid Person.
//
public static class PersonDtoExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static PersonDto FromDomain(Person person) =>
        new() { FirstName = person.FirstName, LastName = person.LastName };


    public static Result<Person, string> ToDomain(this PersonDto dto) =>
        Person.Create(dto.FirstName, dto.LastName);
}
