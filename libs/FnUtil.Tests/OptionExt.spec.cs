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
        IEnumerable<Option<int>> input = [];

        Option<IEnumerable<int>> result = input.AllOrNone();

        Assert.Empty(result.ThrowIfNone());
    }


    [Fact]
    public void AllOrNone_WhenGivenAllSome_ReturnsSomeWithAllValuesInOrder()
    {
        var input = new Option<int>[] { Some(1), Some(2), Some(3) };

        Option<IEnumerable<int>> result = input.AllOrNone();

        Assert.Equal(new[] { 1, 2, 3 }, result.ThrowIfNone());
    }


    [Fact]
    public void AllOrNone_WhenGivenANone_ReturnsNone()
    {
        var input = new Option<int>[] { Some(1), none, Some(3) };

        Option<IEnumerable<int>> result = input.AllOrNone();

        Assert.True(result.IsNone);
    }


    [Fact]
    public void AllOrNone_WhenGivenANone_StopsEnumeratingAtFirstNone()
    {
        int numEnumerated = 0;
        IEnumerable<Option<int>> input = new Option<int>[] { Some(1), none, Some(2), none }
            .Select((opt) =>
            {
                numEnumerated++;
                return opt;
            });

        Option<IEnumerable<int>> result = input.AllOrNone();

        Assert.True(result.IsNone);
        Assert.Equal(2, numEnumerated);
    }


    // ── AllOrNone (tuples) ────────────────────────────────────────────────

    [Fact]
    public void AllOrNone_Tuple_WhenAllSome_ReturnsTupleOfValuesOfDifferentTypes()
    {
        Option<int> count = Some(3);
        Option<string> name = Some("Fred");
        Option<bool> isAdmin = Some(true);

        Option<(int, string, bool)> result = (count, name, isAdmin).AllOrNone();

        Assert.Equal(Some((3, "Fred", true)), result);
    }


    [Fact]
    public void AllOrNone_Tuple_WhenAnyNone_ReturnsNone()
    {
        Option<int> count = Some(3);
        Option<string> name = none;
        Option<bool> isAdmin = Some(true);

        Option<(int, string, bool)> result = (count, name, isAdmin).AllOrNone();

        Assert.True(result.IsNone);
    }


    [Fact]
    public void AllOrNone_Tuple_SupportsTwoThroughSevenElements()
    {
        Option<int> o = Some(1);
        Option<int> n = none;

        Assert.Equal(Some((1, 1)), (o, o).AllOrNone());
        Assert.Equal(Some((1, 1, 1)), (o, o, o).AllOrNone());
        Assert.Equal(Some((1, 1, 1, 1)), (o, o, o, o).AllOrNone());
        Assert.Equal(Some((1, 1, 1, 1, 1)), (o, o, o, o, o).AllOrNone());
        Assert.Equal(Some((1, 1, 1, 1, 1, 1)), (o, o, o, o, o, o).AllOrNone());
        Assert.Equal(Some((1, 1, 1, 1, 1, 1, 1)), (o, o, o, o, o, o, o).AllOrNone());
        Assert.True((o, o, o, o, o, o, n).AllOrNone().IsNone);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Bind ──────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Bind_WhenSome_ReturnsOptionFromFunction()
    {
        Option<int> result = Some("Fred").Bind((name) => Some(name.Length));
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public void Bind_WhenSomeAndFunctionReturnsNone_ReturnsNone()
    {
        Option<int> result = Some("Fred").Bind((name) => NoneE<int>());
        Assert.True(result.IsNone);
    }


    [Fact]
    public void Bind_WhenNone_ReturnsNoneWithoutCallingFunction()
    {
        bool called = false;
        Option<int> result = NoneE<string>().Bind((name) =>
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
        Option<string> result = await Task.FromResult(Some(42))
            .Bind((x) => Some($"Value: {x}"));
        Assert.Equal(Some("Value: 42"), result);
    }


    [Fact]
    public async Task Bind_Task_WhenNone_ReturnsNoneWithoutCallingFunction()
    {
        bool called = false;
        Option<string> result = await Task.FromResult(NoneE<int>())
            .Bind((x) =>
            {
                called = true;
                return Some($"Value: {x}");
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── BindAsync (non-Task input — async callback) ───────────────────────

    [Fact]
    public async Task BindAsync_NonTaskInput_WhenSome_ChainsCorrectly()
    {
        Option<int> result =
            await Some(21)
            .BindAsync(async (x) =>
            {
                await Task.Delay(1);
                return Some(x * 2);
            });
        Assert.Equal(Some(42), result);
    }


    [Fact]
    public async Task BindAsync_NonTaskInput_WhenNone_ReturnsNoneWithoutCallingFunction()
    {
        bool called = false;
        Option<int> result =
            await NoneE<int>().
            BindAsync(async (x) =>
            {
                called = true;
                await Task.Delay(1);
                return Some(x * 2);
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── BindAsync (Tier 3 — async callback) ───────────────────────────────

    [Fact]
    public async Task BindAsync_WhenSome_ChainsCorrectly()
    {
        Option<int> result = await Task.FromResult(Some(21))
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
        Option<int> result =
            await Task.FromResult(Some(21))
            .BindAsync(async Task<Option<int>> (x) =>
            {
                await Task.Delay(1);
                return none;
            });
        Assert.True(result.IsNone);
    }


    [Fact]
    public async Task BindAsync_WhenNone_ReturnsNoneWithoutCallingFunction()
    {
        bool called = false;
        Option<int> result = await Task.FromResult(NoneE<int>())
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
    // ── BindNone ──────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void BindNone_WhenSome_ReturnsInputWithoutCallingFunction()
    {
        bool called = false;
        Option<int> result = Some(1).BindNone(() =>
        {
            called = true;
            return Some(5);
        });
        Assert.Equal(Some(1), result);
        Assert.False(called);
    }


    [Fact]
    public void BindNone_WhenNone_ReturnsFallbackOption()
    {
        Option<int> result = NoneE<int>().BindNone(() => Some(5));
        Assert.Equal(Some(5), result);
    }


    [Fact]
    public void BindNone_WhenNoneAndFallbackIsNone_ReturnsNone()
    {
        Option<int> result = NoneE<int>().BindNone(() => none);
        Assert.True(result.IsNone);
    }


    // ── BindNone (Tier 2 lift — sync callback) ────────────────────────────

    [Fact]
    public async Task BindNone_Task_WhenNone_ReturnsFallbackOption()
    {
        Option<int> result = await Task.FromResult(NoneE<int>()).BindNone(() => Some(5));
        Assert.Equal(Some(5), result);
    }


    [Fact]
    public async Task BindNone_Task_WhenSome_ReturnsInputWithoutCallingFunction()
    {
        bool called = false;
        Option<int> result = await Task.FromResult(Some(1)).BindNone(() =>
        {
            called = true;
            return Some(5);
        });
        Assert.Equal(Some(1), result);
        Assert.False(called);
    }


    // ── BindNoneAsync (async callback) ────────────────────────────────────

    [Fact]
    public async Task BindNoneAsync_WhenNone_ReturnsFallbackOption()
    {
        Option<int> result = await NoneE<int>().BindNoneAsync(async () =>
        {
            await Task.Delay(1);
            return Some(5);
        });
        Assert.Equal(Some(5), result);
    }


    [Fact]
    public async Task BindNoneAsync_WhenSome_ReturnsInputWithoutCallingFunction()
    {
        bool called = false;
        Option<int> result = await Some(1).BindNoneAsync(async () =>
        {
            called = true;
            await Task.Delay(1);
            return Some(5);
        });
        Assert.Equal(Some(1), result);
        Assert.False(called);
    }


    [Fact]
    public async Task BindNoneAsync_Task_WhenNone_ReturnsFallbackOption()
    {
        Option<int> result = await Task.FromResult(NoneE<int>()).BindNoneAsync(async () =>
        {
            await Task.Delay(1);
            return Some(5);
        });
        Assert.Equal(Some(5), result);
    }


    [Fact]
    public async Task BindNoneAsync_Task_WhenSome_ReturnsInputWithoutCallingFunction()
    {
        bool called = false;
        Option<int> result = await Task.FromResult(Some(1)).BindNoneAsync(async () =>
        {
            called = true;
            await Task.Delay(1);
            return Some(5);
        });
        Assert.Equal(Some(1), result);
        Assert.False(called);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Choose ────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Choose_KeepsOnlySomeValuesInOrder()
    {
        int[] input = new[] { 1, 2, 3, 4 };
        IEnumerable<int> result = input.Choose((x) => x % 2 == 0 ? Some(x * 10) : none);
        Assert.Equal(new[] { 20, 40 }, result);
    }


    [Fact]
    public void Choose_WhenGivenEmptySequence_ReturnsEmptySequence()
    {
        IEnumerable<int> empty = [];
        IEnumerable<int> result = empty.Choose((x) => Some(x));
        Assert.Empty(result);
    }


    [Fact]
    public void Choose_IsEvaluatedLazily()
    {
        var mapped = new List<int>();

        Option<int> evenTimesTen(int x)
        {
            mapped.Add(x);
            return x % 2 == 0 ? Some(x * 10) : none;
        }

        IEnumerable<int> result = new[] { 1, 2, 3, 4 }.Choose(evenTimesTen);
        Assert.Empty(mapped);

        Assert.Equal(20, result.First());
        Assert.Equal(new[] { 1, 2 }, mapped);
    }


    // ── ChooseAsync (async callback) ──────────────────────────────────────

    [Fact]
    public async Task ChooseAsync_KeepsOnlySomeValuesInOrder()
    {
        int[] input = new[] { 1, 2, 3, 4 };
        IEnumerable<int> result = await input.ChooseAsync(async (x) =>
        {
            await Task.Delay(1);
            return x % 2 == 0 ? Some(x * 10) : none;
        });
        Assert.Equal(new[] { 20, 40 }, result);
    }


    [Fact]
    public async Task ChooseAsync_MapsItemsOneAtATimeInOrder()
    {
        var events = new List<string>();
        int[] input = new[] { 1, 2, 3 };

        await input.ChooseAsync(async (x) =>
        {
            events.Add($"start {x}");
            await Task.Delay(1);
            events.Add($"end {x}");
            return Some(x);
        });

        Assert.Equal(new[] { "start 1", "end 1", "start 2", "end 2", "start 3", "end 3" }, events);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── DefaultValue ──────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void DefaultValue_WhenSome_ReturnsContainedValue()
    {
        string val = Some("Fred").DefaultValue("default");
        Assert.Equal("Fred", val);
    }


    [Fact]
    public void DefaultValue_WhenNone_ReturnsDefaultValue()
    {
        string val = NoneE<string>().DefaultValue("default");
        Assert.Equal("default", val);
    }


    // ── DefaultValue (Tier 2 lift) ────────────────────────────────────────

    [Fact]
    public async Task DefaultValue_Task_WhenSome_ReturnsContainedValue()
    {
        string val = await Task.FromResult(Some("Fred")).DefaultValue("default");
        Assert.Equal("Fred", val);
    }


    [Fact]
    public async Task DefaultValue_Task_WhenNone_ReturnsDefaultValue()
    {
        string val = await Task.FromResult(NoneE<string>()).DefaultValue("default");
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
        string val = Some("Fred").DefaultWith(() => "fallback");
        Assert.Equal("Fred", val);
    }


    [Fact]
    public void DefaultWith_WhenNone_ReturnsComputedValue()
    {
        string val = NoneE<string>().DefaultWith(() => "fallback");
        Assert.Equal("fallback", val);
    }


    [Fact]
    public void DefaultWith_WhenSome_DoesNotCallFunction()
    {
        bool called = false;
        Some("Fred").DefaultWith(() =>
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
        bool called = false;
        string val = await Task.FromResult(Some("Fred"))
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
        string val = await Task.FromResult(NoneE<string>()).DefaultWith(() => "fallback");
        Assert.Equal("fallback", val);
    }


    // ── DefaultWithAsync (Tier 3 — async callback) ────────────────────────

    [Fact]
    public async Task DefaultWithAsync_WhenSome_ReturnsContainedValue()
    {
        string val = await Some("Fred").DefaultWithAsync(() => Task.FromResult("fallback"));
        Assert.Equal("Fred", val);
    }


    [Fact]
    public async Task DefaultWithAsync_WhenNone_ReturnsComputedValue()
    {
        string val = await NoneE<string>().DefaultWithAsync(() => Task.FromResult("fallback"));
        Assert.Equal("fallback", val);
    }


    [Fact]
    public async Task DefaultWithAsync_WhenSome_DoesNotCallFunction()
    {
        bool called = false;
        await Some("Fred").DefaultWithAsync(() =>
        {
            called = true;
            return Task.FromResult("fallback");
        });
        Assert.False(called);
    }


    [Fact]
    public async Task DefaultWithAsync_Task_WhenSome_ReturnsContainedValueWithoutCallingFunction()
    {
        bool called = false;
        string val = await Task.FromResult(Some("Fred"))
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
        string val = await Task.FromResult(NoneE<string>())
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
        Option<int> result = Some(10).Gate((x) => x > 5 ? Some("big enough") : none);
        Assert.Equal(Some(10), result);
    }


    [Fact]
    public void Gate_WhenSomeAndGateReturnsNone_ReturnsNone()
    {
        Option<int> result = Some(3).Gate((x) => x > 5 ? Some("big enough") : none);
        Assert.True(result.IsNone);
    }


    [Fact]
    public void Gate_WhenNone_ReturnsNoneWithoutCallingGate()
    {
        bool called = false;
        Option<int> result = NoneE<int>().Gate((x) =>
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
        Option<int> result = await Task.FromResult(Some(10))
            .Gate((x) => x > 5 ? Some(true) : none);
        Assert.Equal(Some(10), result);
    }


    [Fact]
    public async Task Gate_Task_WhenGateReturnsNone_ReturnsNone()
    {
        Option<int> result = await Task.FromResult(Some(3))
            .Gate((x) => x > 5 ? Some(true) : none);
        Assert.True(result.IsNone);
    }


    // ── GateAsync (non-Task input — async callback) ───────────────────────

    [Fact]
    public async Task GateAsync_NonTaskInput_WhenGateReturnsSome_ReturnsOriginalOption()
    {
        Option<int> result = await Some(10).GateAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 5 ? Some(true) : none;
        });
        Assert.Equal(Some(10), result);
    }


    [Fact]
    public async Task GateAsync_NonTaskInput_WhenGateReturnsNone_ReturnsNone()
    {
        Option<int> result = await Some(3).GateAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 5 ? Some(true) : none;
        });
        Assert.True(result.IsNone);
    }


    // ── GateAsync (Tier 3 — async callback) ───────────────────────────────

    [Fact]
    public async Task GateAsync_WhenGateReturnsSome_ReturnsOriginalOption()
    {
        Option<int> result = await Task.FromResult(Some(10))
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
        Option<int> result = await Task.FromResult(Some(3))
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
        bool called = false;
        Option<int> result = await Task.FromResult(NoneE<int>())
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
        string val = Some("Fred").GetValue();
        Assert.Equal("Fred", val);
    }


    [Fact]
    public void GetValue_WhenNone_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => NoneE<string>().GetValue());
    }


    [Fact]
    public async Task GetValue_Task_WhenSome_ReturnsContainedValue()
    {
        string val = await Task.FromResult(Some("Fred")).GetValue();
        Assert.Equal("Fred", val);
    }


    [Fact]
    public async Task GetValue_Task_WhenNone_ThrowsInvalidOperationException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await Task.FromResult(NoneE<string>()).GetValue());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Map ───────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Map_WhenSome_ReturnsSomeWithMappedValue()
    {
        Option<int> result = Some("Fred").Map((name) => name.Length);
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public void Map_WhenNone_ReturnsNone()
    {
        Option<int> result = NoneE<string>().Map((name) => name.Length);
        Assert.True(result.IsNone);
    }


    [Fact]
    public void Map_WhenNone_DoesNotCallMapFn()
    {
        bool called = false;
        NoneE<string>().Map((name) =>
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
        Option<int> result = await Task.FromResult(Some("Fred")).Map((name) => name.Length);
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public async Task Map_Task_WhenNone_ReturnsNoneWithoutCallingMapFn()
    {
        bool called = false;
        Option<int> result = await Task.FromResult(NoneE<string>())
            .Map((name) =>
            {
                called = true;
                return name.Length;
            });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── MapAsync (non-Task input — async callback) ────────────────────────

    [Fact]
    public async Task MapAsync_NonTaskInput_WhenSome_ReturnsSomeWithMappedValue()
    {
        Option<int> result = await
            Some("Fred")
            .MapAsync(async (name) =>
            {
                await Task.Delay(1);
                return name.Length;
            });
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public async Task MapAsync_NonTaskInput_WhenNone_ReturnsNoneWithoutCallingMapFn()
    {
        bool called = false;
        Option<int> result = await NoneE<string>().MapAsync(async (name) =>
        {
            called = true;
            await Task.Delay(1);
            return name.Length;
        });
        Assert.True(result.IsNone);
        Assert.False(called);
    }


    // ── MapAsync (Tier 3 — async callback) ────────────────────────────────

    [Fact]
    public async Task MapAsync_WhenSome_ReturnsSomeWithMappedValue()
    {
        Option<int> result = await Task.FromResult(Some("Fred"))
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
        bool called = false;
        Option<int> result = await Task.FromResult(NoneE<string>())
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
        int res = opt.Match(
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
        int res = NoneE<string>().Match(
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
        Some("Fred").Match(
            calls.Add,
            () => calls.Add("none")
        );
        Assert.Equal(new[] { "Fred" }, calls);
    }


    [Fact]
    public void Match_Action_WhenGivenANone_RunsTheNoneAction()
    {
        var calls = new List<string>();
        NoneE<string>().Match(
            calls.Add,
            () => calls.Add("none")
        );
        Assert.Equal(new[] { "none" }, calls);
    }


    // ── Match (Tier 2 lift — sync callbacks) ──────────────────────────────

    [Fact]
    public async Task Match_Task_WhenGivenASome_RunsTheSomeFunction()
    {
        int res = await Task.FromResult(Some("Fred")).Match((name) => name.Length, () => 0);
        Assert.Equal(4, res);
    }


    [Fact]
    public async Task Match_Task_WhenGivenANone_RunsTheNoneFunction()
    {
        int res = await Task.FromResult(NoneE<string>()).Match((name) => name.Length, () => 0);
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
        await Task.FromResult(NoneE<string>()).Match(
            calls.Add,
            () => calls.Add("none")
        );
        Assert.Equal(new[] { "none" }, calls);
    }


    // ── MatchAsync (non-Task input — async callbacks) ─────────────────────

    [Fact]
    public async Task MatchAsync_NonTaskInput_WhenGivenASome_RunsTheSomeFunction()
    {
        int res = await Some("Fred").MatchAsync(
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
    public async Task MatchAsync_NonTaskInput_WhenGivenANone_RunsTheNoneFunction()
    {
        int res = await NoneE<string>().MatchAsync(
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
    public async Task MatchAsync_NonTaskInput_Action_WhenGivenASome_RunsTheSomeAction()
    {
        var calls = new List<string>();
        await Some("Fred").MatchAsync(
            async (name) =>
            {
                await Task.Delay(1);
                calls.Add(name);
            },
            async () =>
            {
                await Task.Delay(1);
                calls.Add("none");
            }
        );
        Assert.Equal(new[] { "Fred" }, calls);
    }


    [Fact]
    public async Task MatchAsync_NonTaskInput_Action_WhenGivenANone_RunsTheNoneAction()
    {
        var calls = new List<string>();
        await NoneE<string>().MatchAsync(
            async (name) =>
            {
                await Task.Delay(1);
                calls.Add(name);
            },
            async () =>
            {
                await Task.Delay(1);
                calls.Add("none");
            }
        );
        Assert.Equal(new[] { "none" }, calls);
    }


    // ── MatchAsync (Tier 3 — async callbacks) ─────────────────────────────

    [Fact]
    public async Task MatchAsync_WhenGivenASome_RunsTheSomeFunction()
    {
        int res = await Task.FromResult(Some("Fred")).MatchAsync(
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
        int res = await Task.FromResult(NoneE<string>()).MatchAsync(
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
        string captured = "";
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
        string captured = "";
        await Task.FromResult(NoneE<string>()).MatchAsync(
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
    // ── RequireNonEmpty ───────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void RequireNonEmpty_WhenGivenNonEmptyList_ReturnsSomeWithSameList()
    {
        int[] input = new[] { 1, 2, 3 };
        Option<IReadOnlyList<int>> result = input.RequireNonEmpty();
        Assert.Same(input, result.ThrowIfNone());
    }


    [Fact]
    public void RequireNonEmpty_WhenGivenEmptyList_ReturnsNone()
    {
        var input = new List<int>();
        Assert.True(input.RequireNonEmpty().IsNone);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── RequireSingle ─────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void RequireSingle_WhenGivenOneElement_ReturnsSomeWithThatElement()
    {
        Assert.Equal(Some(42), new[] { 42 }.RequireSingle());
    }


    [Fact]
    public void RequireSingle_WhenGivenNoElements_ReturnsNone()
    {
        IEnumerable<int> empty = [];
        Assert.True(empty.RequireSingle().IsNone);
    }


    [Fact]
    public void RequireSingle_WhenGivenMultipleElements_ReturnsNone()
    {
        Assert.True(new[] { 1, 2 }.RequireSingle().IsNone);
    }


    [Fact]
    public void RequireSingle_EnumeratesAtMostTwoElements()
    {
        int numEnumerated = 0;
        IEnumerable<int> input = Enumerable.Range(1, int.MaxValue)
            .Select((x) =>
            {
                numEnumerated++;
                return x;
            });

        Option<int> result = input.RequireSingle();

        Assert.True(result.IsNone);
        Assert.Equal(2, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Select ────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Select_WhenSome_ReturnsSomeWithMappedValue()
    {
        Option<int> result = Some("Fred").Select((name) => name.Length);
        Assert.Equal(Some(4), result);
    }


    [Fact]
    public void Select_WhenNone_ReturnsNoneWithoutCallingSelector()
    {
        bool called = false;
        Option<int> result = NoneE<string>().Select((name) =>
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

        Option<int> result =
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
        Option<string> result = Some("Fred").SelectMany(
            (name) => Some(name.Length),
            (name, length) => $"{name}:{length}");
        Assert.Equal(Some("Fred:4"), result);
    }


    [Fact]
    public void SelectMany_WhenInputIsNone_ReturnsNoneWithoutCallingNextOrProject()
    {
        bool nextCalled = false;
        bool projectCalled = false;
        Option<string> result = NoneE<string>().SelectMany(
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
        bool projectCalled = false;
        Option<string> result = Some("Fred").SelectMany(
            (name) => NoneE<int>(),
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
        Option<string> result =
            from name in Some("Fred")
            from age in Some(42)
            from isAdmin in Some(true)
            select $"{name}/{age}/{isAdmin}";

        Assert.Equal(Some("Fred/42/True"), result);
    }


    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntaxAndMiddleIsNone_StopsAndReturnsNone()
    {
        bool lastCalled = false;

        Option<int> last()
        {
            lastCalled = true;
            return Some(3);
        }

        Option<int> result =
            from a in Some(1)
            from b in NoneE<int>()
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
            dict.TryGetValue(key, out TValue? value) ? Some(value) : none;

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
        IEnumerable<Option<int>> input = [];
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
        IEnumerable<Option<int>> input = new Option<int>[] { none, Some(1), Some(2) }
            .Select((opt) =>
            {
                numEnumerated++;
                return opt;
            });

        IEnumerable<int> somes = input.Somes();
        Assert.Equal(0, numEnumerated);

        Assert.Equal(1, somes.First());
        Assert.Equal(2, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── TapNone ───────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void TapNone_WhenNone_InvokesActionAndReturnsInput()
    {
        var calls = new List<string>();
        Option<int> opt = none;

        Option<int> output = opt.TapNone(() => calls.Add("none"));

        Assert.Same(opt, output);
        Assert.Equal(new[] { "none" }, calls);
    }


    [Fact]
    public void TapNone_WhenSome_DoesNotInvokeAction()
    {
        var calls = new List<string>();
        Option<int> opt = Some(1);

        Option<int> output = opt.TapNone(() => calls.Add("none"));

        Assert.Same(opt, output);
        Assert.Empty(calls);
    }


    // ── TapNone (Tier 2 lift — sync callback) ─────────────────────────────

    [Fact]
    public async Task TapNone_Task_WhenNone_InvokesAction()
    {
        var calls = new List<string>();
        Option<int> output = await Task.FromResult(NoneE<int>()).TapNone(() => calls.Add("none"));
        Assert.True(output.IsNone);
        Assert.Equal(new[] { "none" }, calls);
    }


    [Fact]
    public async Task TapNone_Task_WhenSome_DoesNotInvokeAction()
    {
        var calls = new List<string>();
        Option<int> output = await Task.FromResult(Some(1)).TapNone(() => calls.Add("none"));
        Assert.Equal(Some(1), output);
        Assert.Empty(calls);
    }


    // ── TapNoneAsync (async callback) ─────────────────────────────────────

    [Fact]
    public async Task TapNoneAsync_WhenNone_InvokesAction()
    {
        var calls = new List<string>();
        Option<int> opt = none;

        Option<int> output = await opt.TapNoneAsync(async () =>
        {
            await Task.Delay(1);
            calls.Add("none");
        });

        Assert.Same(opt, output);
        Assert.Equal(new[] { "none" }, calls);
    }


    [Fact]
    public async Task TapNoneAsync_WhenSome_DoesNotInvokeAction()
    {
        var calls = new List<string>();
        Option<int> opt = Some(1);

        Option<int> output = await opt.TapNoneAsync(async () =>
        {
            await Task.Delay(1);
            calls.Add("none");
        });

        Assert.Same(opt, output);
        Assert.Empty(calls);
    }


    [Fact]
    public async Task TapNoneAsync_Task_WhenNone_InvokesAction()
    {
        var calls = new List<string>();

        Option<int> output = await Task.FromResult(NoneE<int>()).TapNoneAsync(async () =>
        {
            await Task.Delay(1);
            calls.Add("none");
        });

        Assert.True(output.IsNone);
        Assert.Equal(new[] { "none" }, calls);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── TapSome ───────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void TapSome_WhenSome_InvokesActionWithValueAndReturnsInput()
    {
        var seen = new List<int>();
        Option<int> opt = Some(1);

        Option<int> output = opt.TapSome(seen.Add);

        Assert.Same(opt, output);
        Assert.Equal(new[] { 1 }, seen);
    }


    [Fact]
    public void TapSome_WhenNone_DoesNotInvokeAction()
    {
        var seen = new List<int>();
        Option<int> opt = none;

        Option<int> output = opt.TapSome(seen.Add);

        Assert.Same(opt, output);
        Assert.Empty(seen);
    }


    // ── TapSome (Tier 2 lift — sync callback) ─────────────────────────────

    [Fact]
    public async Task TapSome_Task_WhenSome_InvokesActionWithValue()
    {
        var seen = new List<int>();
        Option<int> output = await Task.FromResult(Some(1)).TapSome(seen.Add);
        Assert.Equal(Some(1), output);
        Assert.Equal(new[] { 1 }, seen);
    }


    [Fact]
    public async Task TapSome_Task_WhenNone_DoesNotInvokeAction()
    {
        var seen = new List<int>();
        Option<int> output = await Task.FromResult(NoneE<int>()).TapSome(seen.Add);
        Assert.True(output.IsNone);
        Assert.Empty(seen);
    }


    // ── TapSomeAsync (async callback) ─────────────────────────────────────

    [Fact]
    public async Task TapSomeAsync_WhenSome_InvokesActionWithValue()
    {
        var seen = new List<int>();
        Option<int> opt = Some(1);

        Option<int> output = await opt.TapSomeAsync(async (x) =>
        {
            await Task.Delay(1);
            seen.Add(x);
        });

        Assert.Same(opt, output);
        Assert.Equal(new[] { 1 }, seen);
    }


    [Fact]
    public async Task TapSomeAsync_WhenNone_DoesNotInvokeAction()
    {
        var seen = new List<int>();
        Option<int> opt = none;

        Option<int> output = await opt.TapSomeAsync(async (x) =>
        {
            await Task.Delay(1);
            seen.Add(x);
        });

        Assert.Same(opt, output);
        Assert.Empty(seen);
    }


    [Fact]
    public async Task TapSomeAsync_Task_WhenSome_InvokesActionWithValue()
    {
        var seen = new List<int>();

        Option<int> output = await Task.FromResult(Some(1)).TapSomeAsync(async (x) =>
        {
            await Task.Delay(1);
            seen.Add(x);
        });

        Assert.Equal(Some(1), output);
        Assert.Equal(new[] { 1 }, seen);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ThrowIfNone ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ThrowIfNone_WhenSome_ReturnsContainedValue()
    {
        Assert.Equal("Fred", Some("Fred").ThrowIfNone());
    }


    [Fact]
    public void ThrowIfNone_WhenNone_ThrowsWithDefaultMessage()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => NoneE<string>().ThrowIfNone());
        Assert.Equal("Expected Some Option but got None.", ex.Message);
    }


    [Fact]
    public void ThrowIfNone_WhenNone_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => NoneE<string>().ThrowIfNone("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }


    // ── ThrowIfNone (Tier 2 lift) ─────────────────────────────────────────

    [Fact]
    public async Task ThrowIfNone_Task_WhenSome_ReturnsContainedValue()
    {
        string val = await Task.FromResult(Some("Fred")).ThrowIfNone();
        Assert.Equal("Fred", val);
    }


    [Fact]
    public async Task ThrowIfNone_Task_WhenNone_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(NoneE<string>()).ThrowIfNone("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ThrowIfSome ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ThrowIfSome_WhenNone_DoesNotThrow()
    {
        Exception ex = Record.Exception(() => NoneE<string>().ThrowIfSome());
        Assert.Null(ex);
    }


    [Fact]
    public void ThrowIfSome_WhenSome_ThrowsWithDefaultMessage()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => Some("Fred").ThrowIfSome());
        Assert.Equal("Expected None Option but got Some: Fred", ex.Message);
    }


    [Fact]
    public void ThrowIfSome_WhenSome_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => Some("Fred").ThrowIfSome("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }


    [Fact]
    public void ThrowIfSome_MessageFn_WhenSome_ThrowsWithMessageFromValue()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => Some("Fred").ThrowIfSome((name) => $"Unexpected: {name}"));
        Assert.Equal("Unexpected: Fred", ex.Message);
    }


    [Fact]
    public void ThrowIfSome_MessageFn_WhenNone_DoesNotCallFunction()
    {
        bool called = false;
        NoneE<string>().ThrowIfSome((name) =>
        {
            called = true;
            return name;
        });
        Assert.False(called);
    }


    // ── ThrowIfSome (Tier 2 lift) ─────────────────────────────────────────

    [Fact]
    public async Task ThrowIfSome_Task_WhenSome_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(Some("Fred")).ThrowIfSome("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }


    [Fact]
    public async Task ThrowIfSome_Task_MessageFn_WhenSome_ThrowsWithMessageFromValue()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(Some("Fred")).ThrowIfSome((name) => $"Unexpected: {name}"));
        Assert.Equal("Unexpected: Fred", ex.Message);
    }


    [Fact]
    public async Task ThrowIfSome_Task_WhenNone_DoesNotThrow()
    {
        Exception ex = await Record.ExceptionAsync(async () => await Task.FromResult(NoneE<string>()).ThrowIfSome());
        Assert.Null(ex);
    }


    // ── ThrowIfSomeAsync (async message function) ─────────────────────────

    [Fact]
    public async Task ThrowIfSomeAsync_WhenSome_ThrowsWithMessageFromAsyncFunction()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Some("Fred").ThrowIfSomeAsync(async (name) =>
            {
                await Task.Delay(1);
                return $"Unexpected: {name}";
            }));
        Assert.Equal("Unexpected: Fred", ex.Message);
    }


    [Fact]
    public async Task ThrowIfSomeAsync_WhenNone_DoesNotCallFunction()
    {
        bool called = false;
        await NoneE<string>().ThrowIfSomeAsync(async (name) =>
        {
            called = true;
            await Task.Delay(1);
            return name;
        });
        Assert.False(called);
    }


    [Fact]
    public async Task ThrowIfSomeAsync_Task_WhenSome_ThrowsWithMessageFromAsyncFunction()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(Some("Fred")).ThrowIfSomeAsync(async (name) =>
            {
                await Task.Delay(1);
                return $"Unexpected: {name}";
            }));
        Assert.Equal("Unexpected: Fred", ex.Message);
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


    // ── ToOption (nullable value type) ────────────────────────────────────

    [Fact]
    public void ToOption_NullableValueType_WhenHasValue_ReturnsSomeOfUnderlyingType()
    {
        int? value = 5;
        Option<int> result = value.ToOption();
        Assert.Equal(Some(5), result);
    }


    [Fact]
    public void ToOption_NullableValueType_WhenNull_ReturnsNone()
    {
        int? value = null;
        Option<int> result = value.ToOption();
        Assert.True(result.IsNone);
    }


    // ── ToOption (bool) ───────────────────────────────────────────────────

    [Fact]
    public void ToOption_Bool_WhenTrue_ReturnsSomeValue()
    {
        bool isAdmin = true;
        Assert.Equal(Some("admin"), isAdmin.ToOption("admin"));
    }


    [Fact]
    public void ToOption_Bool_WhenFalse_ReturnsNone()
    {
        bool isAdmin = false;
        Assert.True(isAdmin.ToOption("admin").IsNone);
    }
}
