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
    // ── AssertError ──────────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void AssertError_WhenGivenErrorResult_ReturnsErrorValue()
    {
        var err =
            ErrorE<double, string>("Error message")
            .AssertError();

        Assert.Equal("Error message", err);
    }


    [Fact]
    public void AssertError_WhenGivenSuccessResult_ThrowsException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            SuccessE<int, string>(42)
            .AssertError()
        );

        Assert.Contains("Expected error Result but got success: 42", ex.Message);
    }


    [Fact]
    public void AssertError_WhenGivenSuccessResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            SuccessE<int, string>(42)
            .AssertError("Custom error")
        );

        Assert.Equal("Custom error", ex.Message);
    }


    // ── AssertError (Tier 2 lift) ────────────────────────────────────────

    [Fact]
    public async Task AssertError_Task_WhenGivenErrorResult_ReturnsErrorValue()
    {
        var err = await Task.FromResult(ErrorE<int, string>("Error message")).AssertError();
        Assert.Equal("Error message", err);
    }


    [Fact]
    public async Task AssertError_Task_WhenGivenSuccessResult_ThrowsException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).AssertError());
        Assert.Contains("Expected error Result but got success: 42", ex.Message);
    }


    [Fact]
    public async Task AssertError_Task_WhenGivenSuccessResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(SuccessE<int, string>(42)).AssertError("Custom error"));
        Assert.Equal("Custom error", ex.Message);
    }


    //
    // ══════════════════════════════════════════════════════════════════════
    // ── AssertSuccessful ─────────────────────────────────────────────────
    // ══════════════════════════════════════════════════════════════════════
    //

    [Fact]
    public void AssertSuccessful_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        var val =
            SuccessE<int, string>(42)
            .AssertSuccessful();

        Assert.Equal(42, val);
    }


    [Fact]
    public void AssertSuccessful_WhenGivenErrorResult_ThrowsException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            ErrorE<int, string>("Error message")
            .AssertSuccessful()
        );

        Assert.Contains("Expected successful Result but got error: Error message", ex.Message);
    }


    [Fact]
    public void AssertSuccessful_WhenGivenErrorResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            ErrorE<int, string>("Error message")
            .AssertSuccessful("Custom error")
        );

        Assert.Equal("Custom error", ex.Message);
    }


    // ── AssertSuccessful (Tier 2 lift) ───────────────────────────────────

    [Fact]
    public async Task AssertSuccessful_Task_WhenGivenSuccessResult_ReturnsSuccessValue()
    {
        var val = await Task.FromResult(SuccessE<int, string>(42)).AssertSuccessful();
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task AssertSuccessful_Task_WhenGivenErrorResult_ThrowsException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(ErrorE<int, string>("Error message")).AssertSuccessful());
        Assert.Contains("Expected successful Result but got error: Error message", ex.Message);
    }


    [Fact]
    public async Task AssertSuccessful_Task_WhenGivenErrorResultWithCustomMessage_ThrowsWithCustomMessage()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.FromResult(ErrorE<int, string>("Error message")).AssertSuccessful("Custom error"));
        Assert.Equal("Custom error", ex.Message);
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
            .AssertError();

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
            .AssertSuccessful();

        Assert.Equal(1, numInvocations);
        Assert.Equal(2d, val);
    }


    // ── Bind (Tier 2 lift — sync callback) ───────────────────────────────

    [Fact]
    public async Task Bind_Task_WhenGivenSuccessResult_ChainsCorrectly()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .Bind((x) => SuccessE<string, string>($"Value: {x}"));
        var val = result.AssertSuccessful();
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
        var err = result.AssertError();
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
        var val = result.AssertSuccessful();
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
        var err = result.AssertError();
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
        var err = result.AssertError();
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
            .AssertError();

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
            .AssertSuccessful();

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
            .AssertError();

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
        var val = result.AssertSuccessful();
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
        var err = result.AssertError();
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
        var err = result.AssertError();
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
        var val = result.AssertSuccessful();
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
        var err = result.AssertError();
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
        var err = result.AssertError();
        Assert.False(gateCalled);
        Assert.Equal("Input error", err);
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
            .AssertError();

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
            .AssertSuccessful();

        Assert.Equal(1, numInvocations);
        Assert.Equal(10, val);
    }


    [Fact]
    public void MapSuccess_WhenMappingTypeChanges_ReturnsCorrectType()
    {
        var val =
            SuccessE<int, string>(42)
            .MapSuccess(x => $"Value: {x}")
            .AssertSuccessful();

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
            .AssertSuccessful();

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
            .AssertError();

        Assert.Equal(0, invocations);
        Assert.Equal("Initial error", err);
    }


    // ── MapSuccess (Tier 2 lift — sync callback) ─────────────────────────

    [Fact]
    public async Task MapSuccess_Task_WhenGivenSuccessResult_TransformsValue()
    {
        var result = await Task.FromResult(SuccessE<int, string>(42))
            .MapSuccess((x) => x * 2);
        var val = result.AssertSuccessful();
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
        var err = result.AssertError();
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
        var val = result.AssertSuccessful();
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
        var err = result.AssertError();
        Assert.False(fnCalled);
        Assert.Equal("Error message", err);
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
            .AssertSuccessful();

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
            .AssertError();

        Assert.Equal(1, numInvocations);
        Assert.Equal("Original error (modified)", err);
    }


    [Fact]
    public void MapError_WhenMappingTypeChanges_ReturnsCorrectType()
    {
        var errVal =
            ErrorE<int, string>("Error message")
            .MapError(err => err.Length)
            .AssertError();

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
            .AssertError();

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
            .AssertSuccessful();

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
        var val = result.AssertSuccessful();
        Assert.False(fnCalled);
        Assert.Equal(42, val);
    }


    [Fact]
    public async Task MapError_Task_WhenGivenErrorResult_TransformsError()
    {
        var result = await Task.FromResult(ErrorE<int, string>("Error message"))
            .MapError((err) => $"Transformed: {err}");
        var err = result.AssertError();
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
        var val = result.AssertSuccessful();
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
        var err = result.AssertError();
        Assert.Equal("Transformed: Error message", err);
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
        var successCalled = false;
        var errorCalled = false;
        await Task.FromResult(SuccessE<int, string>(42))
            .Match(
                (s) => { successCalled = true; },
                (e) => { errorCalled = true; }
            );
        Assert.True(successCalled);
        Assert.False(errorCalled);
    }


    [Fact]
    public async Task Match_Task_VoidAction_WhenGivenErrorResult_ExecutesErrorFn()
    {
        var successCalled = false;
        var errorCalled = false;
        await Task.FromResult(ErrorE<int, string>("Error message"))
            .Match(
                (s) => { successCalled = true; },
                (e) => { errorCalled = true; }
            );
        Assert.False(successCalled);
        Assert.True(errorCalled);
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
}
