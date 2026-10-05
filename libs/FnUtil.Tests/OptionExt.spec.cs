namespace FnUtil.Tests;

public class OptionExtTests
{
    [Fact]
    public void Match_WhenGivenASome_RunsTheSomeFunction()
    {
        Option<string> opt = Some("Fred");
        var res = opt.Match(
            someFn: (name) => 1,
            noneFn: () => 0
        );
        Assert.Equal(1, res);

        res = opt.Match((_) => 1, () => 0);
        Assert.Equal(1, res);
    }


    [Fact]
    public void Match_WhenGivenANone_RunsTheNoneFunction()
    {
        Option<string> opt = none;
        var res = opt.Match(
            someFn: (name) => 1,
            noneFn: () => 0
        );
        Assert.Equal(0, res);
    }


    // GetOrElse with default value

    [Fact]
    public void GetOrElse_Value_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = opt.GetOrElse("default");
        Assert.Equal("Fred", val);
    }

    [Fact]
    public void GetOrElse_Value_WhenNone_ReturnsDefaultValue()
    {
        Option<string> opt = none;
        var val = opt.GetOrElse("default");
        Assert.Equal("default", val);
    }


    // GetOrElse with Func<T> fallback

    [Fact]
    public void GetOrElse_Func_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = opt.GetOrElse(() => "fallback");
        Assert.Equal("Fred", val);
    }

    [Fact]
    public void GetOrElse_Func_WhenNone_ReturnsFallbackValue()
    {
        Option<string> opt = none;
        var val = opt.GetOrElse(() => "fallback");
        Assert.Equal("fallback", val);
    }

    [Fact]
    public void GetOrElse_Func_WhenSome_DoesNotCallFallback()
    {
        var called = false;
        Option<string> opt = Some("Fred");
        opt.GetOrElse(() =>
        {
            called = true;
            return "fallback";
        });
        Assert.False(called);
    }


    // GetOrElse with Func<Task<T>> fallback

    [Fact]
    public async Task GetOrElse_AsyncFunc_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = await opt.GetOrElse(() => Task.FromResult("fallback"));
        Assert.Equal("Fred", val);
    }

    [Fact]
    public async Task GetOrElse_AsyncFunc_WhenNone_ReturnsFallbackValue()
    {
        Option<string> opt = none;
        var val = await opt.GetOrElse(() => Task.FromResult("fallback"));
        Assert.Equal("fallback", val);
    }

    [Fact]
    public async Task GetOrElse_AsyncFunc_WhenSome_DoesNotCallFallback()
    {
        var called = false;
        Option<string> opt = Some("Fred");
        await opt.GetOrElse(() =>
        {
            called = true;
            return Task.FromResult("fallback");
        });
        Assert.False(called);
    }
}
