//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;
using CsDepot;

namespace ExampleLib.Tests;


public class PersonDtoTests
{
    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Serialization ────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Serialize_WhenDeserialized_RoundTrips()
    {
        PersonDto dto = new() { FirstName = "Jane", LastName = "Doe" };

        string json = JsonSerializer.Serialize(dto, JsonOptions.Default);
        Option<PersonDto> copy = JsonSerializer.Deserialize<PersonDto>(json, JsonOptions.Default).ToOption();

        Assert.Equal("""{"firstName":"Jane","lastName":"Doe"}""", json);
        Assert.Equal(Some(dto), copy);
    }


    [Theory]
    [InlineData("""{"firstName":"Jane"}""")]                    // missing property
    [InlineData("""{"firstName":"Jane","lastName":null}""")]    // null property
    public void Deserialize_WhenInputIsMalformed_Throws(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<PersonDto>(json, JsonOptions.Default));
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FromDomain ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void FromDomain_CopiesNames()
    {
        Person person = Person.Create("Jane", "Doe").ThrowIfError();

        PersonDto dto = PersonDtoExt.FromDomain(person);

        Assert.Equal(new PersonDto { FirstName = "Jane", LastName = "Doe" }, dto);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ToDomain ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ToDomain_WhenNamesAreValid_ReturnsPerson()
    {
        PersonDto dto = new() { FirstName = "Jane", LastName = "Doe" };

        Person person = dto.ToDomain().ThrowIfError();

        Assert.Equal(Person.Create("Jane", "Doe").ThrowIfError(), person);
    }


    // Well-formed JSON can still describe an invalid Person.  Validation
    // happens here, at the boundary, not in the DTO.
    [Fact]
    public void ToDomain_WhenNameIsInvalid_ReturnsError()
    {
        PersonDto dto = new() { FirstName = "", LastName = "Doe" };

        Result<Person, string> result = dto.ToDomain();

        Assert.Equal("firstName must not be blank.", result.ThrowIfSuccess());
    }
}
