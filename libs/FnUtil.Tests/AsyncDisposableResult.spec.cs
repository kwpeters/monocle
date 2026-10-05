namespace FnUtil.Tests;


public class AsyncDisposableResultTests
{
    private sealed class FakeAsyncDisposable : IAsyncDisposable
    {
        public bool IsDisposed { get; private set; }
        public ValueTask DisposeAsync()
        {
            this.IsDisposed = true;
            return ValueTask.CompletedTask;
        }
    }

    [Fact]
    public async Task Ok_IsSuccess_ReturnsTrue()
    {
        await using var inner = new FakeAsyncDisposable();
        await using var dr = AsyncDisposableResult.Success<FakeAsyncDisposable, string>(inner);
        Assert.True(dr.IsSuccess);
        Assert.False(dr.IsError);
    }

    [Fact]
    public async Task Fail_IsError_ReturnsTrue()
    {
        await using var dr = AsyncDisposableResult.Error<FakeAsyncDisposable, string>("boom");
        Assert.True(dr.IsError);
        Assert.False(dr.IsSuccess);
    }

    [Fact]
    public async Task Detach_ReturnsValue_And_DisposeAsync_DoesNotDisposeValue()
    {
        await using var inner = new FakeAsyncDisposable();
        var dr = AsyncDisposableResult.Success<FakeAsyncDisposable, string>(inner);

        var detached = dr.Detach();
        Assert.Same(inner, detached);

        await dr.DisposeAsync();
        Assert.False(inner.IsDisposed);
    }

    [Fact]
    public async Task DisposeAsync_DisposesValue_WhenNotDetached()
    {
        await using var inner = new FakeAsyncDisposable();
        var dr = AsyncDisposableResult.Success<FakeAsyncDisposable, string>(inner);

        await dr.DisposeAsync();
        Assert.True(inner.IsDisposed);
    }

    [Fact]
    public async Task DisposeAsync_IsIdempotent()
    {
        await using var inner = new FakeAsyncDisposable();
        var dr = AsyncDisposableResult.Success<FakeAsyncDisposable, string>(inner);

        await dr.DisposeAsync();
        await dr.DisposeAsync(); // should not throw
        Assert.True(inner.IsDisposed);
    }

    [Fact]
    public async Task DisposeAsync_OnError_DoesNotThrow()
    {
        var dr = AsyncDisposableResult.Error<FakeAsyncDisposable, string>("fail");
        await dr.DisposeAsync(); // should not throw
    }

    [Fact]
    public async Task Detach_OnError_Throws()
    {
        await using var dr = AsyncDisposableResult.Error<FakeAsyncDisposable, string>("fail");
        Assert.Throws<InvalidOperationException>(dr.Detach);
    }

    [Fact]
    public async Task Match_OnSuccess_CallsSuccessFn()
    {
        await using var inner = new FakeAsyncDisposable();
        await using var dr = AsyncDisposableResult.Success<FakeAsyncDisposable, string>(inner);

        var result = dr.Match(
            (s) => "ok",
            (e) => "error"
        );
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Match_OnError_CallsErrorFn()
    {
        await using var dr = AsyncDisposableResult.Error<FakeAsyncDisposable, string>("boom");

        var result = dr.Match(
            (s) => "ok",
            (e) => e
        );
        Assert.Equal("boom", result);
    }

    [Fact]
    public async Task MatchVoid_OnSuccess_CallsSuccessAction()
    {
        await using var inner = new FakeAsyncDisposable();
        await using var dr = AsyncDisposableResult.Success<FakeAsyncDisposable, string>(inner);

        var called = false;
        dr.Match(
            (s) => { called = true; },
            (e) => { Assert.Fail("Should not call error action"); }
        );
        Assert.True(called);
    }

    [Fact]
    public async Task MatchVoid_OnError_CallsErrorAction()
    {
        await using var dr = AsyncDisposableResult.Error<FakeAsyncDisposable, string>("boom");

        var errorValue = "";
        dr.Match(
            (s) => { Assert.Fail("Should not call success action"); },
            (e) => { errorValue = e; }
        );
        Assert.Equal("boom", errorValue);
    }

    [Fact]
    public async Task ImplicitConversion_FromResult_Works()
    {
        await using var inner = new FakeAsyncDisposable();
        Result<FakeAsyncDisposable, string> result = F.Success(inner);

        AsyncDisposableResult<FakeAsyncDisposable, string> dr = result;
        Assert.True(dr.IsSuccess);

        var detached = dr.Detach();
        Assert.Same(inner, detached);
        await dr.DisposeAsync();
    }

    [Fact]
    public async Task AssertError_OnError_ReturnsError()
    {
        await using var dr = AsyncDisposableResult.Error<FakeAsyncDisposable, string>("boom");
        Assert.Equal("boom", dr.AssertError());
    }

    [Fact]
    public async Task AssertError_OnSuccess_Throws()
    {
        await using var inner = new FakeAsyncDisposable();
        await using var dr = AsyncDisposableResult.Success<FakeAsyncDisposable, string>(inner);
        Assert.Throws<InvalidOperationException>(() => dr.AssertError());
    }
}
