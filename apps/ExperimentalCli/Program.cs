using ExperimentalLib;

// Console.WriteLine("Hello, World!");

////////////////////////////////////////////////////////////////////////////////


var p1 = new PersonStruct()
{
    FirstName = "John",
    LastName = "Doe"
};


var p2 = default(PersonStruct);
Console.WriteLine(p2.FirstName);


////////////////////////////////////////////////////////////////////////////////


PersonClass p3 = new()
{
    FirstName = "Jane",
    LastName = "Doe"
};

PersonClass p4 = new() {
    FirstName = "Default",
    LastName = "User"
};
Console.WriteLine(p4.FirstName);
