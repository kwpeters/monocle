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
        var input = Enumerable.Empty<Result<int, string>>();

        var result = input.AllOrErrors();

        Assert.Empty(result.ThrowIfError());
    }


    [Fact]
    public void AllOrErrors_WhenGivenAllSuccesses_ReturnsSuccessWithAllValuesInOrder()
    {
        var input = new Result<int, string>[] { Success(1), Success(2), Success(3) };

        var result = input.AllOrErrors();

        Assert.Equal(new[] { 1, 2, 3 }, result.ThrowIfError());
    }


    [Fact]
    public void AllOrErrors_WhenGivenAllErrors_ReturnsErrorWithAllErrorsInOrder()
    {
        var input = new Result<int, string>[] { Error("err1"), Error("err2"), Error("err3") };

        var result = input.AllOrErrors();

        Assert.Equal(new[] { "err1", "err2", "err3" }, result.ThrowIfSuccess());
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

        var result = input.AllOrErrors();

        Assert.Equal(new[] { "err1", "err2" }, result.ThrowIfSuccess());
    }


    [Fact]
    public void AllOrErrors_WhenGivenAnError_StillEnumeratesEntireSequence()
    {
        int numEnumerated = 0;
        var input = new Result<int, string>[] { Error("err1"), Success(1), Error("err2") }
            .Select((r) =>
            {
                numEnumerated++;
                return r;
            });

        var result = input.AllOrErrors();

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
        var input = Enumerable.Empty<Result<int, string>>();

        var result = input.AllOrFirstError();

        Assert.Empty(result.ThrowIfError());
    }


    [Fact]
    public void AllOrFirstError_WhenGivenAllSuccesses_ReturnsSuccessWithAllValuesInOrder()
    {
        var input = new Result<int, string>[] { Success(1), Success(2), Success(3) };

        var result = input.AllOrFirstError();

        Assert.Equal(new[] { 1, 2, 3 }, result.ThrowIfError());
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

        var result = input.AllOrFirstError();

        Assert.Equal("err1", result.ThrowIfSuccess());
    }


    [Fact]
    public void AllOrFirstError_WhenGivenAnError_StopsEnumeratingAtFirstError()
    {
        int numEnumerated = 0;
        var input = new Result<int, string>[] { Success(1), Error("err1"), Success(2), Error("err2") }
            .Select((r) =>
            {
                numEnumerated++;
                return r;
            });

        var result = input.AllOrFirstError();

        Assert.Equal("err1", result.ThrowIfSuccess());
        Assert.Equal(2, numEnumerated);
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

        var err =
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

        var val =
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
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .Bind((x) => SuccessE<string, string>($"Value: {x}"));
        var val = result.ThrowIfError();
        Assert.Equal("Value: 42", val);
    }


    [Fact]
    public async Task Bind_Task_WhenGivenErrorResult_PropagatesErrorWithoutCallingFunction()
    {
        var fnCalled = false;
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .Bind((x) => {
                fnCalled = true;
                return SuccessE<string, string>($"Value: {x}");
            });
        var err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }


    // ── BindAsync (Tier 3 — async callback) ──────────────────────────────

    [Fact]
    public async Task BindAsync_WhenGivenSuccessResult_ChainsCorrectly()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .BindAsync(async (x) => {
                await Task.Delay(1);
                return SuccessE<string, string>($"Value: {x}");
            });
        var val = result.ThrowIfError();
        Assert.Equal("Value: 42", val);
    }


    [Fact]
    public async Task BindAsync_WhenGivenErrorResult_PropagatesErrorWithoutCallingFunction()
    {
        var fnCalled = false;
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .BindAsync(async (x) => {
                fnCalled = true;
                await Task.Delay(1);
                return SuccessE<string, string>($"Value: {x}");
            });
        var err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }


    [Fact]
    public async Task BindAsync_WhenFunctionReturnsError_PropagatesError()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .BindAsync(async (x) => {
                await Task.Delay(1);
                return ErrorE<string, string>("Function error");
            });
        var err = result.ThrowIfSuccess();
        Assert.Equal("Function error", err);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── DefaultValue ─────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void DefaultValue_WhenInputIsSuccess_ReturnsSuccessValue()
    {
        var outVal =
            SuccessE<int, string>(5)
            .DefaultValue(3);

        Assert.Equal(5, outVal);
    }


    [Fact]
    public void DefaultValue_WhenInputIsError_ReturnsDefaultValue()
    {
        var outVal =
            ErrorE<int, string>("Error message")
            .DefaultValue(3);

        Assert.Equal(3, outVal);
    }


    // ── DefaultValue (Tier 2 lift) ───────────────────────────────────────

    [Fact]
    public async Task DefaultValue_Task_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        var val = await Task.FromResult(SuccessE<int, string>(42)).DefaultValue(0);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task DefaultValue_Task_WhenGivenErrorResult_ReturnsDefaultValue()
    {
        var val = await Task.FromResult(ErrorE<int, string>("Error message")).DefaultValue(0);
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
        var numInvocations = 0;

        var outVal =
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
        var numInvocations = 0;

        var outVal =
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
        var fnCalled = false;
        var val = await Task.FromResult(SuccessE<int, string>(42))
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
        var val = await Task.FromResult(ErrorE<int, string>("Error message"))
            .DefaultWith((err) => err.Length);
        Assert.Equal(13, val);
    }


    // ── DefaultWithAsync (Tier 3 — async callback) ───────────────────────

    [Fact]
    public async Task DefaultWithAsync_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        var fnCalled = false;
        var val = await Task.FromResult(SuccessE<int, string>(42))
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
        var val = await Task.FromResult(ErrorE<int, string>("Error message"))
            .DefaultWithAsync(async (err) => {
                await Task.Delay(1);
                return err.Length;
            });
        Assert.Equal(13, val);
    }


    [Fact]
    public async Task DefaultWithAsync_SyncResult_WhenGivenSuccessResult_ReturnsSuccessValueWithoutCallingFunction()
    {
        var fnCalled = false;
        Result<int, string> input = Success(42);
        var val = await input.DefaultWithAsync(async (err) => {
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
        Result<int, string> input = Error("Error message");
        var val = await input.DefaultWithAsync(async (err) => {
            await Task.Delay(1);
            return err.Length;
        });
        Assert.Equal(13, val);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Gate ─────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Gate_WhenInputIsError_GateFnIsNotInvokedAndErrorResultIsReturned()
    {
        var numInvocations = 0;

        var err =
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
        var numInvocations = 0;

        var val =
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
        var numInvocations = 0;

        var err =
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
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .Gate((x) => {
                Result<bool, string> r = x > 0 ?
                    SuccessE<bool, string>(true) :
                    ErrorE<bool, string>("Validation failed");
                return r;
            });
        var val = result.ThrowIfError();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task Gate_Task_WhenInputSuccessAndGateFails_PropagatesGateError()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .Gate((x) => {
                Result<bool, string> r = x < 0 ?
                    SuccessE<bool, string>(true) :
                    ErrorE<bool, string>("Gate validation failed");
                return r;
            });
        var err = result.ThrowIfSuccess();
        Assert.Equal("Gate validation failed", err);
    }


    [Fact]
    public async Task Gate_Task_WhenInputError_PropagatesErrorWithoutCallingGate()
    {
        var gateCalled = false;
        var result = await Task.FromResult(ErrorE<int, string>("Input error"))
            .Gate((x) => {
                gateCalled = true;
                Result<bool, string> r = SuccessE<bool, string>(true);
                return r;
            });
        var err = result.ThrowIfSuccess();
        Assert.False(gateCalled);
        Assert.Equal("Input error", err);
    }


    // ── GateAsync (Tier 3 — async callback) ──────────────────────────────

    [Fact]
    public async Task GateAsync_WhenInputSuccessAndGateSucceeds_PreservesOriginalValue()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .GateAsync(async (x) => {
                await Task.Delay(1);
                return x > 0 ? SuccessE<bool, string>(true) : ErrorE<bool, string>("Validation failed");
            });
        var val = result.ThrowIfError();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task GateAsync_WhenInputSuccessAndGateFails_PropagatesGateError()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .GateAsync(async (x) => {
                await Task.Delay(1);
                return x < 0 ? SuccessE<bool, string>(true) : ErrorE<bool, string>("Gate validation failed");
            });
        var err = result.ThrowIfSuccess();
        Assert.Equal("Gate validation failed", err);
    }


    [Fact]
    public async Task GateAsync_WhenInputError_PropagatesErrorWithoutCallingGate()
    {
        var gateCalled = false;
        var result = await Task.FromResult(ErrorE<int, string>("Input error"))
            .GateAsync(async (x) => {
                gateCalled = true;
                await Task.Delay(1);
                return SuccessE<bool, string>(true);
            });
        var err = result.ThrowIfSuccess();
        Assert.False(gateCalled);
        Assert.Equal("Input error", err);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── GetError / GetValue ──────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void GetError_WhenGivenErrorResult_ReturnsErrorValue()
    {
        var err = ErrorE<int, string>("Error message").GetError();
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
        var err = await Task.FromResult(ErrorE<int, string>("Error message")).GetError();
        Assert.Equal("Error message", err);
    }


    [Fact]
    public async Task GetError_Task_WhenGivenSuccessResult_ThrowsException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).GetError());
    }


    [Fact]
    public void GetValue_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        var val = SuccessE<int, string>(42).GetValue();
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
        var val = await Task.FromResult(SuccessE<int, string>(42)).GetValue();
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

        var val =
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

        var err =
            ErrorE<int, string>("Original error")
            .MapError(appendText)
            .ThrowIfSuccess();

        Assert.Equal(1, numInvocations);
        Assert.Equal("Original error (modified)", err);
    }


    [Fact]
    public void MapError_WhenMappingTypeChanges_ReturnsCorrectType()
    {
        var errVal =
            ErrorE<int, string>("Error message")
            .MapError(err => err.Length)
            .ThrowIfSuccess();

        Assert.Equal(13, errVal);
    }


    [Fact]
    public void MapError_WhenChained_AppliesTransformationsInOrder()
    {
        var errVal =
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

        var val =
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
        var fnCalled = false;
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .MapError((err) => {
                fnCalled = true;
                return $"Transformed: {err}";
            });
        var val = result.ThrowIfError();
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task MapError_Task_WhenGivenErrorResult_TransformsError()
    {
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapError((err) => $"Transformed: {err}");
        var err = result.ThrowIfSuccess();
        Assert.Equal("Transformed: Error message", err);
    }


    // ── MapErrorAsync (Tier 3 — async callback) ──────────────────────────

    [Fact]
    public async Task MapErrorAsync_WhenGivenSuccessResult_PropagatesSuccess()
    {
        var fnCalled = false;
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .MapErrorAsync(async (err) => {
                fnCalled = true;
                await Task.Delay(1);
                return $"Transformed: {err}";
            });
        var val = result.ThrowIfError();
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task MapErrorAsync_WhenGivenErrorResult_TransformsError()
    {
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapErrorAsync(async (err) => {
                await Task.Delay(1);
                return $"Transformed: {err}";
            });
        var err = result.ThrowIfSuccess();
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

        var err =
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

        var val =
            SuccessE<int, string>(5)
            .MapSuccess(doubleValue)
            .ThrowIfError();

        Assert.Equal(1, numInvocations);
        Assert.Equal(10, val);
    }


    [Fact]
    public void MapSuccess_WhenMappingTypeChanges_ReturnsCorrectType()
    {
        var val =
            SuccessE<int, string>(42)
            .MapSuccess(x => $"Value: {x}")
            .ThrowIfError();

        Assert.Equal("Value: 42", val);
    }


    [Fact]
    public void MapSuccess_WhenChained_AppliesTransformationsInOrder()
    {
        var val =
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

        var err =
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
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .MapSuccess((x) => x * 2);
        var val = result.ThrowIfError();
        Assert.Equal(84, val);
    }


    [Fact]
    public async Task MapSuccess_Task_WhenGivenErrorResult_PropagatesError()
    {
        var fnCalled = false;
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapSuccess((x) => {
                fnCalled = true;
                return x * 2;
            });
        var err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }


    // ── MapSuccessAsync (Tier 3 — async callback) ────────────────────────

    [Fact]
    public async Task MapSuccessAsync_WhenGivenSuccessResult_TransformsValue()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .MapSuccessAsync(async (x) => {
                await Task.Delay(1);
                return x * 2;
            });
        var val = result.ThrowIfError();
        Assert.Equal(84, val);
    }


    [Fact]
    public async Task MapSuccessAsync_WhenGivenErrorResult_PropagatesError()
    {
        var fnCalled = false;
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapSuccessAsync(async (x) => {
                fnCalled = true;
                await Task.Delay(1);
                return x * 2;
            });
        var err = result.ThrowIfSuccess();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Match ────────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Match_WhenGivenASuccessResult_RunsTheSuccessFunction()
    {
        var val =
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
        var val =
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
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .Match(
                (s) => $"Success: {s}",
                (e) => $"Error: {e}"
            );
        Assert.Equal("Success: 42", result);
    }


    [Fact]
    public async Task Match_Task_WithReturnValue_WhenGivenErrorResult_ExecutesErrorFn()
    {
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
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


    // ── MatchAsync (Tier 3 — async callbacks) ────────────────────────────

    [Fact]
    public async Task MatchAsync_WithReturnValue_WhenGivenSuccessResult_ExecutesSuccessFn()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
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
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
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
        var successCalled = false;
        var errorCalled = false;
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
        var successCalled = false;
        var errorCalled = false;
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
        var input = Enumerable.Empty<Result<int, string>>();
        var (successes, failures) = input.Partition();
        Assert.Empty(successes);
        Assert.Empty(failures);
    }


    [Fact]
    public void Partition_WhenGivenAllSuccesses_ReturnsAllInSuccessCollection()
    {
        var input = new[] { Success(1), Success(2), Success(3) }
            .Select(r => (Result<int, string>)r);
        var (successes, failures) = input.Partition();
        Assert.Equal(new[] { 1, 2, 3 }, successes);
        Assert.Empty(failures);
    }


    [Fact]
    public void Partition_WhenGivenAllErrors_ReturnsAllInErrorCollection()
    {
        var input = new[] { Error("err1"), Error("err2"), Error("err3") }
            .Select(r => (Result<int, string>)r);
        var (successes, failures) = input.Partition();
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
        var (successes, failures) = input.Partition();
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
        var (successes, failures) = input.Partition();
        Assert.Equal(new[] { 10, 20, 30 }, successes);
        Assert.Equal(new[] { "z", "y" }, failures);
    }


    // ── PartitionAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task PartitionAsync_WhenGivenEmptySequence_ReturnsTwoEmptyCollections()
    {
        var input = Enumerable.Empty<Task<Result<int, string>>>();
        var (successes, failures) = await input.PartitionAsync();
        Assert.Empty(successes);
        Assert.Empty(failures);
    }


    [Fact]
    public async Task PartitionAsync_WhenGivenAllSuccesses_ReturnsAllInSuccessCollection()
    {
        var (successes, failures) = await new[] {
            Task.FromResult(SuccessE<int, string>(1)),
            Task.FromResult(SuccessE<int, string>(2)),
            Task.FromResult(SuccessE<int, string>(3))
        }.PartitionAsync();
        Assert.Equal(new[] { 1, 2, 3 }, successes);
        Assert.Empty(failures);
    }


    [Fact]
    public async Task PartitionAsync_WhenGivenAllErrors_ReturnsAllInErrorCollection()
    {
        var (successes, failures) = await new[] {
            Task.FromResult(ErrorE<int, string>("err1")),
            Task.FromResult(ErrorE<int, string>("err2")),
            Task.FromResult(ErrorE<int, string>("err3"))
        }.PartitionAsync();
        Assert.Empty(successes);
        Assert.Equal(new[] { "err1", "err2", "err3" }, failures);
    }


    [Fact]
    public async Task PartitionAsync_WhenGivenMixedResults_PartitionsCorrectly()
    {
        var (successes, failures) = await new[] {
            Task.FromResult(SuccessE<int, string>(1)),
            Task.FromResult(ErrorE<int, string>("err1")),
            Task.FromResult(SuccessE<int, string>(2)),
            Task.FromResult(ErrorE<int, string>("err2")),
            Task.FromResult(SuccessE<int, string>(3))
        }.PartitionAsync();
        Assert.Equal(new[] { 1, 2, 3 }, successes);
        Assert.Equal(new[] { "err1", "err2" }, failures);
    }


    [Fact]
    public async Task PartitionAsync_PreservesOrderOfElements()
    {
        var (successes, failures) = await new[] {
            Task.FromResult(SuccessE<int, string>(10)),
            Task.FromResult(SuccessE<int, string>(20)),
            Task.FromResult(ErrorE<int, string>("z")),
            Task.FromResult(ErrorE<int, string>("y")),
            Task.FromResult(SuccessE<int, string>(30))
        }.PartitionAsync();
        Assert.Equal(new[] { 10, 20, 30 }, successes);
        Assert.Equal(new[] { "z", "y" }, failures);
    }


    [Fact]
    public async Task PartitionAsync_WaitsForAllTasksToComplete()
    {
        var (successes, failures) = await new[] {
            Task.Run(async () => { await Task.Delay(10); return SuccessE<int, string>(1); }),
            Task.Run(async () => { await Task.Delay(5); return ErrorE<int, string>("err1"); }),
            Task.Run(async () => { await Task.Delay(15); return SuccessE<int, string>(2); })
        }.PartitionAsync();
        Assert.Equal(new[] { 1, 2 }, successes);
        Assert.Equal(new[] { "err1" }, failures);
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── Select ───────────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void Select_WhenInputIsSuccess_MapsValue()
    {
        Result<int, string> input = Success(21);

        var result = input.Select((x) => x * 2);

        Assert.Equal(42, result.ThrowIfError());
    }


    [Fact]
    public void Select_WhenInputIsError_ReturnsErrorWithoutInvokingSelector()
    {
        int numInvocations = 0;
        Result<int, string> input = Error("err1");

        var result = input.Select((x) =>
        {
            numInvocations++;
            return x * 2;
        });

        Assert.Equal("err1", result.ThrowIfSuccess());
        Assert.Equal(0, numInvocations);
    }


    [Fact]
    public void Select_WhenUsedWithQuerySyntax_MapsValue()
    {
        Result<int, string> input = Success(21);

        var result =
            from x in input
            select x * 2;

        Assert.Equal(42, result.ThrowIfError());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── SelectMany ───────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void SelectMany_WhenBothSucceed_ProjectsBothValues()
    {
        Result<int, string> input = Success(2);

        var result = input.SelectMany(
            (a) => SuccessE<string, string>(new string('x', a)),
            (a, b) => $"{a}:{b}");

        Assert.Equal("2:xx", result.ThrowIfError());
    }


    [Fact]
    public void SelectMany_WhenInputIsError_ReturnsErrorWithoutInvokingNextOrProject()
    {
        int numNextInvocations = 0;
        int numProjectInvocations = 0;
        Result<int, string> input = Error("err1");

        var result = input.SelectMany(
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

        Assert.Equal("err1", result.ThrowIfSuccess());
        Assert.Equal(0, numNextInvocations);
        Assert.Equal(0, numProjectInvocations);
    }


    [Fact]
    public void SelectMany_WhenNextReturnsError_ReturnsThatErrorWithoutInvokingProject()
    {
        int numProjectInvocations = 0;
        Result<int, string> input = Success(1);

        var result = input.SelectMany(
            (a) => ErrorE<int, string>("err2"),
            (a, b) =>
            {
                numProjectInvocations++;
                return a + b;
            });

        Assert.Equal("err2", result.ThrowIfSuccess());
        Assert.Equal(0, numProjectInvocations);
    }


    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntaxAndAllSucceed_CombinesValuesOfDifferentTypes()
    {
        var result =
            from count in SuccessE<int, string>(3)
            from text in SuccessE<string, string>("ab")
            from scale in SuccessE<double, string>(1.5)
            select $"{count}-{text}-{scale.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

        Assert.Equal("3-ab-1.5", result.ThrowIfError());
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

        var result =
            from a in SuccessE<int, string>(1)
            from b in ErrorE<int, string>("err2")
            from c in last()
            select a + b + c;

        Assert.Equal("err2", result.ThrowIfSuccess());
        Assert.Equal(0, numLastInvocations);
    }


    [Fact]
    public void SelectMany_WhenUsedWithQuerySyntax_AllowsLaterClausesToUseEarlierValues()
    {
        static Result<int, string> half(int x) =>
            x % 2 == 0 ?
                Success(x / 2) :
                Error($"{x} is odd");

        var success =
            from a in SuccessE<int, string>(8)
            from b in half(a)
            from c in half(b)
            select c;

        var error =
            from a in SuccessE<int, string>(6)
            from b in half(a)
            from c in half(b)
            select c;

        Assert.Equal(2, success.ThrowIfError());
        Assert.Equal("3 is odd", error.ThrowIfSuccess());
    }

    //
    // ══════════════════════════════════════════════════════════════════════
    // ── ThrowIfError ─────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void ThrowIfError_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        var val =
            SuccessE<int, string>(42)
            .ThrowIfError();

        Assert.Equal(42, val);
    }


    [Fact]
    public void ThrowIfError_WhenGivenErrorResult_ThrowsException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            ErrorE<int, string>("Error message")
            .ThrowIfError()
        );

        Assert.Contains("Expected successful Result but got error: Error message", ex.Message);
    }


    [Fact]
    public void ThrowIfError_WhenGivenErrorResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            ErrorE<int, string>("Error message")
            .ThrowIfError("Custom error")
        );

        Assert.Equal("Custom error", ex.Message);
    }


    // ── ThrowIfError (Tier 2 lift) ───────────────────────────────────

    [Fact]
    public async Task ThrowIfError_Task_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        var val = await Task.FromResult(SuccessE<int, string>(42)).ThrowIfError();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task ThrowIfError_Task_WhenGivenErrorResult_ThrowsException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(ErrorE<int, string>("Error message")).ThrowIfError());
        Assert.Contains("Expected successful Result but got error: Error message", ex.Message);
    }


    [Fact]
    public async Task ThrowIfError_Task_WhenGivenErrorResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
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
        var err =
            ErrorE<double, string>("Error message")
            .ThrowIfSuccess();

        Assert.Equal("Error message", err);
    }


    [Fact]
    public void ThrowIfSuccess_WhenGivenSuccessResult_ThrowsException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            SuccessE<int, string>(42)
            .ThrowIfSuccess()
        );

        Assert.Contains("Expected error Result but got success: 42", ex.Message);
    }


    [Fact]
    public void ThrowIfSuccess_WhenGivenSuccessResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            SuccessE<int, string>(42)
            .ThrowIfSuccess("Custom error")
        );

        Assert.Equal("Custom error", ex.Message);
    }


    // ── ThrowIfSuccess (Tier 2 lift) ────────────────────────────────────

    [Fact]
    public async Task ThrowIfSuccess_Task_WhenGivenErrorResult_ReturnsErrorValue()
    {
        var err = await Task.FromResult(ErrorE<int, string>("Error message")).ThrowIfSuccess();
        Assert.Equal("Error message", err);
    }


    [Fact]
    public async Task ThrowIfSuccess_Task_WhenGivenSuccessResult_ThrowsException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).ThrowIfSuccess());
        Assert.Contains("Expected error Result but got success: 42", ex.Message);
    }


    [Fact]
    public async Task ThrowIfSuccess_Task_WhenGivenSuccessResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
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

        Assert.Equal("alpha", result.ThrowIfError());
    }


    [Fact]
    public void ToResult_WhenGivenNullReference_ReturnsErrorFromFactory()
    {
        string? value = null;

        var result = value.ToResult(() => "missing value");

        Assert.Equal("missing value", result.ThrowIfSuccess());
    }


    [Fact]
    public void ToResult_WhenGivenNonNullReference_DoesNotInvokeErrorFactory()
    {
        string? value = "alpha";
        var invoked = false;

        var result = value.ToResult(() =>
        {
            invoked = true;
            return "missing value";
        });

        Assert.Equal("alpha", result.ThrowIfError());
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
        Option<string> input = Some("alpha");

        var result = input.ToResult(() => "missing value");

        Assert.Equal("alpha", result.ThrowIfError());
    }


    [Fact]
    public void ToResult_WhenGivenNoneOption_ReturnsErrorFromFactory()
    {
        Option<string> input = none;

        var result = input.ToResult(() => "missing value");

        Assert.Equal("missing value", result.ThrowIfSuccess());
    }


    [Fact]
    public void ToResult_WhenGivenSomeOption_DoesNotInvokeErrorFactory()
    {
        Option<string> input = Some("alpha");
        var invoked = false;

        var result = input.ToResult(() =>
        {
            invoked = true;
            return "missing value";
        });

        Assert.Equal("alpha", result.ThrowIfError());
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
        Option<string> input = none;

        Assert.Throws<ArgumentNullException>(() => input.ToResult<string, string>(null!));
    }


    // ── ToResult (Tier 2 lift) ──────────────────────────────────────────

    [Fact]
    public async Task ToResult_Task_WhenGivenSomeOption_ReturnsSuccess()
    {
        Task<Option<string>> input = Task.FromResult(Some("alpha"));

        var result = await input.ToResult(() => "missing value");

        Assert.Equal("alpha", result.ThrowIfError());
    }


    [Fact]
    public async Task ToResult_Task_WhenGivenNoneOption_ReturnsErrorFromFactory()
    {
        Task<Option<string>> input = Task.FromResult<Option<string>>(none);

        var result = await input.ToResult(() => "missing value");

        Assert.Equal("missing value", result.ThrowIfSuccess());
    }


    [Fact]
    public async Task ToResult_Task_WhenGivenSomeOption_DoesNotInvokeErrorFactory()
    {
        Task<Option<string>> input = Task.FromResult(Some("alpha"));
        var invoked = false;

        var result = await input.ToResult(() =>
        {
            invoked = true;
            return "missing value";
        });

        Assert.Equal("alpha", result.ThrowIfError());
        Assert.False(invoked);
    }


    [Fact]
    public async Task ToResult_Task_WhenGivenNullTask_ThrowsArgumentNullException()
    {
        Task<Option<string>> input = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await input.ToResult(() => "missing value"));
    }
}
