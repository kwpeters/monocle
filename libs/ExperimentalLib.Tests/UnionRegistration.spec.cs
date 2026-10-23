//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Reflection;
using CsDepot;
using ExperimentalLib.Drawing;

namespace ExperimentalLib.Tests;


//
// Every assembly that defines unions has one of these tests.  It checks that
// all of the assembly's unions follow the conventions in
// docs/csharp_data_modeling.md.
//
public class UnionRegistrationTests
{
    //------------------------------------------------------------------------------
    // Static fields
    //------------------------------------------------------------------------------

    // Any type from the assembly under test.
    private static readonly Assembly _targetAssembly = typeof(Shape).Assembly;

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    [Fact]
    public void FindUnions_InTargetAssembly_FindsAtLeastOneUnion()
    {
        Assert.NotEmpty(UnionConventions.FindUnions(_targetAssembly));
    }


    [Fact]
    public void FindAllViolations_InTargetAssembly_ReturnsEmpty()
    {
        Assert.Empty(UnionConventions.FindAllViolations(_targetAssembly));
    }
}
