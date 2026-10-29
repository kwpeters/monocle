//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace ExampleLib.Tests;


public class PersonTests
{
    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Create ───────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Create_WhenNamesAreValid_ReturnsPerson()
    {
        Person person = Person.Create("Jane", "Doe").ThrowIfError();

        Assert.Equal("Jane", person.FirstName);
        Assert.Equal("Doe", person.LastName);
    }


    [Fact]
    public void Create_WhenNamesHaveSurroundingWhitespace_TrimsThem()
    {
        Person person = Person.Create("  Jane ", " Doe  ").ThrowIfError();

        Assert.Equal("Jane", person.FirstName);
        Assert.Equal("Doe", person.LastName);
    }


    [Fact]
    public void Create_WhenFirstNameIsBlank_ReturnsError()
    {
        Result<Person, string> result = Person.Create("  ", "Doe");

        Assert.Equal("firstName must not be blank.", result.GetError());
    }


    [Fact]
    public void Create_WhenLastNameHasDisallowedCharacters_ReturnsError()
    {
        Result<Person, string> result = Person.Create("Jane", "Doe42");

        Assert.StartsWith("lastName must contain only letters", result.GetError());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FullName and Initials ────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void FullName_JoinsFirstAndLastNames()
    {
        Person person = Person.Create("Jane", "Doe").ThrowIfError();

        Assert.Equal("Jane Doe", person.FullName);
    }


    [Fact]
    public void Initials_AreFirstLettersOfEachName()
    {
        Person person = Person.Create("Jane", "Doe").ThrowIfError();

        Assert.Equal("JD", person.Initials);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── WithLastName ─────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void WithLastName_WhenNameIsValid_ReturnsNewPersonAndLeavesOriginalUnchanged()
    {
        Person original = Person.Create("Jane", "Doe").ThrowIfError();

        Person renamed = original.WithLastName("Smith").ThrowIfError();

        Assert.Equal("Smith", renamed.LastName);
        Assert.Equal("Doe", original.LastName);
    }


    [Fact]
    public void WithLastName_WhenNameIsInvalid_ReturnsError()
    {
        Person original = Person.Create("Jane", "Doe").ThrowIfError();

        Result<Person, string> result = original.WithLastName("");

        Assert.Equal("lastName must not be blank.", result.GetError());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── FindByLastName ───────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void FindByLastName_WhenNameMatchesIgnoringCase_ReturnsSome()
    {
        Person jane = Person.Create("Jane", "Doe").ThrowIfError();
        Person john = Person.Create("John", "Smith").ThrowIfError();

        Option<Person> found = new[] { jane, john }.FindByLastName("smith");

        Assert.Equal(Some(john), found);
    }


    [Fact]
    public void FindByLastName_WhenNoNameMatches_ReturnsNone()
    {
        Person jane = Person.Create("Jane", "Doe").ThrowIfError();

        Option<Person> found = new[] { jane }.FindByLastName("Jones");

        Assert.True(found.IsNone);
    }
}
