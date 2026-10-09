//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace FnUtil.Tests;


public class OptionExtTests
{
    //
    // ══════════════════════════════════════════════════════════════════════
    // ── AllOrNone ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void AllOrNone_WhenGivenEmptySequence_ReturnsSomeWithEmptyCollection()
    {
        var input = Enumerable.Empty<Option<int>>();

        var result = input.AllOrNone();

        Assert.Empty(result.ThrowIfNone());
    }


    [Fact]
    public void AllOrNone_WhenGivenAllSome_ReturnsSomeWithAllValuesInOrder()
    {
        var input = new Option<int>[] { Some(1), Some(2), Some(3) };

        var result = input.AllOrNone();

        Assert.Equal(new[] { 1, 2, 3 }, result.ThrowIfNone());
    }


    [Fact]
    public void AllOrNone_WhenGivenANone_ReturnsNone()
    {
        var input = new Option<int>[] { Some(1), none, Some(3) };

        var result = input.AllOrNone();

        Assert.True(result.IsNone);
    }


    [Fact]
    public void AllOrNone_WhenGivenANone_StopsEnumeratingAtFirstNone()
    {
        int numEnumerated = 0;
        var input = new Option<int>[] { Some(1), none, Some(2), none }
            .Select((opt) =>
            {
                numEnumerated++;
                return opt;
            });

        var result = input.AllOrNone();

        Assert.True(result.IsNone);
        Assert.Equal(2, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Bind ──────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Bind_WhenSome_ReturnsOptionFromFunction()
    {
        Option<string> opt = Some("Fred");
        var result = opt.Bind((name) => Some(name.Length));
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public void Bind_WhenSomeAndFunctionReturnsNone_ReturnsNone()
    {
        Option<string> opt = Some("Fred");
        var result = opt.Bind((name) => (Option<int>)none);
        Assert.True(result.IsNone);
    }


    [Fact]
    public void Bind_WhenNone_ReturnsNoneWithoutCallingFunction()
    {
        var called = false;
        Option<string> opt = none;
        var result = opt.Bind((name) =>
        {
            called = true;
            return Some(name.Length);
        });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── Bind (Tier 2 lift — sync callback) ────────────────────────────────

    [Fact]
    public async Task Bind_Task_WhenSome_ChainsCorrectly()
    {
        var result = await Task.FromResult(Some(42))
            .Bind((x) => Some($"Value: {x}"));
        Assert.Equal(Some("Value: 42"), result);
    }


    [Fact]
    public async Task Bind_Task_WhenNone_ReturnsNoneWithoutCallingFunction()
    {
        var called = false;
        var result = await Task.FromResult<Option<int>>(none)
            .Bind((x) =>
            {
                called = true;
                return Some($"Value: {x}");
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── BindAsync (Tier 3 — async callback) ───────────────────────────────

    [Fact]
    public async Task BindAsync_WhenSome_ChainsCorrectly()
    {
        var result = await Task.FromResult(Some(21))
            .BindAsync(async (x) =>
            {
                await Task.Delay(1);
                return Some(x * 2);
            });
        Assert.Equal(Some(42), result);
    }


    [Fact]
    public async Task BindAsync_WhenSomeAndFunctionReturnsNone_ReturnsNone()
    {
        var result = await Task.FromResult(Some(21))
            .BindAsync(async (x) =>
            {
                await Task.Delay(1);
                return (Option<int>)none;
            });
        Assert.True(result.IsNone);
    }


    [Fact]
    public async Task BindAsync_WhenNone_ReturnsNoneWithoutCallingFunction()
    {
        var called = false;
        var result = await Task.FromResult<Option<int>>(none)
            .BindAsync(async (x) =>
            {
                called = true;
                await Task.Delay(1);
                return Some(x * 2);
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── DefaultValue ──────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void DefaultValue_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = opt.DefaultValue("default");
        Assert.Equal("Fred", val);
    }


    [Fact]
    public void DefaultValue_WhenNone_ReturnsDefaultValue()
    {
        Option<string> opt = none;
        var val = opt.DefaultValue("default");
        Assert.Equal("default", val);
    }


    // ── DefaultValue (Tier 2 lift) ────────────────────────────────────────

    [Fact]
    public async Task DefaultValue_Task_WhenSome_ReturnsContainedValue()
    {
        var val = await Task.FromResult(Some("Fred")).DefaultValue("default");
        Assert.Equal("Fred", val);
    }


    [Fact]
    public async Task DefaultValue_Task_WhenNone_ReturnsDefaultValue()
    {
        var val = await Task.FromResult<Option<string>>(none).DefaultValue("default");
        Assert.Equal("default", val);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── DefaultWith ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void DefaultWith_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = opt.DefaultWith(() => "fallback");
        Assert.Equal("Fred", val);
    }


    [Fact]
    public void DefaultWith_WhenNone_ReturnsComputedValue()
    {
        Option<string> opt = none;
        var val = opt.DefaultWith(() => "fallback");
        Assert.Equal("fallback", val);
    }


    [Fact]
    public void DefaultWith_WhenSome_DoesNotCallFunction()
    {
        var called = false;
        Option<string> opt = Some("Fred");
        opt.DefaultWith(() =>
        {
            called = true;
            return "fallback";
        });
        Assert.False(called);
    }


    // ── DefaultWith (Tier 2 lift — sync callback) ─────────────────────────

    [Fact]
    public async Task DefaultWith_Task_WhenSome_ReturnsContainedValueWithoutCallingFunction()
    {
        var called = false;
        var val = await Task.FromResult(Some("Fred"))
            .DefaultWith(() =>
            {
                called = true;
                return "fallback";
            });
        Assert.Equal("Fred", val);
        Assert.False(called);
    }


    [Fact]
    public async Task DefaultWith_Task_WhenNone_ReturnsComputedValue()
    {
        var val = await Task.FromResult<Option<string>>(none).DefaultWith(() => "fallback");
        Assert.Equal("fallback", val);
    }


    // ── DefaultWithAsync (Tier 3 — async callback) ────────────────────────

    [Fact]
    public async Task DefaultWithAsync_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        var val = await opt.DefaultWithAsync(() => Task.FromResult("fallback"));
        Assert.Equal("Fred", val);
    }


    [Fact]
    public async Task DefaultWithAsync_WhenNone_ReturnsComputedValue()
    {
        Option<string> opt = none;
        var val = await opt.DefaultWithAsync(() => Task.FromResult("fallback"));
        Assert.Equal("fallback", val);
    }


    [Fact]
    public async Task DefaultWithAsync_WhenSome_DoesNotCallFunction()
    {
        var called = false;
        Option<string> opt = Some("Fred");
        await opt.DefaultWithAsync(() =>
        {
            called = true;
            return Task.FromResult("fallback");
        });
        Assert.False(called);
    }


    [Fact]
    public async Task DefaultWithAsync_Task_WhenSome_ReturnsContainedValueWithoutCallingFunction()
    {
        var called = false;
        var val = await Task.FromResult(Some("Fred"))
            .DefaultWithAsync(async () =>
            {
                called = true;
                await Task.Delay(1);
                return "fallback";
            });
        Assert.Equal("Fred", val);
        Assert.False(called);
    }


    [Fact]
    public async Task DefaultWithAsync_Task_WhenNone_ReturnsComputedValue()
    {
        var val = await Task.FromResult<Option<string>>(none)
            .DefaultWithAsync(async () =>
            {
                await Task.Delay(1);
                return "fallback";
            });
        Assert.Equal("fallback", val);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Gate ──────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Gate_WhenSomeAndGateReturnsSome_ReturnsOriginalOption()
    {
        Option<int> opt = Some(10);
        var result = opt.Gate((x) => x > 5 ? Some("big enough") : none);
        Assert.Equal(Some(10), result);
    }


    [Fact]
    public void Gate_WhenSomeAndGateReturnsNone_ReturnsNone()
    {
        Option<int> opt = Some(3);
        var result = opt.Gate((x) => x > 5 ? Some("big enough") : none);
        Assert.True(result.IsNone);
    }


    [Fact]
    public void Gate_WhenNone_ReturnsNoneWithoutCallingGate()
    {
        var called = false;
        Option<int> opt = none;
        var result = opt.Gate((x) =>
        {
            called = true;
            return Some(true);
        });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── Gate (Tier 2 lift — sync callback) ────────────────────────────────

    [Fact]
    public async Task Gate_Task_WhenGateReturnsSome_ReturnsOriginalOption()
    {
        var result = await Task.FromResult(Some(10))
            .Gate((x) => x > 5 ? Some(true) : none);
        Assert.Equal(Some(10), result);
    }


    [Fact]
    public async Task Gate_Task_WhenGateReturnsNone_ReturnsNone()
    {
        var result = await Task.FromResult(Some(3))
            .Gate((x) => x > 5 ? Some(true) : none);
        Assert.True(result.IsNone);
    }


    // ── GateAsync (Tier 3 — async callback) ───────────────────────────────

    [Fact]
    public async Task GateAsync_WhenGateReturnsSome_ReturnsOriginalOption()
    {
        var result = await Task.FromResult(Some(10))
            .GateAsync(async (x) =>
            {
                await Task.Delay(1);
                return x > 5 ? Some(true) : none;
            });
        Assert.Equal(Some(10), result);
    }


    [Fact]
    public async Task GateAsync_WhenGateReturnsNone_ReturnsNone()
    {
        var result = await Task.FromResult(Some(3))
            .GateAsync(async (x) =>
            {
                await Task.Delay(1);
                return x > 5 ? Some(true) : none;
            });
        Assert.True(result.IsNone);
    }


    [Fact]
    public async Task GateAsync_WhenNone_ReturnsNoneWithoutCallingGate()
    {
        var called = false;
        var result = await Task.FromResult<Option<int>>(none)
            .GateAsync(async (x) =>
            {
                called = true;
                await Task.Delay(1);
                return Some(true);
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── GetValue ──────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

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

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Map ───────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

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


    // ── Map (Tier 2 lift — sync callback) ─────────────────────────────────

    [Fact]
    public async Task Map_Task_WhenSome_ReturnsSomeWithMappedValue()
    {
        var result = await Task.FromResult(Some("Fred")).Map((name) => name.Length);
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public async Task Map_Task_WhenNone_ReturnsNoneWithoutCallingMapFn()
    {
        var called = false;
        var result = await Task.FromResult<Option<string>>(none)
            .Map((name) =>
            {
                called = true;
                return name.Length;
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── MapAsync (Tier 3 — async callback) ────────────────────────────────

    [Fact]
    public async Task MapAsync_WhenSome_ReturnsSomeWithMappedValue()
    {
        var result = await Task.FromResult(Some("Fred"))
            .MapAsync(async (name) =>
            {
                await Task.Delay(1);
                return name.Length;
            });
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public async Task MapAsync_WhenNone_ReturnsNoneWithoutCallingMapFn()
    {
        var called = false;
        var result = await Task.FromResult<Option<string>>(none)
            .MapAsync(async (name) =>
            {
                called = true;
                await Task.Delay(1);
                return name.Length;
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Match ─────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

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


    // The callbacks in the Action tests below return void (List.Add), so they
    // can only bind to the Action overloads, not the Func overloads.

    [Fact]
    public void Match_Action_WhenGivenASome_RunsTheSomeAction()
    {
        var calls = new List<string>();
        Option<string> opt = Some("Fred");
        opt.Match(
            calls.Add,
            () => calls.Add("none")
        );
        Assert.Equal(new[] { "Fred" }, calls);
    }


    [Fact]
    public void Match_Action_WhenGivenANone_RunsTheNoneAction()
    {
        var calls = new List<string>();
        Option<string> opt = none;
        opt.Match(
            calls.Add,
            () => calls.Add("none")
        );
        Assert.Equal(new[] { "none" }, calls);
    }


    // ── Match (Tier 2 lift — sync callbacks) ──────────────────────────────

    [Fact]
    public async Task Match_Task_WhenGivenASome_RunsTheSomeFunction()
    {
        var res = await Task.FromResult(Some("Fred")).Match((name) => name.Length, () => 0);
        Assert.Equal(4, res);
    }


    [Fact]
    public async Task Match_Task_WhenGivenANone_RunsTheNoneFunction()
    {
        var res = await Task.FromResult<Option<string>>(none).Match((name) => name.Length, () => 0);
        Assert.Equal(0, res);
    }


    [Fact]
    public async Task Match_Task_Action_WhenGivenASome_RunsTheSomeAction()
    {
        var calls = new List<string>();
        await Task.FromResult(Some("Fred")).Match(
            calls.Add,
            () => calls.Add("none")
        );
        Assert.Equal(new[] { "Fred" }, calls);
    }


    [Fact]
    public async Task Match_Task_Action_WhenGivenANone_RunsTheNoneAction()
    {
        var calls = new List<string>();
        await Task.FromResult<Option<string>>(none).Match(
            calls.Add,
            () => calls.Add("none")
        );
        Assert.Equal(new[] { "none" }, calls);
    }


    // ── MatchAsync (Tier 3 — async callbacks) ─────────────────────────────

    [Fact]
    public async Task MatchAsync_WhenGivenASome_RunsTheSomeFunction()
    {
        var res = await Task.FromResult(Some("Fred")).MatchAsync(
            async (name) =>
            {
                await Task.Delay(1);
                return name.Length;
            },
            async () =>
            {
                await Task.Delay(1);
                return 0;
            }
        );
        Assert.Equal(4, res);
    }


    [Fact]
    public async Task MatchAsync_WhenGivenANone_RunsTheNoneFunction()
    {
        var res = await Task.FromResult<Option<string>>(none).MatchAsync(
            async (name) =>
            {
                await Task.Delay(1);
                return name.Length;
            },
            async () =>
            {
                await Task.Delay(1);
                return 0;
            }
        );
        Assert.Equal(0, res);
    }


    [Fact]
    public async Task MatchAsync_Action_WhenGivenASome_RunsTheSomeAction()
    {
        var captured = "";
        await Task.FromResult(Some("Fred")).MatchAsync(
            async (name) =>
            {
                await Task.Delay(1);
                captured = name;
            },
            async () =>
            {
                await Task.Delay(1);
                captured = "none";
            }
        );
        Assert.Equal("Fred", captured);
    }


    [Fact]
    public async Task MatchAsync_Action_WhenGivenANone_RunsTheNoneAction()
    {
        var captured = "";
        await Task.FromResult<Option<string>>(none).MatchAsync(
            async (name) =>
            {
                await Task.Delay(1);
                captured = name;
            },
            async () =>
            {
                await Task.Delay(1);
                captured = "none";
            }
        );
        Assert.Equal("none", captured);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Select ────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

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

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── SelectMany ────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

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

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Somes ─────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Somes_WhenGivenEmptySequence_ReturnsEmptySequence()
    {
        var input = Enumerable.Empty<Option<int>>();
        Assert.Empty(input.Somes());
    }


    [Fact]
    public void Somes_WhenGivenAllNone_ReturnsEmptySequence()
    {
        var input = new Option<int>[] { none, none };
        Assert.Empty(input.Somes());
    }


    [Fact]
    public void Somes_WhenGivenMixedOptions_ReturnsOnlySomeValuesInOrder()
    {
        var input = new Option<int>[] { Some(1), none, Some(2), none, Some(3) };
        Assert.Equal(new[] { 1, 2, 3 }, input.Somes());
    }


    [Fact]
    public void Somes_IsEvaluatedLazily()
    {
        int numEnumerated = 0;
        var input = new Option<int>[] { none, Some(1), Some(2) }
            .Select((opt) =>
            {
                numEnumerated++;
                return opt;
            });

        var somes = input.Somes();
        Assert.Equal(0, numEnumerated);

        Assert.Equal(1, somes.First());
        Assert.Equal(2, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ThrowIfNone ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ThrowIfNone_WhenSome_ReturnsContainedValue()
    {
        Option<string> opt = Some("Fred");
        Assert.Equal("Fred", opt.ThrowIfNone());
    }


    [Fact]
    public void ThrowIfNone_WhenNone_ThrowsWithDefaultMessage()
    {
        Option<string> opt = none;
        var ex = Assert.Throws<InvalidOperationException>(() => opt.ThrowIfNone());
        Assert.Equal("Expected Some Option but got None.", ex.Message);
    }


    [Fact]
    public void ThrowIfNone_WhenNone_ThrowsWithCustomMessage()
    {
        Option<string> opt = none;
        var ex = Assert.Throws<InvalidOperationException>(() => opt.ThrowIfNone("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }


    // ── ThrowIfNone (Tier 2 lift) ─────────────────────────────────────────

    [Fact]
    public async Task ThrowIfNone_Task_WhenSome_ReturnsContainedValue()
    {
        var val = await Task.FromResult(Some("Fred")).ThrowIfNone();
        Assert.Equal("Fred", val);
    }


    [Fact]
    public async Task ThrowIfNone_Task_WhenNone_ThrowsWithCustomMessage()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult<Option<string>>(none).ThrowIfNone("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ToOption ──────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

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
}
