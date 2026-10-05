namespace FnUtil;

/// <summary>
/// Wraps a <see cref="Result{TSuccess, TError}"/> whose success value is
/// <see cref="IAsyncDisposable"/>. Disposing this wrapper asynchronously
/// disposes the contained value (if the result is successful and
/// <see cref="Detach"/> has not been called), so callers can write
/// <c>await using var result = …</c> on the result itself as a safety net.
/// To take ownership of the value, call <see cref="Detach"/>.
/// </summary>
public sealed class AsyncDisposableResult<TSuccess, TError> : IAsyncDisposable
    where TSuccess : IAsyncDisposable
{
    private readonly Result<TSuccess, TError> _result;
    private bool _disposed;
    private bool _detached;

    internal AsyncDisposableResult(Result<TSuccess, TError> result) => this._result = result;

    public bool IsSuccess => this._result.IsSuccess;

    public bool IsError => this._result.IsError;

    /// <summary>
    /// Extracts the success value and transfers ownership to the caller.
    /// After calling this, <see cref="DisposeAsync"/> will no longer dispose
    /// the value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the result is an error.
    /// </exception>
    public TSuccess Detach()
    {
        this._detached = true;
        return this._result.Match(
            (s) => s,
            (e) => throw new InvalidOperationException(
                $"Cannot detach value from an error result: {e}")
        );
    }

    /// <summary>
    /// Asserts that the result is an error and returns the error value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the result is a success.
    /// </exception>
    public TError AssertError(string? errMsg = null)
    {
        return this._result.Match(
            (s) => throw new InvalidOperationException(errMsg ?? $"Expected error result but got success: {s}"),
            (e) => e
        );
    }

    /// <summary>
    /// Pattern matches on the underlying result.
    /// </summary>
    public TResult Match<TResult>(
        Func<TSuccess, TResult> successFn,
        Func<TError, TResult> errorFn
    )
    {
        return this._result.Match(successFn, errorFn);
    }

    /// <summary>
    /// Pattern matches on the underlying result for side-effects only.
    /// </summary>
    public void Match(
        Action<TSuccess> successAction,
        Action<TError> errorAction
    )
    {
        this._result.Match(successAction, errorAction);
    }

    public async ValueTask DisposeAsync()
    {
        if (this._disposed)
        {
            return;
        }

        this._disposed = true;
        if (!this._detached && this._result.IsSuccess)
        {
            await this._result.Match(
                (s) => s.DisposeAsync(),
                (e) => ValueTask.CompletedTask
            ).ConfigureAwait(false);
        }
    }

    public static implicit operator AsyncDisposableResult<TSuccess, TError>(
        Result<TSuccess, TError> result) => new(result);
}


/// <summary>
/// Non-generic factory methods for <see cref="AsyncDisposableResult{TSuccess, TError}"/>.
/// </summary>
public static class AsyncDisposableResult
{
    public static AsyncDisposableResult<TSuccess, TError> Success<TSuccess, TError>(TSuccess value)
        where TSuccess : IAsyncDisposable
    {
        Result<TSuccess, TError> result = F.Success(value);
        return new AsyncDisposableResult<TSuccess, TError>(result);
    }

    public static AsyncDisposableResult<TSuccess, TError> Error<TSuccess, TError>(TError error)
        where TSuccess : IAsyncDisposable
    {
        Result<TSuccess, TError> result = F.Error(error);
        return new AsyncDisposableResult<TSuccess, TError>(result);
    }
}
