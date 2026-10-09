namespace FnUtil.Tests;

public class UnitTests
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    [Fact]
    public void CanBeUsedConveniently()
    {
        _ = unit;
    }

    [Fact]
    public void AllInstancesOfUnitAreEqual()
    {
        var a = unit;
        var b = unit;

        Assert.Equal(a, b);
    }
}
