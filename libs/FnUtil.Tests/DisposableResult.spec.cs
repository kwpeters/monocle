namespace FnUtil.Tests;

public class DisposableResultTests
{
    private sealed class FakeDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose() => IsDisposed = true;
    }

    [Fact]
    public void Ok_IsSuccess_ReturnsTrue()
    {
        using var inner = new FakeDisposable();
        using var dr = DisposableResult.Success<FakeDisposable, string>(inner);
        Assert.True(dr.IsSuccess);
        Assert.False(dr.IsError);
    }

    [Fact]
    public void Fail_IsError_ReturnsTrue()
    {
        using var dr = DisposableResult.Error<FakeDisposable, string>("boom");
        Assert.True(dr.IsError);
        Assert.False(dr.IsSuccess);
    }

    [Fact]
    public void Detach_ReturnsValue_And_Dispose_DoesNotDisposeValue()
    {
        using var inner = new FakeDisposable();
        var dr = DisposableResult.Success<FakeDisposable, string>(inner);

        var detached = dr.Detach();
        Assert.Same(inner, detached);

        dr.Dispose();
        Assert.False(inner.IsDisposed);
    }

    [Fact]
    public void Dispose_DisposesValue_WhenNotDetached()
    {
        using var inner = new FakeDisposable();
        var dr = DisposableResult.Success<FakeDisposable, string>(inner);

        dr.Dispose();
        Assert.True(inner.IsDisposed);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        using var inner = new FakeDisposable();
        var dr = DisposableResult.Success<FakeDisposable, string>(inner);

        dr.Dispose();
        dr.Dispose(); // should not throw
        Assert.True(inner.IsDisposed);
    }

    [Fact]
    public void Dispose_OnError_DoesNotThrow()
    {
        var dr = DisposableResult.Error<FakeDisposable, string>("fail");
        dr.Dispose(); // should not throw
    }

    [Fact]
    public void Detach_OnError_Throws()
    {
        using var dr = DisposableResult.Error<FakeDisposable, string>("fail");
        Assert.Throws<InvalidOperationException>(dr.Detach);
    }

    [Fact]
    public void Match_OnSuccess_CallsSuccessFn()
    {
        using var inner = new FakeDisposable();
        using var dr = DisposableResult.Success<FakeDisposable, string>(inner);

        var result = dr.Match(
            (s) => "ok",
            (e) => "error"
        );
        Assert.Equal("ok", result);
    }

    [Fact]
    public void Match_OnError_CallsErrorFn()
    {
        using var dr = DisposableResult.Error<FakeDisposable, string>("boom");

        var result = dr.Match(
            (s) => "ok",
            (e) => e
        );
        Assert.Equal("boom", result);
    }

    [Fact]
    public void MatchVoid_OnSuccess_CallsSuccessAction()
    {
        using var inner = new FakeDisposable();
        using var dr = DisposableResult.Success<FakeDisposable, string>(inner);

        var called = false;
        dr.Match(
            (s) => called = true,
            (e) => Assert.Fail("Should not call error action"));
        Assert.True(called);
    }

    [Fact]
    public void MatchVoid_OnError_CallsErrorAction()
    {
        using var dr = DisposableResult.Error<FakeDisposable, string>("boom");

        var errorValue = "";
        dr.Match(
            (s) => Assert.Fail("Should not call success action"),
            (e) => errorValue = e);
        Assert.Equal("boom", errorValue);
    }

    [Fact]
    public void ImplicitConversion_FromResult_Works()
    {
        using var inner = new FakeDisposable();
        Result<FakeDisposable, string> result = Success(inner);

        DisposableResult<FakeDisposable, string> dr = result;
        Assert.True(dr.IsSuccess);

        var detached = dr.Detach();
        Assert.Same(inner, detached);
        dr.Dispose();
    }

    [Fact]
    public void ThrowIfSuccess_OnError_ReturnsError()
    {
        var dr = DisposableResult.Error<FakeDisposable, string>("boom");
        Assert.Equal("boom", dr.ThrowIfSuccess());
    }

    [Fact]
    public void ThrowIfSuccess_OnSuccess_Throws()
    {
        using var inner = new FakeDisposable();
        var dr = DisposableResult.Success<FakeDisposable, string>(inner);
        Assert.Throws<InvalidOperationException>(() => dr.ThrowIfSuccess());
    }
}
