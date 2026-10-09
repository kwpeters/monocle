using ExperimentalLib;

// Console.WriteLine("Hello, World!");



var p1 = new PersonStruct()
{
    FirstName = "John",
    LastName = "Doe"
};


var p2 = default(PersonStruct);
Console.WriteLine(p2.FirstName);
