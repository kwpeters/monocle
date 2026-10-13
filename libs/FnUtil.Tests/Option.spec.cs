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
    public async Task NoneE_ReturnsNoneOptionOfSpecifiedType()
    {
        // No type argument or cast is needed on Task.FromResult().
        Task<Option<int>> task = Task.FromResult(NoneE<int>());
        Assert.True((await task).IsNone);
    }


    [Fact]
    public void NoneE_EqualsImplicitlyConvertedNone()
    {
        Option<int> converted = none;
        Assert.Equal(converted, NoneE<int>());
    }


    [Fact]
    public void None_AllInstancesAreEqual()
    {
        None opt1 = none;
        None opt2 = none;
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
        Assert.True(Some(3).IsSome);
    }


    [Fact]
    public void IsSome_WhenWrappingANone_ReturnsFalse()
    {
        Assert.False(NoneE<int>().IsSome);
    }


    [Fact]
    public void IsNone_WhenWrappingASome_ReturnsFalse()
    {
        Assert.False(Some(3).IsNone);
    }


    [Fact]
    public void IsNone_WhenWrappingANone_ReturnsTrue()
    {
        Assert.True(NoneE<int>().IsNone);
    }


    [Fact]
    public void ToOption_WhenGivenNullReference_ReturnsNone()
    {
        string? name = null;
        Option<string> opt = name!.ToOption();
        Assert.True(opt.IsNone);
    }


    [Fact]
    public void Some_WhenGivenNullReference_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Some<string>(null!));
    }
}
