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
        Unit a = unit;
        Unit b = unit;

        Assert.Equal(a, b);
    }
}
