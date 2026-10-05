namespace FnUtil.Tests;


public class OptionTests
{
    [Fact]
    public void Some_CanBeEasilyCreated()
    {
        _ = Some(5);
    }


    [Fact]
    public void None_CanBeEasilyCreated()
    {
        _ = none;
    }


    [Fact]
    public void None_AllInstancesAreEqual()
    {
        var opt1 = none;
        var opt2 = none;
        Assert.True(opt1 == opt2);
    }


    [Fact]
    public void None_WhenCompared_UsesValueEquality()
    {
        Option<string> opt1 = none;
        Option<string> opt2 = none;
        Assert.True(opt1 == opt2);
    }


    [Fact]
    public void Some_WhenCompared_UsesValueEquality()
    {
        Option<string> opt1 = Some("foo");
        Option<string> opt2 = Some("foo");
        Assert.True(opt1 == opt2);
    }


    [Fact]
    public void IsSome_WhenWrappingASome_ReturnsTrue()
    {
        Option<int> opt = Some(3);
        Assert.True(opt.IsSome);
    }


    [Fact]
    public void IsSome_WhenWrappingANone_ReturnsFalse()
    {
        Option<int> opt = none;
        Assert.False(opt.IsSome);
    }


    [Fact]
    public void IsNone_WhenWrappingASome_ReturnsFalse()
    {
        Option<int> opt = Some(3);
        Assert.False(opt.IsNone);
    }


    [Fact]
    public void IsNone_WhenWrappingANone_ReturnsTrue()
    {
        Option<int> opt = none;
        Assert.True(opt.IsNone);
    }
}
