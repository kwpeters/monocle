//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

using System.Globalization;

namespace FnUtil.Tests;


[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity", Justification = "Unit tests not globalized.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Don't need to optimize unit tests. Clarity more important.")]
public class ResultExtTests
{
    //
    // ══════════════════════════════════════════════════════════════════════
    // ── AllOrErrors ──────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void AllOrErrors_WhenGivenEmptySequence_ReturnsSuccessWithEmptyCollection()
    {
        IEnumerable<Result<int, string>> input = [];

        Result<IEnumerable<int>, IEnumerable<string>> result = input.AllOrErrors();

        Assert.Empty(result.GetValue());
    }


    [Fact]
    public void AllOrErrors_WhenGivenAllSuccesses_ReturnsSuccessWithAllValuesInOrder()
    {
        var input = new Result<int, string>[] { Success(1), Success(2), Success(3) };

        Result<IEnumerable<int>, IEnumerable<string>> result = input.AllOrErrors();

        Assert.Equal(new[] { 1, 2, 3 }, result.GetValue());
    }


    [Fact]
    public void AllOrErrors_WhenGivenAllErrors_ReturnsErrorWithAllErrorsInOrder()
    {
        var input = new Result<int, string>[] { Error("err1"), Error("err2"), Error("err3") };

        Result<IEnumerable<int>, IEnumerable<string>> result = input.AllOrErrors();

        Assert.Equal(new[] { "err1", "err2", "err3" }, result.GetError());
    }


    [Fact]
    public void AllOrErrors_WhenGivenMixedResults_ReturnsErrorWithOnlyTheErrorsInOrder()
    {
        var input = new Result<int, string>[]
        {
            Success(1),
            Error("err1"),
            Success(2),
            Error("err2"),
            Success(3)
        };

        Result<IEnumerable<int>, IEnumerable<string>> result = input.AllOrErrors();

        Assert.Equal(new[] { "err1", "err2" }, result.GetError());
    }


    [Fact]
    public void AllOrErrors_WhenGivenAnError_StillEnumeratesEntireSequence()
    {
        int numEnumerated = 0;
        IEnumerable<Result<int, string>> input = new Result<int, string>[] { Error("err1"), Success(1), Error("err2") }
            .Select((r) =>
            {
                numEnumerated++;
                return r;
            });

        Result<IEnumerable<int>, IEnumerable<string>> result = input.AllOrErrors();

        Assert.True(result.IsError);
        Assert.Equal(3, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── AllOrFirstError ──────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void AllOrFirstError_WhenGivenEmptySequence_ReturnsSuccessWithEmptyCollection()
    {
        IEnumerable<Result<int, string>> input = [];

        Result<IEnumerable<int>, string> result = input.AllOrFirstError();

        Assert.Empty(result.GetValue());
    }


    [Fact]
    public void AllOrFirstError_WhenGivenAllSuccesses_ReturnsSuccessWithAllValuesInOrder()
    {
        var input = new Result<int, string>[] { Success(1), Success(2), Success(3) };

        Result<IEnumerable<int>, string> result = input.AllOrFirstError();

        Assert.Equal(new[] { 1, 2, 3 }, result.GetValue());
    }


    [Fact]
    public void AllOrFirstError_WhenGivenMixedResults_ReturnsFirstError()
    {
        var input = new Result<int, string>[]
        {
            Success(1),
            Error("err1"),
            Success(2),
            Error("err2")
        };

        Result<IEnumerable<int>, string> result = input.AllOrFirstError();

        Assert.Equal("err1", result.GetError());
    }


    [Fact]
    public void AllOrFirstError_WhenGivenAnError_StopsEnumeratingAtFirstError()
    {
        int numEnumerated = 0;
        IEnumerable<Result<int, string>> input = new Result<int, string>[] { Success(1), Error("err1"), Success(2), Error("err2") }
            .Select((r) =>
            {
                numEnumerated++;
                return r;
            });

        Result<IEnumerable<int>, string> result = input.AllOrFirstError();

        Assert.Equal("err1", result.GetError());
        Assert.Equal(2, numEnumerated);
    }


    // ── AllOrFirstError (tuples) ──────────────────────────────────────────

    [Fact]
    public void AllOrFirstError_Tuple_WhenAllSucceed_ReturnsTupleOfValuesOfDifferentTypes()
    {
        Result<int, string> count = Success(3);
        Result<string, string> name = Success("Fred");
        Result<bool, string> isAdmin = Success(true);

        Result<(int, string, bool), string> result = (count, name, isAdmin).AllOrFirstError();

        Assert.Equal((3, "Fred", true), result.GetValue());
    }


    [Fact]
    public void AllOrFirstError_Tuple_WhenSomeFail_ReturnsFirstErrorInTupleOrder()
    {
        Result<int, string> a = Success(1);
        Result<string, string> b = Error("err b");
        Result<bool, string> c = Error("err c");

        Result<(int, string, bool), string> result = (a, b, c).AllOrFirstError();

        Assert.Equal("err b", result.GetError());
    }


    [Fact]
    public void AllOrFirstError_Tuple_SupportsTwoThroughSevenElements()
    {
        Result<int, string> r = Success(1);
        Result<int, string> e = Error("last");

        Assert.Equal((1, 1), (r, r).AllOrFirstError().GetValue());
        Assert.Equal((1, 1, 1), (r, r, r).AllOrFirstError().GetValue());
        Assert.Equal((1, 1, 1, 1), (r, r, r, r).AllOrFirstError().GetValue());
        Assert.Equal((1, 1, 1, 1, 1), (r, r, r, r, r).AllOrFirstError().GetValue());
        Assert.Equal((1, 1, 1, 1, 1, 1), (r, r, r, r, r, r).AllOrFirstError().GetValue());
        Assert.Equal((1, 1, 1, 1, 1, 1, 1), (r, r, r, r, r, r, r).AllOrFirstError().GetValue());
        Assert.Equal("last", (r, r, r, r, r, r, e).AllOrFirstError().GetError());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Bind ─────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Bind_WhenInputIsError_DoesNotInvokeFunctionAndReturnsError()
    {
        int numInvocations = 0;

        Result<double, string> sqrt(double x)
        {
            numInvocations++;
            return x < 0 ?
                Error("Cannot take square root of negative number.") :
                Success(Math.Sqrt(x));
        }

        string err =
            ErrorE<double, string>("Initial error!")
            .Bind(sqrt)
            .ThrowIfSuccess();

        Assert.Equal(0, numInvocations);
        Assert.Equal("Initial error!", err);
    }


    [Fact]
    public void Bind_WhenInputIsSuccess_InvokesFunctionAndReturnsResult()
    {
        int numInvocations = 0;

        Result<double, string> sqrt(double x)
        {
            numInvocations++;
            return x < 0 ?
                Error("Cannot take square root of negative number.") :
                Success(Math.Sqrt(x));
        }

        double val =
            SuccessE<double, string>(4d)
            .Bind(sqrt)
            .ThrowIfError();

        Assert.Equal(1, numInvocations);
        Assert.Equal(2d, val);
    }


    // ── Bind (Tier 2 lift — sync callback) ───────────────────────────────

    [Fact]
    public async Task Bind_Task_WhenGivenSuccessResult_ChainsCorrectly()
    {
        Result<string, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .Bind((x) => SuccessE<string, string>($"Value: {x}"));
        string val = result.ThrowIfError();
        Assert.Equal("Value: 42", val);
    }


    [Fact]
    public async Task Bind_Task_WhenGivenErrorResult_PropagatesErrorWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<string, string> result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .Bind((x) => {
                fnCalled = true;
                return SuccessE<string, string>($"Value: {x}");
            });
        string err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }


    // ── BindAsync (non-Task input — async callback) ───────────────────────

    [Fact]
    public async Task BindAsync_NonTaskInput_WhenGivenSuccessResult_ChainsCorrectly()
    {
        Result<int, string> result = await SuccessE<int, string>(21).BindAsync(async (x) =>
        {
            await Task.Delay(1);
            return SuccessE<int, string>(x * 2);
        });
        Assert.Equal(42, result.GetValue());
    }


    [Fact]
    public async Task BindAsync_NonTaskInput_WhenGivenErrorResult_PropagatesErrorWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<int, string> result = await ErrorE<int, string>("Error message").BindAsync(async (x) =>
        {
            fnCalled = true;
            await Task.Delay(1);
            return SuccessE<int, string>(x * 2);
        });
        Assert.False(fnCalled);
        Assert.Equal("Error message", result.GetError());
    }


    // ── BindAsync (Tier 3 — async callback) ──────────────────────────────

    [Fact]
    public async Task BindAsync_WhenGivenSuccessResult_ChainsCorrectly()
    {
        Result<string, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .BindAsync(async (x) => {
                await Task.Delay(1);
                return SuccessE<string, string>($"Value: {x}");
            });
        string val = result.ThrowIfError();
        Assert.Equal("Value: 42", val);
    }


    [Fact]
    public async Task BindAsync_WhenGivenErrorResult_PropagatesErrorWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<string, string> result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .BindAsync(async (x) => {
                fnCalled = true;
                await Task.Delay(1);
                return SuccessE<string, string>($"Value: {x}");
            });
        string err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }


    [Fact]
    public async Task BindAsync_WhenFunctionReturnsError_PropagatesError()
    {
        Result<string, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .BindAsync(async (x) => {
                await Task.Delay(1);
                return ErrorE<string, string>("Function error");
            });
        string err = result.ThrowIfSuccess();
        Assert.Equal("Function error", err);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── BindError ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void BindError_WhenGivenErrorResult_ReturnsResultOfFunction()
    {
        Result<int, string> result = ErrorE<int, string>("primary failed").BindError((e) => SuccessE<int, string>(5));
        Assert.Equal(5, result.GetValue());
    }


    [Fact]
    public void BindError_WhenFunctionReturnsError_ReturnsNewErrorOfNewType()
    {
        Result<int, int> result = ErrorE<int, string>("primary failed").BindError((e) => ErrorE<int, int>(e.Length));
        Assert.Equal(14, result.GetError());
    }


    [Fact]
    public void BindError_WhenGivenSuccessResult_ReturnsSuccessWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<int, string> result = SuccessE<int, string>(42).BindError((e) =>
        {
            fnCalled = true;
            return SuccessE<int, string>(0);
        });
        Assert.False(fnCalled);
        Assert.Equal(42, result.GetValue());
    }


    // ── BindError (Tier 2 lift — sync callback) ───────────────────────────

    [Fact]
    public async Task BindError_Task_WhenGivenErrorResult_ReturnsResultOfFunction()
    {
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("primary failed"))
            .BindError((e) => SuccessE<int, string>(5));
        Assert.Equal(5, result.GetValue());
    }


    [Fact]
    public async Task BindError_Task_WhenGivenSuccessResult_ReturnsSuccessWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .BindError((e) =>
            {
                fnCalled = true;
                return SuccessE<int, string>(0);
            });
        Assert.False(fnCalled);
        Assert.Equal(42, result.GetValue());
    }


    // ── BindErrorAsync (async callback) ───────────────────────────────────

    [Fact]
    public async Task BindErrorAsync_WhenGivenErrorResult_ReturnsResultOfFunction()
    {
        Result<int, string> result = await ErrorE<int, string>("primary failed").BindErrorAsync(async (e) =>
        {
            await Task.Delay(1);
            return SuccessE<int, string>(e.Length);
        });
        Assert.Equal(14, result.GetValue());
    }


    [Fact]
    public async Task BindErrorAsync_WhenGivenSuccessResult_ReturnsSuccessWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<int, string> result = await SuccessE<int, string>(42).BindErrorAsync(async (e) =>
        {
            fnCalled = true;
            await Task.Delay(1);
            return SuccessE<int, string>(0);
        });
        Assert.False(fnCalled);
        Assert.Equal(42, result.GetValue());
    }


    [Fact]
    public async Task BindErrorAsync_Task_WhenGivenErrorResult_ReturnsResultOfFunction()
    {
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("primary failed"))
            .BindErrorAsync(async (e) =>
            {
                await Task.Delay(1);
                return SuccessE<int, string>(e.Length);
            });
        Assert.Equal(14, result.GetValue());
    }


    [Fact]
    public async Task BindErrorAsync_Task_WhenGivenSuccessResult_ReturnsSuccessWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .BindErrorAsync(async (e) =>
            {
                fnCalled = true;
                await Task.Delay(1);
                return SuccessE<int, string>(0);
            });
        Assert.False(fnCalled);
        Assert.Equal(42, result.GetValue());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Contains ──────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Contains_WhenSuccessAndValueMatches_ReturnsTrue()
    {
        Result<int, string> input = Success(42);

        bool result = input.Contains(42);

        Assert.True(result);
    }


    [Fact]
    public void Contains_WhenSuccessAndValueDoesNotMatch_ReturnsFalse()
    {
        Result<int, string> input = Success(42);

        bool result = input.Contains(99);

        Assert.False(result);
    }


    [Fact]
    public void Contains_WhenError_ReturnsFalse()
    {
        Result<int, string> input = Error("failed");

        bool result = input.Contains(42);

        Assert.False(result);
    }


    // ── Contains (Tier 2 lift) ────────────────────────────────────────────

    [Fact]
    public async Task Contains_Task_WhenSuccessAndValueMatches_ReturnsTrue()
    {
        Result<int, string> successResult = Success(42);
        Task<Result<int, string>> input = Task.FromResult(successResult);

        bool result = await input.Contains(42);

        Assert.True(result);
    }


    [Fact]
    public async Task Contains_Task_WhenError_ReturnsFalse()
    {
        Result<int, string> errorResult = Error("failed");
        Task<Result<int, string>> input = Task.FromResult(errorResult);

        bool result = await input.Contains(42);

        Assert.False(result);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── DefaultValue ─────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void DefaultValue_WhenInputIsSuccess_ReturnsSuccessValue()
    {
        int outVal =
            SuccessE<int, string>(5)
            .DefaultValue(3);

        Assert.Equal(5, outVal);
    }


    [Fact]
    public void DefaultValue_WhenInputIsError_ReturnsDefaultValue()
    {
        int outVal =
            ErrorE<int, string>("Error message")
            .DefaultValue(3);

        Assert.Equal(3, outVal);
    }


    // ── DefaultValue (Tier 2 lift) ───────────────────────────────────────

    [Fact]
    public async Task DefaultValue_Task_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        int val = await Task.FromResult(SuccessE<int, string>(42)).DefaultValue(0);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task DefaultValue_Task_WhenGivenErrorResult_ReturnsDefaultValue()
    {
        int val = await Task.FromResult(ErrorE<int, string>("Error message")).DefaultValue(0);
        Assert.Equal(0, val);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── DefaultWith ──────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void DefaultWith_WhenInputIsSuccess_DoesNotInvokeFuncAndReturnsSuccessValue()
    {
        int numInvocations = 0;

        int outVal =
            SuccessE<int, string>(5)
            .DefaultWith((e) => {
                numInvocations++;
                return 10;
            });

        // Then
        Assert.Equal(5, outVal);
        Assert.Equal(0, numInvocations);
    }


    [Fact]
    public void DefaultWith_WhenInputIsError_InvokesFuncAndReturnsReturnedValue()
    {
        int numInvocations = 0;

        int outVal =
            ErrorE<int, string>("Error")
            .DefaultWith((e) => {
                numInvocations++;
                return 10;
            });

        Assert.Equal(10, outVal);
        Assert.Equal(1, numInvocations);
    }


    // ── DefaultWith (Tier 2 lift — sync callback) ────────────────────────

    [Fact]
    public async Task DefaultWith_Task_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        bool fnCalled = false;
        int val = await Task.FromResult(SuccessE<int, string>(42))
            .DefaultWith((err) => {
                fnCalled = true;
                return 0;
            });
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task DefaultWith_Task_WhenGivenErrorResult_ComputesDefaultFromError()
    {
        int val = await Task.FromResult(ErrorE<int, string>("Error message"))
            .DefaultWith((err) => err.Length);
        Assert.Equal(13, val);
    }


    // ── DefaultWithAsync (Tier 3 — async callback) ───────────────────────

    [Fact]
    public async Task DefaultWithAsync_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        bool fnCalled = false;
        int val = await Task.FromResult(SuccessE<int, string>(42))
            .DefaultWithAsync(async (err) => {
                fnCalled = true;
                await Task.Delay(1);
                return 0;
            });
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task DefaultWithAsync_WhenGivenErrorResult_ComputesDefaultFromError()
    {
        int val = await Task.FromResult(ErrorE<int, string>("Error message"))
            .DefaultWithAsync(async (err) => {
                await Task.Delay(1);
                return err.Length;
            });
        Assert.Equal(13, val);
    }


    [Fact]
    public async Task DefaultWithAsync_SyncResult_WhenGivenSuccessResult_ReturnsSuccessValueWithoutCallingFunction()
    {
        bool fnCalled = false;
        int val = await SuccessE<int, string>(42).DefaultWithAsync(async (err) => {
            fnCalled = true;
            await Task.Delay(1);
            return 0;
        });
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task DefaultWithAsync_SyncResult_WhenGivenErrorResult_ComputesDefaultFromError()
    {
        int val = await ErrorE<int, string>("Error message").DefaultWithAsync(async (err) => {
            await Task.Delay(1);
            return err.Length;
        });
        Assert.Equal(13, val);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Errors ────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Errors_WhenGivenEmptySequence_ReturnsEmptySequence()
    {
        IEnumerable<Result<int, string>> input = [];
        Assert.Empty(input.Errors());
    }


    [Fact]
    public void Errors_WhenGivenMixedResults_ReturnsOnlyErrorsInOrder()
    {
        var input = new Result<int, string>[] { Success(1), Error("err1"), Success(2), Error("err2") };
        Assert.Equal(new[] { "err1", "err2" }, input.Errors());
    }


    [Fact]
    public void Errors_IsEvaluatedLazily()
    {
        int numEnumerated = 0;
        IEnumerable<Result<int, string>> input = new Result<int, string>[] { Success(1), Error("err1"), Error("err2") }
            .Select((r) =>
            {
                numEnumerated++;
                return r;
            });

        IEnumerable<string> errors = input.Errors();
        Assert.Equal(0, numEnumerated);

        Assert.Equal("err1", errors.First());
        Assert.Equal(2, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ExecuteWhileSuccessful ────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ExecuteWhileSuccessful_WhenGivenNoFunctions_ReturnsSuccessWithEmptyCollection()
    {
        IEnumerable<Func<Result<int, string>>> fns = [];
        Assert.Empty(fns.ExecuteWhileSuccessful().GetValue());
    }


    [Fact]
    public void ExecuteWhileSuccessful_WhenAllSucceed_ReturnsAllValuesInOrder()
    {
        var fns = new Func<Result<int, string>>[]
        {
            () => Success(1),
            () => Success(2),
            () => Success(3)
        };

        Result<IEnumerable<int>, string> result = fns.ExecuteWhileSuccessful();

        Assert.Equal(new[] { 1, 2, 3 }, result.GetValue());
    }


    [Fact]
    public void ExecuteWhileSuccessful_WhenOneFails_ReturnsItsErrorAndDoesNotInvokeTheRest()
    {
        var calls = new List<string>();
        var fns = new Func<Result<int, string>>[]
        {
            () => { calls.Add("a"); return Success(1); },
            () => { calls.Add("b"); return Error("err b"); },
            () => { calls.Add("c"); return Success(3); }
        };

        Result<IEnumerable<int>, string> result = fns.ExecuteWhileSuccessful();

        Assert.Equal("err b", result.GetError());
        Assert.Equal(new[] { "a", "b" }, calls);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Exists ────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Exists_WhenSuccessAndPredicateMatches_ReturnsTrue()
    {
        Result<int, string> input = Success(42);

        bool result = input.Exists((x) => x > 40);

        Assert.True(result);
    }


    [Fact]
    public void Exists_WhenSuccessAndPredicateDoesNotMatch_ReturnsFalse()
    {
        Result<int, string> input = Success(42);

        bool result = input.Exists((x) => x > 50);

        Assert.False(result);
    }


    [Fact]
    public void Exists_WhenError_ReturnsFalse()
    {
        Result<int, string> input = Error("failed");

        bool result = input.Exists((x) => x > 40);

        Assert.False(result);
    }


    // ── Exists (Tier 2 lift — sync callback) ──────────────────────────────

    [Fact]
    public async Task Exists_Task_WhenSuccessAndPredicateMatches_ReturnsTrue()
    {
        Result<int, string> successResult = Success(42);
        Task<Result<int, string>> input = Task.FromResult(successResult);

        bool result = await input.Exists((x) => x > 40);

        Assert.True(result);
    }


    [Fact]
    public async Task Exists_Task_WhenError_ReturnsFalse()
    {
        Result<int, string> errorResult = Error("failed");
        Task<Result<int, string>> input = Task.FromResult(errorResult);

        bool result = await input.Exists((x) => x > 40);

        Assert.False(result);
    }


    // ── ExistsAsync (non-Task input — async callback) ─────────────────────

    [Fact]
    public async Task ExistsAsync_WhenSuccessAndAsyncPredicateMatches_ReturnsTrue()
    {
        Result<int, string> input = Success(42);

        bool result = await input.ExistsAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 40;
        });

        Assert.True(result);
    }


    [Fact]
    public async Task ExistsAsync_WhenSuccessAndAsyncPredicateDoesNotMatch_ReturnsFalse()
    {
        Result<int, string> input = Success(42);

        bool result = await input.ExistsAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 50;
        });

        Assert.False(result);
    }


    [Fact]
    public async Task ExistsAsync_WhenError_ReturnsFalse()
    {
        Result<int, string> input = Error("failed");

        bool result = await input.ExistsAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 40;
        });

        Assert.False(result);
    }


    [Fact]
    public async Task ExistsAsync_WhenErrorAndAsyncPredicate_DoesNotInvokePredicate()
    {
        Result<int, string> input = Error("failed");
        bool predicateInvoked = false;

        bool result = await input.ExistsAsync(async (x) =>
        {
            predicateInvoked = true;
            await Task.Delay(1);
            return x > 40;
        });

        Assert.False(result);
        Assert.False(predicateInvoked);
    }


    // ── ExistsAsync (Tier 3 — async callback) ─────────────────────────────

    [Fact]
    public async Task ExistsAsync_Task_WhenSuccessAndAsyncPredicate_ReturnsTrue()
    {
        Result<int, string> successResult = Success(42);
        Task<Result<int, string>> input = Task.FromResult(successResult);

        bool result = await input.ExistsAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 40;
        });

        Assert.True(result);
    }


    [Fact]
    public async Task ExistsAsync_Task_WhenErrorAndAsyncPredicate_ReturnsFalse()
    {
        Result<int, string> errorResult = Error("failed");
        Task<Result<int, string>> input = Task.FromResult(errorResult);

        bool result = await input.ExistsAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 40;
        });

        Assert.False(result);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Fold ──────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Fold_WhenSuccess_FoldsValueIntoState()
    {
        Result<int, string> input = Success(5);

        int result = input.Fold((state, value) => state + value, 10);

        Assert.Equal(15, result);
    }


    [Fact]
    public void Fold_WhenError_ReturnsInitialState()
    {
        Result<int, string> input = Error("failed");

        int result = input.Fold((state, value) => state + value, 10);

        Assert.Equal(10, result);
    }


    [Fact]
    public void Fold_WhenSuccessWithComplexAccumulation_WorksCorrectly()
    {
        Result<string, string> input = Success("hello");

        string result = input.Fold((state, value) => state + " " + value, "start");

        Assert.Equal("start hello", result);
    }


    // ── Fold (Tier 2 lift — sync callback) ────────────────────────────────

    [Fact]
    public async Task Fold_Task_WhenSuccess_FoldsValueIntoState()
    {
        Result<int, string> successResult = Success(5);
        Task<Result<int, string>> input = Task.FromResult(successResult);

        int result = await input.Fold((state, value) => state + value, 10);

        Assert.Equal(15, result);
    }


    [Fact]
    public async Task Fold_Task_WhenError_ReturnsInitialState()
    {
        Result<int, string> errorResult = Error("failed");
        Task<Result<int, string>> input = Task.FromResult(errorResult);

        int result = await input.Fold((state, value) => state + value, 10);

        Assert.Equal(10, result);
    }


    // ── FoldAsync (non-Task input — async callback) ───────────────────────

    [Fact]
    public async Task FoldAsync_WhenSuccessAndAsyncFolder_FoldsValueIntoState()
    {
        Result<int, string> input = Success(5);

        int result = await input.FoldAsync(async (state, value) =>
        {
            await Task.Delay(1);
            return state + value;
        }, 10);

        Assert.Equal(15, result);
    }


    [Fact]
    public async Task FoldAsync_WhenErrorAndAsyncFolder_ReturnsInitialState()
    {
        Result<int, string> input = Error("failed");

        int result = await input.FoldAsync(async (state, value) =>
        {
            await Task.Delay(1);
            return state + value;
        }, 10);

        Assert.Equal(10, result);
    }


    [Fact]
    public async Task FoldAsync_WhenErrorAndAsyncFolder_DoesNotInvokeFolder()
    {
        Result<int, string> input = Error("failed");
        bool folderInvoked = false;

        int result = await input.FoldAsync(async (state, value) =>
        {
            folderInvoked = true;
            await Task.Delay(1);
            return state + value;
        }, 10);

        Assert.Equal(10, result);
        Assert.False(folderInvoked);
    }


    // ── FoldAsync (Tier 3 — async callback) ───────────────────────────────

    [Fact]
    public async Task FoldAsync_Task_WhenSuccessAndAsyncFolder_FoldsValueIntoState()
    {
        Result<int, string> successResult = Success(5);
        Task<Result<int, string>> input = Task.FromResult(successResult);

        int result = await input.FoldAsync(async (state, value) =>
        {
            await Task.Delay(1);
            return state + value;
        }, 10);

        Assert.Equal(15, result);
    }


    [Fact]
    public async Task FoldAsync_Task_WhenErrorAndAsyncFolder_ReturnsInitialState()
    {
        Result<int, string> errorResult = Error("failed");
        Task<Result<int, string>> input = Task.FromResult(errorResult);

        int result = await input.FoldAsync(async (state, value) =>
        {
            await Task.Delay(1);
            return state + value;
        }, 10);

        Assert.Equal(10, result);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Gate ─────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Gate_WhenInputIsError_GateFnIsNotInvokedAndErrorResultIsReturned()
    {
        int numInvocations = 0;

        string err =
            ErrorE<int, string>("Initial error")
            .Gate(s => {
                numInvocations++;
                Result<int, string> r = Success(3);
                return r;
            })
            .ThrowIfSuccess();

        Assert.Equal(0, numInvocations);
        Assert.Equal("Initial error", err);
    }


    [Fact]
    public void Gate_WhenInputIsSuccess_AndGateFnReturnsSuccess_ReturnsOriginalSuccess()
    {
        int numInvocations = 0;

        int val =
            SuccessE<int, string>(1)
            .Gate(s => {
                numInvocations++;
                Result<int, string> r = Success(2);
                return r;
            })
            .ThrowIfError();

        Assert.Equal(1, numInvocations);
        Assert.Equal(1, val);
    }


    [Fact]
    public void Gate_WhenInputIsSuccess_AndGateFnReturnsError_ReturnsGateFnError()
    {
        int numInvocations = 0;

        string err =
            SuccessE<int, string>(1)
            .Gate(s => {
                numInvocations++;
                Result<int, string> r = Error("error");
                return r;
            })
            .ThrowIfSuccess();

        Assert.Equal(1, numInvocations);
        Assert.Equal("error", err);
    }


    // ── Gate (Tier 2 lift — sync callback) ───────────────────────────────

    [Fact]
    public async Task Gate_Task_WhenInputSuccessAndGateSucceeds_PreservesOriginalValue()
    {
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .Gate((x) => {
                Result<bool, string> r = x > 0 ?
                    SuccessE<bool, string>(true) :
                    ErrorE<bool, string>("Validation failed");
                return r;
            });
        int val = result.ThrowIfError();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task Gate_Task_WhenInputSuccessAndGateFails_PropagatesGateError()
    {
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .Gate((x) => {
                Result<bool, string> r = x < 0 ?
                    SuccessE<bool, string>(true) :
                    ErrorE<bool, string>("Gate validation failed");
                return r;
            });
        string err = result.ThrowIfSuccess();
        Assert.Equal("Gate validation failed", err);
    }


    [Fact]
    public async Task Gate_Task_WhenInputError_PropagatesErrorWithoutCallingGate()
    {
        bool gateCalled = false;
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("Input error"))
            .Gate((x) => {
                gateCalled = true;
                Result<bool, string> r = SuccessE<bool, string>(true);
                return r;
            });
        string err = result.ThrowIfSuccess();
        Assert.False(gateCalled);
        Assert.Equal("Input error", err);
    }


    // ── GateAsync (non-Task input — async callback) ───────────────────────

    [Fact]
    public async Task GateAsync_NonTaskInput_WhenGateSucceeds_ReturnsOriginalSuccess()
    {
        Result<int, string> result = await SuccessE<int, string>(10).GateAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 5 ? SuccessE<bool, string>(true) : ErrorE<bool, string>("too small");
        });
        Assert.Equal(10, result.GetValue());
    }


    [Fact]
    public async Task GateAsync_NonTaskInput_WhenGateFails_ReturnsGateError()
    {
        Result<int, string> result = await SuccessE<int, string>(3).GateAsync(async (x) =>
        {
            await Task.Delay(1);
            return x > 5 ? SuccessE<bool, string>(true) : ErrorE<bool, string>("too small");
        });
        Assert.Equal("too small", result.GetError());
    }


    // ── GateAsync (Tier 3 — async callback) ──────────────────────────────

    [Fact]
    public async Task GateAsync_WhenInputSuccessAndGateSucceeds_PreservesOriginalValue()
    {
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .GateAsync(async (x) => {
                await Task.Delay(1);
                return x > 0 ?
                    SuccessE<bool, string>(true) :
                    ErrorE<bool, string>("Validation failed");
            });
        int val = result.ThrowIfError();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task GateAsync_WhenInputSuccessAndGateFails_PropagatesGateError()
    {
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .GateAsync(async (x) => {
                await Task.Delay(1);
                return x < 0 ?
                    SuccessE<bool, string>(true) :
                    ErrorE<bool, string>("Gate validation failed");
            });
        string err = result.ThrowIfSuccess();
        Assert.Equal("Gate validation failed", err);
    }


    [Fact]
    public async Task GateAsync_WhenInputError_PropagatesErrorWithoutCallingGate()
    {
        bool gateCalled = false;
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("Input error"))
            .GateAsync(async (x) => {
                gateCalled = true;
                await Task.Delay(1);
                return SuccessE<bool, string>(true);
            });
        string err = result.ThrowIfSuccess();
        Assert.False(gateCalled);
        Assert.Equal("Input error", err);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── GetError ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void GetError_WhenGivenErrorResult_ReturnsErrorValue()
    {
        string err = ErrorE<int, string>("Error message").GetError();
        Assert.Equal("Error message", err);
    }


    [Fact]
    public void GetError_WhenGivenSuccessResult_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => SuccessE<int, string>(42).GetError());
    }


    [Fact]
    public async Task GetError_Task_WhenGivenErrorResult_ReturnsErrorValue()
    {
        string err = await Task.FromResult(ErrorE<int, string>("Error message")).GetError();
        Assert.Equal("Error message", err);
    }


    [Fact]
    public async Task GetError_Task_WhenGivenSuccessResult_ThrowsException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).GetError());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── GetValue ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void GetValue_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        int val = SuccessE<int, string>(42).GetValue();
        Assert.Equal(42, val);
    }


    [Fact]
    public void GetValue_WhenGivenErrorResult_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => ErrorE<int, string>("Error message").GetValue());
    }


    [Fact]
    public async Task GetValue_Task_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        int val = await Task.FromResult(SuccessE<int, string>(42)).GetValue();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task GetValue_Task_WhenGivenErrorResult_ThrowsException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(ErrorE<int, string>("Error message")).GetValue());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── MapError ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void MapError_WhenInputIsSuccess_DoesNotInvokeFunctionAndReturnsSuccess()
    {
        int numInvocations = 0;

        string appendText(string err)
        {
            numInvocations++;
            return err + " (modified)";
        }

        int val =
            SuccessE<int, string>(42)
            .MapError(appendText)
            .ThrowIfError();

        Assert.Equal(0, numInvocations);
        Assert.Equal(42, val);
    }


    [Fact]
    public void MapError_WhenInputIsError_InvokesFunctionAndReturnsErrorWithMappedValue()
    {
        int numInvocations = 0;

        string appendText(string err)
        {
            numInvocations++;
            return err + " (modified)";
        }

        string err =
            ErrorE<int, string>("Original error")
            .MapError(appendText)
            .ThrowIfSuccess();

        Assert.Equal(1, numInvocations);
        Assert.Equal("Original error (modified)", err);
    }


    [Fact]
    public void MapError_WhenMappingTypeChanges_ReturnsCorrectType()
    {
        int errVal =
            ErrorE<int, string>("Error message")
            .MapError(err => err.Length)
            .ThrowIfSuccess();

        Assert.Equal(13, errVal);
    }


    [Fact]
    public void MapError_WhenChained_AppliesTransformationsInOrder()
    {
        int errVal =
            ErrorE<int, string>("error")
            .MapError(err => err.ToUpper(CultureInfo.InvariantCulture))  // "ERROR"
            .MapError(err => err + "!")               // "ERROR!"
            .MapError(err => err.Length)              // 6
            .ThrowIfSuccess();

        Assert.Equal(6, errVal);
    }


    [Fact]
    public void MapError_WhenChainedWithSuccess_StopsAtFirstSuccess()
    {
        int invocations = 0;

        int val =
            SuccessE<int, string>(42)
            .MapError(err => { invocations++; return err.ToUpper(CultureInfo.InvariantCulture); })
            .MapError(err => { invocations++; return err + "!"; })
            .MapError(err => { invocations++; return err.Length; })
            .ThrowIfError();

        Assert.Equal(0, invocations);
        Assert.Equal(42, val);
    }


    // ── MapError (Tier 2 lift — sync callback) ───────────────────────────

    [Fact]
    public async Task MapError_Task_WhenGivenSuccessResult_PropagatesSuccess()
    {
        bool fnCalled = false;
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .MapError((err) => {
                fnCalled = true;
                return $"Transformed: {err}";
            });
        int val = result.ThrowIfError();
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task MapError_Task_WhenGivenErrorResult_TransformsError()
    {
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapError((err) => $"Transformed: {err}");
        string err = result.ThrowIfSuccess();
        Assert.Equal("Transformed: Error message", err);
    }


    // ── MapErrorAsync (non-Task input — async callback) ───────────────────

    [Fact]
    public async Task MapErrorAsync_NonTaskInput_WhenGivenErrorResult_MapsError()
    {
        Result<int, int> result = await ErrorE<int, string>("abc").MapErrorAsync(async (e) =>
        {
            await Task.Delay(1);
            return e.Length;
        });
        Assert.Equal(3, result.GetError());
    }


    [Fact]
    public async Task MapErrorAsync_NonTaskInput_WhenGivenSuccessResult_PassesThroughWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<int, int> result = await SuccessE<int, string>(42).MapErrorAsync(async (e) =>
        {
            fnCalled = true;
            await Task.Delay(1);
            return e.Length;
        });
        Assert.False(fnCalled);
        Assert.Equal(42, result.GetValue());
    }


    // ── MapErrorAsync (Tier 3 — async callback) ──────────────────────────

    [Fact]
    public async Task MapErrorAsync_WhenGivenSuccessResult_PropagatesSuccess()
    {
        bool fnCalled = false;
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .MapErrorAsync(async (err) => {
                fnCalled = true;
                await Task.Delay(1);
                return $"Transformed: {err}";
            });
        int val = result.ThrowIfError();
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task MapErrorAsync_WhenGivenErrorResult_TransformsError()
    {
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapErrorAsync(async (err) => {
                await Task.Delay(1);
                return $"Transformed: {err}";
            });
        string err = result.ThrowIfSuccess();
        Assert.Equal("Transformed: Error message", err);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── MapSuccess ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void MapSuccess_WhenInputIsError_DoesNotInvokeFunctionAndReturnsError()
    {
        int numInvocations = 0;

        int doubleValue(int x)
        {
            numInvocations++;
            return x * 2;
        }

        string err =
            ErrorE<int, string>("Initial error!")
            .MapSuccess(doubleValue)
            .ThrowIfSuccess();

        Assert.Equal(0, numInvocations);
        Assert.Equal("Initial error!", err);
    }


    [Fact]
    public void MapSuccess_WhenInputIsSuccess_InvokesFunctionAndReturnsSuccessWithMappedValue()
    {
        int numInvocations = 0;

        int doubleValue(int x)
        {
            numInvocations++;
            return x * 2;
        }

        int val =
            SuccessE<int, string>(5)
            .MapSuccess(doubleValue)
            .ThrowIfError();

        Assert.Equal(1, numInvocations);
        Assert.Equal(10, val);
    }


    [Fact]
    public void MapSuccess_WhenMappingTypeChanges_ReturnsCorrectType()
    {
        string val =
            SuccessE<int, string>(42)
            .MapSuccess(x => $"Value: {x}")
            .ThrowIfError();

        Assert.Equal("Value: 42", val);
    }


    [Fact]
    public void MapSuccess_WhenChained_AppliesTransformationsInOrder()
    {
        string val =
            SuccessE<int, string>(3)
            .MapSuccess(x => x * 2)      // 3 * 2 = 6
            .MapSuccess(x => x + 10)     // 6 + 10 = 16
            .MapSuccess(x => x.ToString(CultureInfo.InvariantCulture))
            .ThrowIfError();

        Assert.Equal("16", val);
    }


    [Fact]
    public void MapSuccess_WhenChainedWithError_StopsAtFirstError()
    {
        int invocations = 0;

        string err =
            ErrorE<int, string>("Initial error")
            .MapSuccess(x => { invocations++; return x * 2; })
            .MapSuccess(x => { invocations++; return x + 10; })
            .MapSuccess(x => { invocations++; return x.ToString(CultureInfo.InvariantCulture); })
            .ThrowIfSuccess();

        Assert.Equal(0, invocations);
        Assert.Equal("Initial error", err);
    }


    // ── MapSuccess (Tier 2 lift — sync callback) ─────────────────────────

    [Fact]
    public async Task MapSuccess_Task_WhenGivenSuccessResult_TransformsValue()
    {
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .MapSuccess((x) => x * 2);
        int val = result.ThrowIfError();
        Assert.Equal(84, val);
    }


    [Fact]
    public async Task MapSuccess_Task_WhenGivenErrorResult_PropagatesError()
    {
        bool fnCalled = false;
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapSuccess((x) => {
                fnCalled = true;
                return x * 2;
            });
        string err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }


    // ── MapSuccessAsync (non-Task input — async callback) ─────────────────

    [Fact]
    public async Task MapSuccessAsync_NonTaskInput_WhenGivenSuccessResult_MapsValue()
    {
        Result<int, string> result = await SuccessE<int, string>(21).MapSuccessAsync(async (x) =>
        {
            await Task.Delay(1);
            return x * 2;
        });
        Assert.Equal(42, result.GetValue());
    }


    [Fact]
    public async Task MapSuccessAsync_NonTaskInput_WhenGivenErrorResult_PassesThroughWithoutCallingFunction()
    {
        bool fnCalled = false;
        Result<int, string> result = await ErrorE<int, string>("Error message").MapSuccessAsync(async (x) =>
        {
            fnCalled = true;
            await Task.Delay(1);
            return x * 2;
        });
        Assert.False(fnCalled);
        Assert.Equal("Error message", result.GetError());
    }


    // ── MapSuccessAsync (Tier 3 — async callback) ────────────────────────

    [Fact]
    public async Task MapSuccessAsync_WhenGivenSuccessResult_TransformsValue()
    {
        Result<int, string> result = await Task.FromResult(SuccessE<int, string>(42))
            .MapSuccessAsync(async (x) => {
                await Task.Delay(1);
                return x * 2;
            });
        int val = result.ThrowIfError();
        Assert.Equal(84, val);
    }


    [Fact]
    public async Task MapSuccessAsync_WhenGivenErrorResult_PropagatesError()
    {
        bool fnCalled = false;
        Result<int, string> result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapSuccessAsync(async (x) => {
                fnCalled = true;
                await Task.Delay(1);
                return x * 2;
            });
        string err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── MapWhileSuccessful ────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void MapWhileSuccessful_WhenAllSucceed_ReturnsAllMappedValuesInOrder()
    {
        static Result<int, string> tenTimesPositive(int x) =>
            x > 0 ? Success(x * 10) : Error($"{x} is not positive");

        Result<IEnumerable<int>, string> result = new[] { 1, 2, 3 }.MapWhileSuccessful(tenTimesPositive);

        Assert.Equal(new[] { 10, 20, 30 }, result.GetValue());
    }


    [Fact]
    public void MapWhileSuccessful_WhenOneFails_ReturnsItsErrorAndStopsMapping()
    {
        var mapped = new List<int>();

        Result<int, string> tenTimesPositive(int x)
        {
            mapped.Add(x);
            return x > 0 ? Success(x * 10) : Error($"{x} is not positive");
        }

        Result<IEnumerable<int>, string> result = new[] { 1, -2, 3, -4 }.MapWhileSuccessful(tenTimesPositive);

        Assert.Equal("-2 is not positive", result.GetError());
        Assert.Equal(new[] { 1, -2 }, mapped);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Match ────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Match_WhenGivenASuccessResult_RunsTheSuccessFunction()
    {
        int val =
            SuccessE<int, string>(5)
            .Match(
                successFn: (_) => 1,
                errorFn: (_) => 0
            );

        Assert.Equal(1, val);
    }


    [Fact]
    public void Match_WhenGivenAnErrorResult_RunsTheErrorFunction()
    {
        int val =
            ErrorE<int, string>("Error message.")
            .Match(
                successFn: (_) => 1,
                errorFn: (_) => 0
            );

        Assert.Equal(0, val);
    }


    // The callbacks in the VoidAction tests below call List.Add, which returns
    // void, so they can only bind to the Action overloads.  A callback such as
    // (s) => called = true returns a value and would bind to the Func overload
    // instead.

    [Fact]
    public void Match_VoidAction_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        var calls = new List<string>();
        SuccessE<int, string>(42)
            .Match(
                (s) => calls.Add($"Success: {s}"),
                (e) => calls.Add($"Error: {e}")
            );
        Assert.Equal(new[] { "Success: 42" }, calls);
    }


    [Fact]
    public void Match_VoidAction_WhenGivenErrorResult_ExecutesErrorFn()
    {
        var calls = new List<string>();
        ErrorE<int, string>("Error message")
            .Match(
                (s) => calls.Add($"Success: {s}"),
                (e) => calls.Add($"Error: {e}")
            );
        Assert.Equal(new[] { "Error: Error message" }, calls);
    }


    // ── Match (Tier 2 lift — sync callbacks) ─────────────────────────────

    [Fact]
    public async Task Match_Task_WithReturnValue_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        string result = await Task.FromResult(SuccessE<int, string>(42))
            .Match(
                (s) => $"Success: {s}",
                (e) => $"Error: {e}"
            );
        Assert.Equal("Success: 42", result);
    }


    [Fact]
    public async Task Match_Task_WithReturnValue_WhenGivenErrorResult_ExecutesErrorFn()
    {
        string result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .Match(
                (s) => $"Success: {s}",
                (e) => $"Error: {e}"
            );
        Assert.Equal("Error: Error message", result);
    }


    [Fact]
    public async Task Match_Task_VoidAction_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        var calls = new List<string>();
        await Task.FromResult(SuccessE<int, string>(42))
            .Match(
                (s) => calls.Add($"Success: {s}"),
                (e) => calls.Add($"Error: {e}")
            );
        Assert.Equal(new[] { "Success: 42" }, calls);
    }


    [Fact]
    public async Task Match_Task_VoidAction_WhenGivenErrorResult_ExecutesErrorFn()
    {
        var calls = new List<string>();
        await Task.FromResult(ErrorE<int, string>("Error message"))
            .Match(
                (s) => calls.Add($"Success: {s}"),
                (e) => calls.Add($"Error: {e}")
            );
        Assert.Equal(new[] { "Error: Error message" }, calls);
    }


    // ── MatchAsync (non-Task input — async callbacks) ─────────────────────

    [Fact]
    public async Task MatchAsync_NonTaskInput_WithReturnValue_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        string result = await SuccessE<int, string>(42).MatchAsync(
            async (s) =>
            {
                await Task.Delay(1);
                return $"Success: {s}";
            },
            async (e) =>
            {
                await Task.Delay(1);
                return $"Error: {e}";
            }
        );
        Assert.Equal("Success: 42", result);
    }


    [Fact]
    public async Task MatchAsync_NonTaskInput_WithReturnValue_WhenGivenErrorResult_ExecutesErrorFn()
    {
        string result = await ErrorE<int, string>("Error message").MatchAsync(
            async (s) =>
            {
                await Task.Delay(1);
                return $"Success: {s}";
            },
            async (e) =>
            {
                await Task.Delay(1);
                return $"Error: {e}";
            }
        );
        Assert.Equal("Error: Error message", result);
    }


    [Fact]
    public async Task MatchAsync_NonTaskInput_VoidAction_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        var calls = new List<string>();
        await SuccessE<int, string>(42).MatchAsync(
            async (s) =>
            {
                await Task.Delay(1);
                calls.Add($"Success: {s}");
            },
            async (e) =>
            {
                await Task.Delay(1);
                calls.Add($"Error: {e}");
            }
        );
        Assert.Equal(new[] { "Success: 42" }, calls);
    }


    [Fact]
    public async Task MatchAsync_NonTaskInput_VoidAction_WhenGivenErrorResult_ExecutesErrorFn()
    {
        var calls = new List<string>();
        await ErrorE<int, string>("Error message").MatchAsync(
            async (s) =>
            {
                await Task.Delay(1);
                calls.Add($"Success: {s}");
            },
            async (e) =>
            {
                await Task.Delay(1);
                calls.Add($"Error: {e}");
            }
        );
        Assert.Equal(new[] { "Error: Error message" }, calls);
    }


    // ── MatchAsync (Tier 3 — async callbacks) ────────────────────────────

    [Fact]
    public async Task MatchAsync_WithReturnValue_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        string result = await Task.FromResult(SuccessE<int, string>(42))
            .MatchAsync(
                async (s) => {
                    await Task.Delay(1);
                    return $"Success: {s}";
                },
                async (e) => {
                    await Task.Delay(1);
                    return $"Error: {e}";
                }
            );
        Assert.Equal("Success: 42", result);
    }


    [Fact]
    public async Task MatchAsync_WithReturnValue_WhenGivenErrorResult_ExecutesErrorFn()
    {
        string result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MatchAsync(
                async (s) => {
                    await Task.Delay(1);
                    return $"Success: {s}";
                },
                async (e) => {
                    await Task.Delay(1);
                    return $"Error: {e}";
                }
            );
        Assert.Equal("Error: Error message", result);
    }


    [Fact]
    public async Task MatchAsync_VoidAction_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        bool successCalled = false;
        bool errorCalled = false;
        await Task.FromResult(SuccessE<int, string>(42))
            .MatchAsync(
                async (s) => {
                    await Task.Delay(1);
                    successCalled = true;
                },
                async (e) => {
                    await Task.Delay(1);
                    errorCalled = true;
                }
            );
        Assert.True(successCalled);
        Assert.False(errorCalled);
    }


    [Fact]
    public async Task MatchAsync_VoidAction_WhenGivenErrorResult_ExecutesErrorFn()
    {
        bool successCalled = false;
        bool errorCalled = false;
        await Task.FromResult(ErrorE<int, string>("Error message"))
            .MatchAsync(
                async (s) => {
                    await Task.Delay(1);
                    successCalled = true;
                },
                async (e) => {
                    await Task.Delay(1);
                    errorCalled = true;
                }
            );
        Assert.False(successCalled);
        Assert.True(errorCalled);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Partition ────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Partition_WhenGivenEmptySequence_ReturnsTwoEmptyCollections()
    {
        IEnumerable<Result<int, string>> input = [];
        (IEnumerable<int> successes, IEnumerable<string> failures) = input.Partition();
        Assert.Empty(successes);
        Assert.Empty(failures);
    }


    [Fact]
    public void Partition_WhenGivenAllSuccesses_ReturnsAllInSuccessCollection()
    {
        IEnumerable<Result<int, string>> input = new[] { Success(1), Success(2), Success(3) }
            .Select(r => (Result<int, string>)r);
        (IEnumerable<int> successes, IEnumerable<string> failures) = input.Partition();
        Assert.Equal(new[] { 1, 2, 3 }, successes);
        Assert.Empty(failures);
    }


    [Fact]
    public void Partition_WhenGivenAllErrors_ReturnsAllInErrorCollection()
    {
        IEnumerable<Result<int, string>> input = new[] { Error("err1"), Error("err2"), Error("err3") }
            .Select(r => (Result<int, string>)r);
        (IEnumerable<int> successes, IEnumerable<string> failures) = input.Partition();
        Assert.Empty(successes);
        Assert.Equal(new[] { "err1", "err2", "err3" }, failures);
    }


    [Fact]
    public void Partition_WhenGivenMixedResults_PartitionsCorrectly()
    {
        var input = new Result<int, string>[]
        {
            Success(1),
            Error("err1"),
            Success(2),
            Error("err2"),
            Success(3)
        };
        (IEnumerable<int> successes, IEnumerable<string> failures) = input.Partition();
        Assert.Equal(new[] { 1, 2, 3 }, successes);
        Assert.Equal(new[] { "err1", "err2" }, failures);
    }


    [Fact]
    public void Partition_PreservesOrderOfElements()
    {
        var input = new Result<int, string>[]
        {
            Success(10),
            Success(20),
            Error("z"),
            Error("y"),
            Success(30)
        };
        (IEnumerable<int> successes, IEnumerable<string> failures) = input.Partition();
        Assert.Equal(new[] { 10, 20, 30 }, successes);
        Assert.Equal(new[] { "z", "y" }, failures);
    }


    // ── Partition (sequence of Tasks) ─────────────────────────────────────

    [Fact]
    public async Task Partition_Tasks_WhenGivenEmptySequence_ReturnsTwoEmptyCollections()
    {
        IEnumerable<Task<Result<int, string>>> input = [];
        (IEnumerable<int> successes, IEnumerable<string> failures) = await input.Partition();
        Assert.Empty(successes);
        Assert.Empty(failures);
    }


    [Fact]
    public async Task Partition_Tasks_WhenGivenAllSuccesses_ReturnsAllInSuccessCollection()
    {
        (IEnumerable<int> successes, IEnumerable<string> failures) = await new[] {
            Task.FromResult(SuccessE<int, string>(1)),
            Task.FromResult(SuccessE<int, string>(2)),
            Task.FromResult(SuccessE<int, string>(3))
        }.Partition();
        Assert.Equal(new[] { 1, 2, 3 }, successes);
        Assert.Empty(failures);
    }


    [Fact]
    public async Task Partition_Tasks_WhenGivenAllErrors_ReturnsAllInErrorCollection()
    {
        (IEnumerable<int> successes, IEnumerable<string> failures) = await new[] {
            Task.FromResult(ErrorE<int, string>("err1")),
            Task.FromResult(ErrorE<int, string>("err2")),
            Task.FromResult(ErrorE<int, string>("err3"))
        }.Partition();
        Assert.Empty(successes);
        Assert.Equal(new[] { "err1", "err2", "err3" }, failures);
    }


    [Fact]
    public async Task Partition_Tasks_WhenGivenMixedResults_PartitionsCorrectly()
    {
        (IEnumerable<int> successes, IEnumerable<string> failures) = await new[] {
            Task.FromResult(SuccessE<int, string>(1)),
            Task.FromResult(ErrorE<int, string>("err1")),
            Task.FromResult(SuccessE<int, string>(2)),
            Task.FromResult(ErrorE<int, string>("err2")),
            Task.FromResult(SuccessE<int, string>(3))
        }.Partition();
        Assert.Equal(new[] { 1, 2, 3 }, successes);
        Assert.Equal(new[] { "err1", "err2" }, failures);
    }


    [Fact]
    public async Task Partition_Tasks_PreservesOrderOfElements()
    {
        (IEnumerable<int> successes, IEnumerable<string> failures) = await new[] {
            Task.FromResult(SuccessE<int, string>(10)),
            Task.FromResult(SuccessE<int, string>(20)),
            Task.FromResult(ErrorE<int, string>("z")),
            Task.FromResult(ErrorE<int, string>("y")),
            Task.FromResult(SuccessE<int, string>(30))
        }.Partition();
        Assert.Equal(new[] { 10, 20, 30 }, successes);
        Assert.Equal(new[] { "z", "y" }, failures);
    }


    [Fact]
    public async Task Partition_Tasks_WaitsForAllTasksToComplete()
    {
        (IEnumerable<int> successes, IEnumerable<string> failures) = await new[] {
            Task.Run(async () => { await Task.Delay(10); return SuccessE<int, string>(1); }),
            Task.Run(async () => { await Task.Delay(5); return ErrorE<int, string>("err1"); }),
            Task.Run(async () => { await Task.Delay(15); return SuccessE<int, string>(2); })
        }.Partition();
        Assert.Equal(new[] { 1, 2 }, successes);
        Assert.Equal(new[] { "err1" }, failures);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── RequireNonEmpty ───────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void RequireNonEmpty_WhenGivenNonEmptyList_ReturnsSuccessWithSameList()
    {
        int[] input = new[] { 1, 2, 3 };
        Result<IReadOnlyList<int>, string> result = input.RequireNonEmpty(() => "empty");
        Assert.Same(input, result.GetValue());
    }


    [Fact]
    public void RequireNonEmpty_WhenGivenEmptyList_ReturnsErrorFromFactory()
    {
        var input = new List<int>();
        Result<IReadOnlyList<int>, string> result = input.RequireNonEmpty(() => "empty");
        Assert.Equal("empty", result.GetError());
    }


    [Fact]
    public void RequireNonEmpty_WhenGivenNonEmptyList_DoesNotInvokeErrorFactory()
    {
        bool invoked = false;
        Result<IReadOnlyList<int>, string> result = new[] { 1 }.RequireNonEmpty(() =>
        {
            invoked = true;
            return "empty";
        });
        Assert.True(result.IsSuccess);
        Assert.False(invoked);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── RequireSingle ─────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void RequireSingle_WhenGivenOneElement_ReturnsSuccessWithThatElement()
    {
        Result<int, string> result = new[] { 42 }.RequireSingle(() => "not single");
        Assert.Equal(42, result.GetValue());
    }


    [Fact]
    public void RequireSingle_WhenGivenNoElements_ReturnsErrorFromFactory()
    {
        IEnumerable<int> empty = [];
        Result<int, string> result = empty.RequireSingle(() => "not single");
        Assert.Equal("not single", result.GetError());
    }


    [Fact]
    public void RequireSingle_WhenGivenMultipleElements_ReturnsErrorFromFactory()
    {
        Result<int, string> result = new[] { 1, 2 }.RequireSingle(() => "not single");
        Assert.Equal("not single", result.GetError());
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

        Result<int, string> result = input.RequireSingle(() => "not single");

        Assert.True(result.IsError);
        Assert.Equal(2, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Select ───────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Select_WhenInputIsSuccess_MapsValue()
    {
        Result<int, string> result = SuccessE<int, string>(21).Select((x) => x * 2);

        Assert.Equal(42, result.GetValue());
    }


    [Fact]
    public void Select_WhenInputIsError_ReturnsErrorWithoutInvokingSelector()
    {
        int numInvocations = 0;

        Result<int, string> result = ErrorE<int, string>("err1").Select((x) =>
        {
            numInvocations++;
            return x * 2;
        });

        Assert.Equal("err1", result.GetError());
        Assert.Equal(0, numInvocations);
    }


    [Fact]
    public void Select_WhenUsedWithQuerySyntax_MapsValue()
    {
        Result<int, string> input = Success(21);

        Result<int, string> result =
            from x in input
            select x * 2;

        Assert.Equal(42, result.GetValue());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── SelectMany ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void SelectMany_WhenBothSucceed_ProjectsBothValues()
    {
        Result<string, string> result = SuccessE<int, string>(2).SelectMany(
            (a) => SuccessE<string, string>(new string('x', a)),
            (a, b) => $"{a}:{b}");

        Assert.Equal("2:xx", result.GetValue());
    }


    [Fact]
    public void SelectMany_WhenInputIsError_ReturnsErrorWithoutInvokingNextOrProject()
    {
        int numNextInvocations = 0;
        int numProjectInvocations = 0;

        Result<int, string> result = ErrorE<int, string>("err1").SelectMany(
            (a) =>
            {
                numNextInvocations++;
                return SuccessE<int, string>(a);
            },
            (a, b) =>
            {
                numProjectInvocations++;
                return a + b;
            });

        Assert.Equal("err1", result.GetError());
        Assert.Equal(0, numNextInvocations);
        Assert.Equal(0, numProjectInvocations);
    }


    [Fact]
    public void SelectMany_WhenNextReturnsError_ReturnsThatErrorWithoutInvokingProject()
    {
        int numProjectInvocations = 0;

        Result<int, string> result = SuccessE<int, string>(1).SelectMany(
            (a) => ErrorE<int, string>("err2"),
            (a, b) =>
            {
                numProjectInvocations++;
                return a + b;
            });

        Assert.Equal("err2", result.GetError());
        Assert.Equal(0, numProjectInvocations);
    }


    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntaxAndAllSucceed_CombinesValuesOfDifferentTypes()
    {
        Result<string, string> result =
            from count in SuccessE<int, string>(3)
            from text in SuccessE<string, string>("ab")
            from scale in SuccessE<double, string>(1.5)
            select $"{count}-{text}-{scale.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

        Assert.Equal("3-ab-1.5", result.GetValue());
    }


    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntaxAndMiddleFails_StopsAtThatError()
    {
        int numLastInvocations = 0;

        Result<int, string> last()
        {
            numLastInvocations++;
            return Success(3);
        }

        Result<int, string> result =
            from a in SuccessE<int, string>(1)
            from b in ErrorE<int, string>("err2")
            from c in last()
            select a + b + c;

        Assert.Equal("err2", result.GetError());
        Assert.Equal(0, numLastInvocations);
    }


    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntax_AllowsLaterClausesToUseEarlierValues()
    {
        static Result<int, string> half(int x) =>
            x % 2 == 0 ? Success(x / 2) : Error($"{x} is odd");

        Result<int, string> success =
            from a in SuccessE<int, string>(8)
            from b in half(a)
            from c in half(b)
            select c;

        Result<int, string> error =
            from a in SuccessE<int, string>(6)
            from b in half(a)
            from c in half(b)
            select c;

        Assert.Equal(2, success.GetValue());
        Assert.Equal("3 is odd", error.GetError());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Successes ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Successes_WhenGivenEmptySequence_ReturnsEmptySequence()
    {
        IEnumerable<Result<int, string>> input = [];
        Assert.Empty(input.Successes());
    }


    [Fact]
    public void Successes_WhenGivenMixedResults_ReturnsOnlySuccessValuesInOrder()
    {
        var input = new Result<int, string>[] { Success(1), Error("err1"), Success(2), Error("err2") };
        Assert.Equal(new[] { 1, 2 }, input.Successes());
    }


    [Fact]
    public void Successes_IsEvaluatedLazily()
    {
        int numEnumerated = 0;
        IEnumerable<Result<int, string>> input = new Result<int, string>[] { Error("err1"), Success(1), Success(2) }
            .Select((r) =>
            {
                numEnumerated++;
                return r;
            });

        IEnumerable<int> successes = input.Successes();
        Assert.Equal(0, numEnumerated);

        Assert.Equal(1, successes.First());
        Assert.Equal(2, numEnumerated);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Tap ───────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Tap_PassesResultToActionAndReturnsInput()
    {
        var seen = new List<Result<int, string>>();
        Result<int, string> input = Success(1);

        Result<int, string> output = input.Tap(seen.Add);

        Assert.Same(input, output);
        Assert.Equal(new[] { input }, seen);
    }


    [Fact]
    public void Tap_WhenGivenErrorResult_StillInvokesAction()
    {
        var seen = new List<Result<int, string>>();
        Result<int, string> input = Error("err");

        Result<int, string> output = input.Tap(seen.Add);

        Assert.Same(input, output);
        Assert.Equal(new[] { input }, seen);
    }


    // ── Tap (Tier 2 lift — sync callback) ─────────────────────────────────

    [Fact]
    public async Task Tap_Task_PassesResultToActionAndReturnsInput()
    {
        var seen = new List<Result<int, string>>();
        Result<int, string> input = Success(1);

        Result<int, string> output = await Task.FromResult(input).Tap(seen.Add);

        Assert.Same(input, output);
        Assert.Equal(new[] { input }, seen);
    }


    // ── TapAsync (async callback) ─────────────────────────────────────────

    [Fact]
    public async Task TapAsync_PassesResultToActionAndReturnsInput()
    {
        var seen = new List<Result<int, string>>();
        Result<int, string> input = Success(1);

        Result<int, string> output = await input.TapAsync(async (r) =>
        {
            await Task.Delay(1);
            seen.Add(r);
        });

        Assert.Same(input, output);
        Assert.Equal(new[] { input }, seen);
    }


    [Fact]
    public async Task TapAsync_Task_PassesResultToActionAndReturnsInput()
    {
        var seen = new List<Result<int, string>>();
        Result<int, string> input = Error("err");

        Result<int, string> output = await Task.FromResult(input).TapAsync(async (r) =>
        {
            await Task.Delay(1);
            seen.Add(r);
        });

        Assert.Same(input, output);
        Assert.Equal(new[] { input }, seen);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── TapError ──────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void TapError_WhenGivenErrorResult_InvokesActionWithErrorAndReturnsInput()
    {
        var seen = new List<string>();
        Result<int, string> input = Error("err");

        Result<int, string> output = input.TapError(seen.Add);

        Assert.Same(input, output);
        Assert.Equal(new[] { "err" }, seen);
    }


    [Fact]
    public void TapError_WhenGivenSuccessResult_DoesNotInvokeAction()
    {
        var seen = new List<string>();
        Result<int, string> input = Success(1);

        Result<int, string> output = input.TapError(seen.Add);

        Assert.Same(input, output);
        Assert.Empty(seen);
    }


    // ── TapError (Tier 2 lift — sync callback) ────────────────────────────

    [Fact]
    public async Task TapError_Task_WhenGivenErrorResult_InvokesActionWithError()
    {
        var seen = new List<string>();
        Result<int, string> output = await Task.FromResult(ErrorE<int, string>("err")).TapError(seen.Add);
        Assert.Equal("err", output.GetError());
        Assert.Equal(new[] { "err" }, seen);
    }


    [Fact]
    public async Task TapError_Task_WhenGivenSuccessResult_DoesNotInvokeAction()
    {
        var seen = new List<string>();
        Result<int, string> output = await Task.FromResult(SuccessE<int, string>(1)).TapError(seen.Add);
        Assert.Equal(1, output.GetValue());
        Assert.Empty(seen);
    }


    // ── TapErrorAsync (async callback) ────────────────────────────────────

    [Fact]
    public async Task TapErrorAsync_WhenGivenErrorResult_InvokesActionWithError()
    {
        var seen = new List<string>();
        Result<int, string> input = Error("err");

        Result<int, string> output = await input.TapErrorAsync(async (e) =>
        {
            await Task.Delay(1);
            seen.Add(e);
        });

        Assert.Same(input, output);
        Assert.Equal(new[] { "err" }, seen);
    }


    [Fact]
    public async Task TapErrorAsync_WhenGivenSuccessResult_DoesNotInvokeAction()
    {
        var seen = new List<string>();
        Result<int, string> input = Success(1);

        Result<int, string> output = await input.TapErrorAsync(async (e) =>
        {
            await Task.Delay(1);
            seen.Add(e);
        });

        Assert.Same(input, output);
        Assert.Empty(seen);
    }


    [Fact]
    public async Task TapErrorAsync_Task_WhenGivenErrorResult_InvokesActionWithError()
    {
        var seen = new List<string>();

        Result<int, string> output = await Task.FromResult(ErrorE<int, string>("err")).TapErrorAsync(async (e) =>
        {
            await Task.Delay(1);
            seen.Add(e);
        });

        Assert.Equal("err", output.GetError());
        Assert.Equal(new[] { "err" }, seen);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── TapSuccess ────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void TapSuccess_WhenGivenSuccessResult_InvokesActionWithValueAndReturnsInput()
    {
        var seen = new List<int>();
        Result<int, string> input = Success(1);

        Result<int, string> output = input.TapSuccess(seen.Add);

        Assert.Same(input, output);
        Assert.Equal(new[] { 1 }, seen);
    }


    [Fact]
    public void TapSuccess_WhenGivenErrorResult_DoesNotInvokeAction()
    {
        var seen = new List<int>();
        Result<int, string> input = Error("err");

        Result<int, string> output = input.TapSuccess(seen.Add);

        Assert.Same(input, output);
        Assert.Empty(seen);
    }


    // ── TapSuccess (Tier 2 lift — sync callback) ──────────────────────────

    [Fact]
    public async Task TapSuccess_Task_WhenGivenSuccessResult_InvokesActionWithValue()
    {
        var seen = new List<int>();
        Result<int, string> output = await Task.FromResult(SuccessE<int, string>(1)).TapSuccess(seen.Add);
        Assert.Equal(1, output.GetValue());
        Assert.Equal(new[] { 1 }, seen);
    }


    [Fact]
    public async Task TapSuccess_Task_WhenGivenErrorResult_DoesNotInvokeAction()
    {
        var seen = new List<int>();
        Result<int, string> output = await Task.FromResult(ErrorE<int, string>("err")).TapSuccess(seen.Add);
        Assert.Equal("err", output.GetError());
        Assert.Empty(seen);
    }


    // ── TapSuccessAsync (async callback) ──────────────────────────────────

    [Fact]
    public async Task TapSuccessAsync_WhenGivenSuccessResult_InvokesActionWithValue()
    {
        var seen = new List<int>();
        Result<int, string> input = Success(1);

        Result<int, string> output = await input.TapSuccessAsync(async (s) =>
        {
            await Task.Delay(1);
            seen.Add(s);
        });

        Assert.Same(input, output);
        Assert.Equal(new[] { 1 }, seen);
    }


    [Fact]
    public async Task TapSuccessAsync_WhenGivenErrorResult_DoesNotInvokeAction()
    {
        var seen = new List<int>();
        Result<int, string> input = Error("err");

        Result<int, string> output = await input.TapSuccessAsync(async (s) =>
        {
            await Task.Delay(1);
            seen.Add(s);
        });

        Assert.Same(input, output);
        Assert.Empty(seen);
    }


    [Fact]
    public async Task TapSuccessAsync_Task_WhenGivenSuccessResult_InvokesActionWithValue()
    {
        var seen = new List<int>();

        Result<int, string> output = await Task.FromResult(SuccessE<int, string>(1)).TapSuccessAsync(async (s) =>
        {
            await Task.Delay(1);
            seen.Add(s);
        });

        Assert.Equal(1, output.GetValue());
        Assert.Equal(new[] { 1 }, seen);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ThrowIfError ─────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ThrowIfError_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        int val =
            SuccessE<int, string>(42)
            .ThrowIfError();

        Assert.Equal(42, val);
    }


    [Fact]
    public void ThrowIfError_WhenGivenErrorResult_ThrowsException()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            ErrorE<int, string>("Error message")
            .ThrowIfError()
        );

        Assert.Contains("Expected successful Result but got error: Error message", ex.Message);
    }


    [Fact]
    public void ThrowIfError_WhenGivenErrorResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            ErrorE<int, string>("Error message")
            .ThrowIfError("Custom error")
        );

        Assert.Equal("Custom error", ex.Message);
    }


    // ── ThrowIfError (message function) ───────────────────────────────────

    [Fact]
    public void ThrowIfError_MessageFn_WhenGivenErrorResult_ThrowsWithMessageFromError()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => ErrorE<int, string>("boom").ThrowIfError((e) => $"Failed: {e}"));
        Assert.Equal("Failed: boom", ex.Message);
    }


    [Fact]
    public void ThrowIfError_MessageFn_WhenGivenSuccessResult_ReturnsValueWithoutCallingFunction()
    {
        bool fnCalled = false;
        int val = SuccessE<int, string>(42).ThrowIfError((e) =>
        {
            fnCalled = true;
            return e;
        });
        Assert.Equal(42, val);
        Assert.False(fnCalled);
    }


    [Fact]
    public async Task ThrowIfError_MessageFn_Task_WhenGivenErrorResult_ThrowsWithMessageFromError()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(ErrorE<int, string>("boom")).ThrowIfError((e) => $"Failed: {e}"));
        Assert.Equal("Failed: boom", ex.Message);
    }


    // ── ThrowIfError (Tier 2 lift) ───────────────────────────────────

    [Fact]
    public async Task ThrowIfError_Task_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        int val = await Task.FromResult(SuccessE<int, string>(42)).ThrowIfError();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task ThrowIfError_Task_WhenGivenErrorResult_ThrowsException()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(ErrorE<int, string>("Error message")).ThrowIfError());
        Assert.Contains("Expected successful Result but got error: Error message", ex.Message);
    }


    [Fact]
    public async Task ThrowIfError_Task_WhenGivenErrorResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(ErrorE<int, string>("Error message")).ThrowIfError("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ThrowIfSuccess ───────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ThrowIfSuccess_WhenGivenErrorResult_ReturnsErrorValue()
    {
        string err =
            ErrorE<double, string>("Error message")
            .ThrowIfSuccess();

        Assert.Equal("Error message", err);
    }


    [Fact]
    public void ThrowIfSuccess_WhenGivenSuccessResult_ThrowsException()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            SuccessE<int, string>(42)
            .ThrowIfSuccess()
        );

        Assert.Contains("Expected error Result but got success: 42", ex.Message);
    }


    [Fact]
    public void ThrowIfSuccess_WhenGivenSuccessResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            SuccessE<int, string>(42)
            .ThrowIfSuccess("Custom error")
        );

        Assert.Equal("Custom error", ex.Message);
    }


    // ── ThrowIfSuccess (message function) ─────────────────────────────────

    [Fact]
    public void ThrowIfSuccess_MessageFn_WhenGivenSuccessResult_ThrowsWithMessageFromValue()
    {
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => SuccessE<int, string>(42).ThrowIfSuccess((s) => $"Unexpected: {s}"));
        Assert.Equal("Unexpected: 42", ex.Message);
    }


    [Fact]
    public void ThrowIfSuccess_MessageFn_WhenGivenErrorResult_ReturnsErrorWithoutCallingFunction()
    {
        bool fnCalled = false;
        string err = ErrorE<int, string>("err").ThrowIfSuccess((s) =>
        {
            fnCalled = true;
            return "unexpected";
        });
        Assert.Equal("err", err);
        Assert.False(fnCalled);
    }


    [Fact]
    public async Task ThrowIfSuccess_MessageFn_Task_WhenGivenSuccessResult_ThrowsWithMessageFromValue()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).ThrowIfSuccess((s) => $"Unexpected: {s}"));
        Assert.Equal("Unexpected: 42", ex.Message);
    }


    // ── ThrowIfSuccess (Tier 2 lift) ────────────────────────────────────

    [Fact]
    public async Task ThrowIfSuccess_Task_WhenGivenErrorResult_ReturnsErrorValue()
    {
        string err = await Task.FromResult(ErrorE<int, string>("Error message")).ThrowIfSuccess();
        Assert.Equal("Error message", err);
    }


    [Fact]
    public async Task ThrowIfSuccess_Task_WhenGivenSuccessResult_ThrowsException()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).ThrowIfSuccess());
        Assert.Contains("Expected error Result but got success: 42", ex.Message);
    }


    [Fact]
    public async Task ThrowIfSuccess_Task_WhenGivenSuccessResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).ThrowIfSuccess("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ToOption ──────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ToOption_WhenGivenSuccessResult_ReturnsSome()
    {
        Result<string, string> input = SuccessE<string, string>("alpha");

        Option<string> result = input.ToOption();

        Assert.Equal(Some("alpha"), result);
    }


    [Fact]
    public void ToOption_WhenGivenErrorResult_ReturnsNone()
    {
        Result<string, string> input = ErrorE<string, string>("missing value");

        Option<string> result = input.ToOption();

        Assert.True(result.IsNone);
    }


    [Fact]
    public void ToOption_WhenGivenNullResult_ThrowsArgumentNullException()
    {
        Result<string, string> input = null!;

        Assert.Throws<ArgumentNullException>(() => input.ToOption());
    }


    // ── ToOption (Tier 2 lift) ────────────────────────────────────────────

    [Fact]
    public async Task ToOption_Task_WhenGivenSuccessResult_ReturnsSome()
    {
        Task<Result<string, string>> input = Task.FromResult(SuccessE<string, string>("alpha"));

        Option<string> result = await input.ToOption();

        Assert.Equal(Some("alpha"), result);
    }


    [Fact]
    public async Task ToOption_Task_WhenGivenErrorResult_ReturnsNone()
    {
        Task<Result<string, string>> input = Task.FromResult(ErrorE<string, string>("missing value"));

        Option<string> result = await input.ToOption();

        Assert.True(result.IsNone);
    }


    [Fact]
    public async Task ToOption_Task_WhenGivenNullTask_ThrowsArgumentNullException()
    {
        Task<Result<string, string>> input = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await input.ToOption());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ToResult ─────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ToResult_WhenGivenNonNullReference_ReturnsSuccess()
    {
        string? value = "alpha";

        var result = value.ToResult(() => "missing value");

        Assert.Equal("alpha", result.GetValue());
    }


    [Fact]
    public void ToResult_WhenGivenNullReference_ReturnsErrorFromFactory()
    {
        string? value = null;

        var result = value.ToResult(() => "missing value");

        Assert.Equal("missing value", result.GetError());
    }


    [Fact]
    public void ToResult_WhenGivenNonNullReference_DoesNotInvokeErrorFactory()
    {
        string? value = "alpha";
        bool invoked = false;

        var result = value.ToResult(() =>
        {
            invoked = true;
            return "missing value";
        });

        Assert.Equal("alpha", result.GetValue());
        Assert.False(invoked);
    }


    [Fact]
    public void ToResult_WhenGivenNullErrorFactory_ThrowsArgumentNullException()
    {
        string? value = "alpha";

        Assert.Throws<ArgumentNullException>(() => value.ToResult<string, string>(null!));
    }


    [Fact]
    public void ToResult_WhenGivenSomeOption_ReturnsSuccess()
    {
        var result = Some("alpha").ToResult(() => "missing value");

        Assert.Equal("alpha", result.GetValue());
    }


    [Fact]
    public void ToResult_WhenGivenNoneOption_ReturnsErrorFromFactory()
    {
        var result = NoneE<string>().ToResult(() => "missing value");

        Assert.Equal("missing value", result.GetError());
    }


    [Fact]
    public void ToResult_WhenGivenSomeOption_DoesNotInvokeErrorFactory()
    {
        bool invoked = false;

        var result = Some("alpha").ToResult(() =>
        {
            invoked = true;
            return "missing value";
        });

        Assert.Equal("alpha", result.GetValue());
        Assert.False(invoked);
    }


    [Fact]
    public void ToResult_WhenGivenNullOption_ThrowsArgumentNullException()
    {
        Option<string> input = null!;

        Assert.Throws<ArgumentNullException>(() => input.ToResult(() => "missing value"));
    }


    [Fact]
    public void ToResult_WhenGivenNoneOptionAndNullErrorFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => NoneE<string>().ToResult<string, string>(null!));
    }


    // ── ToResult (nullable value type) ────────────────────────────────────

    [Fact]
    public void ToResult_NullableValueType_WhenHasValue_ReturnsSuccess()
    {
        int? value = 5;
        Result<int, string> result = value.ToResult(() => "missing value");
        Assert.Equal(5, result.GetValue());
    }


    [Fact]
    public void ToResult_NullableValueType_WhenNull_ReturnsErrorFromFactory()
    {
        int? value = null;
        Result<int, string> result = value.ToResult(() => "missing value");
        Assert.Equal("missing value", result.GetError());
    }


    [Fact]
    public void ToResult_NullableValueType_WhenGivenNullErrorFactory_ThrowsArgumentNullException()
    {
        int? value = 5;
        Assert.Throws<ArgumentNullException>(() => value.ToResult<int, string>(null!));
    }


    // ── ToResult (bool) ───────────────────────────────────────────────────

    [Fact]
    public void ToResult_Bool_WhenTrue_ReturnsSuccessValue()
    {
        bool isValid = true;
        var result = isValid.ToResult(5, "invalid");
        Assert.Equal(5, result.GetValue());
    }


    [Fact]
    public void ToResult_Bool_WhenFalse_ReturnsErrorValue()
    {
        bool isValid = false;
        var result = isValid.ToResult(5, "invalid");
        Assert.Equal("invalid", result.GetError());
    }


    // ── ToResult (Tier 2 lift) ──────────────────────────────────────────

    [Fact]
    public async Task ToResult_Task_WhenGivenSomeOption_ReturnsSuccess()
    {
        Task<Option<string>> input = Task.FromResult(Some("alpha"));

        Result<string, string> result = await input.ToResult(() => "missing value");

        Assert.Equal("alpha", result.GetValue());
    }


    [Fact]
    public async Task ToResult_Task_WhenGivenNoneOption_ReturnsErrorFromFactory()
    {
        Task<Option<string>> input = Task.FromResult(NoneE<string>());

        Result<string, string> result = await input.ToResult(() => "missing value");

        Assert.Equal("missing value", result.GetError());
    }


    [Fact]
    public async Task ToResult_Task_WhenGivenSomeOption_DoesNotInvokeErrorFactory()
    {
        Task<Option<string>> input = Task.FromResult(Some("alpha"));
        bool invoked = false;

        Result<string, string> result = await input.ToResult(() =>
        {
            invoked = true;
            return "missing value";
        });

        Assert.Equal("alpha", result.GetValue());
        Assert.False(invoked);
    }


    [Fact]
    public async Task ToResult_Task_WhenGivenNullTask_ThrowsArgumentNullException()
    {
        Task<Option<string>> input = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await input.ToResult(() => "missing value"));
    }
}
