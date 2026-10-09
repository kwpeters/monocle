namespace FnUtil.Tests;

public class OptionExtTests
{
    [Fact]
    public void ToOption_WhenGivenNonNullReference_ReturnsSome()
    {
        string name = "Fred";
        Option<string> result = name.ToOption();
        Assert.Equal(Some("Fred"), result);
    }

    [Fact]
    public void ToOption_WhenGivenNullReference_ReturnsNone()
    {
        string? name = null;
        Option<string> result = name!.ToOption();
        Assert.True(result.IsNone);
    }

    // Map

    [Fact]
    public void Map_WhenSome_ReturnsSomeWithMappedValue()
    {
        Option<string> opt = Some("Fred");
        var result = opt.Map((name) => name.Length);
        Assert.Equal(Some(4), result);
    }

    [Fact]
    public void Map_WhenNone_ReturnsNone()
    {
        Option<string> opt = none;
        var result = opt.Map((name) => name.Length);
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Map_WhenNone_DoesNotCallMapFn()
    {
        var called = false;
        Option<string> opt = none;
        opt.Map((name) =>
        {
            called = true;
            return name.Length;
        });
        Assert.False(called);
    }

    // Match

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


    // GetValue

    [Fact]
    public void GetValue_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = opt.GetValue();
        Assert.Equal("Fred", val);
    }

    [Fact]
    public void GetValue_WhenNone_ThrowsInvalidOperationException()
    {
        Option<string> opt = none;
        Assert.Throws<InvalidOperationException>(() => opt.GetValue());
    }

    [Fact]
    public async Task GetValue_Task_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = await Task.FromResult(opt).GetValue();
        Assert.Equal("Fred", val);
    }

    [Fact]
    public async Task GetValue_Task_WhenNone_ThrowsInvalidOperationException()
    {
        Option<string> opt = none;
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await Task.FromResult(opt).GetValue());
    }


    // Select

    [Fact]
    public void Select_WhenSome_ReturnsSomeWithMappedValue()
    {
        Option<string> opt = Some("Fred");
        var result = opt.Select((name) => name.Length);
        Assert.Equal(Some(4), result);
    }

    [Fact]
    public void Select_WhenNone_ReturnsNoneWithoutCallingSelector()
    {
        var called = false;
        Option<string> opt = none;
        var result = opt.Select((name) =>
        {
            called = true;
            return name.Length;
        });
        Assert.True(result.IsNone);
        Assert.False(called);
    }

    [Fact]
    public void Select_WhenUsedWithQuerySyntax_MapsValue()
    {
        Option<string> opt = Some("Fred");

        var result =
            from name in opt
            select name.Length;

        Assert.Equal(Some(4), result);
    }


    // SelectMany

    [Fact]
    public void SelectMany_WhenBothSome_ProjectsBothValues()
    {
        Option<string> opt = Some("Fred");
        var result = opt.SelectMany(
            (name) => Some(name.Length),
            (name, length) => $"{name}:{length}");
        Assert.Equal(Some("Fred:4"), result);
    }

    [Fact]
    public void SelectMany_WhenInputIsNone_ReturnsNoneWithoutCallingNextOrProject()
    {
        var nextCalled = false;
        var projectCalled = false;
        Option<string> opt = none;
        var result = opt.SelectMany(
            (name) =>
            {
                nextCalled = true;
                return Some(name.Length);
            },
            (name, length) =>
            {
                projectCalled = true;
                return $"{name}:{length}";
            });
        Assert.True(result.IsNone);
        Assert.False(nextCalled);
        Assert.False(projectCalled);
    }

    [Fact]
    public void SelectMany_WhenNextReturnsNone_ReturnsNoneWithoutCallingProject()
    {
        var projectCalled = false;
        Option<string> opt = Some("Fred");
        var result = opt.SelectMany(
            (name) => (Option<int>)none,
            (name, length) =>
            {
                projectCalled = true;
                return $"{name}:{length}";
            });
        Assert.True(result.IsNone);
        Assert.False(projectCalled);
    }

    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntaxAndAllSome_CombinesValuesOfDifferentTypes()
    {
        var result =
            from name in Some("Fred")
            from age in Some(42)
            from isAdmin in Some(true)
            select $"{name}/{age}/{isAdmin}";

        Assert.Equal(Some("Fred/42/True"), result);
    }

    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntaxAndMiddleIsNone_StopsAndReturnsNone()
    {
        var lastCalled = false;

        Option<int> last()
        {
            lastCalled = true;
            return Some(3);
        }

        var result =
            from a in Some(1)
            from b in (Option<int>)none
            from c in last()
            select a + b + c;

        Assert.True(result.IsNone);
        Assert.False(lastCalled);
    }

    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntax_AllowsLaterClausesToUseEarlierValues()
    {
        var userIds = new Dictionary<string, int> { ["fred"] = 1, ["wilma"] = 2 };
        var emails = new Dictionary<int, string> { [1] = "fred@example.com" };

        static Option<TValue> lookup<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key) where TKey : notnull =>
            dict.TryGetValue(key, out var value) ?
                Some(value) :
                none;

        Option<string> contactFor(string userName) =>
            from userId in lookup(userIds, userName)
            from email in lookup(emails, userId)
            select $"{userName} <{email}>";

        Assert.Equal(Some("fred <fred@example.com>"), contactFor("fred"));
        Assert.True(contactFor("wilma").IsNone);   // User exists, but has no email
        Assert.True(contactFor("barney").IsNone);  // User does not exist
    }
}
