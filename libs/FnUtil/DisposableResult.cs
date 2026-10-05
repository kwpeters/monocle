namespace FnUtil;

/// <summary>
/// Wraps a <see cref="Result{TSuccess, TError}"/> whose success value is
/// <see cref="IDisposable"/>. Disposing this wrapper disposes the contained
/// value (if the result is successful and <see cref="Detach"/> has not been
/// called), so callers can write <c>using var result = …</c> on the result
/// itself as a safety net. To take ownership of the value, call
/// <see cref="Detach"/>.
/// </summary>
public sealed class DisposableResult<TSuccess, TError> : IDisposable
    where TSuccess : IDisposable
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static implicit operator DisposableResult<TSuccess, TError>(
        Result<TSuccess, TError> result) => new(result);

    //------------------------------------------------------------------------------
    // Instance fields
    //------------------------------------------------------------------------------

    private readonly Result<TSuccess, TError> _result;
    private bool _disposed;
    private bool _detached;

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    internal DisposableResult(Result<TSuccess, TError> result) => _result = result;

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    public bool IsSuccess => _result.IsSuccess;

    public bool IsError => _result.IsError;

    //------------------------------------------------------------------------------
    // Instance methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Extracts the success value and transfers ownership to the caller.
    /// After calling this, <see cref="Dispose"/> will no longer dispose the
    /// value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the result is an error.
    /// </exception>
    public TSuccess Detach()
    {
        _detached = true;
        return _result.Match(
            (s) => s,
            (e) => throw new InvalidOperationException(
                $"Cannot detach value from an error result: {e}")
        );
    }


    /// <summary>
    /// Throws an InvalidOperationException if the result is a success, otherwise returns the error value.
     /// </summary>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TError GetError()
    {
        return _result.Match(
            (s) => throw new InvalidOperationException($"Expected error result but got success: {s}"),
            (e) => e
        );
    }


    /// <summary>
    /// Throws an InvalidOperationException if the result is a success, otherwise returns the error value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the result is a success.
    /// </exception>
    public TError ThrowIfSuccess(string? errMsg = null)
    {
        return _result.Match(
            (s) => throw new InvalidOperationException(errMsg ?? $"Expected error result but got success: {s}"),
            (e) => e
        );
    }


    /// <summary>
    /// Pattern matches on the underlying result.
    /// </summary>
    public TResult
    Match<TResult>(
        Func<TSuccess, TResult> successFn,
        Func<TError, TResult> errorFn
    )
    {
        return _result.Match(successFn, errorFn);
    }

    /// <summary>
    /// Pattern matches on the underlying result for side-effects only.
    /// </summary>
    public void
    Match(
        Action<TSuccess> successAction,
        Action<TError> errorAction
    )
    {
        _result.Match(successAction, errorAction);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (!_detached && _result.IsSuccess)
        {
            _result.Match(
                (s) => s.Dispose(),
                (e) => { }
            );
        }
    }
}


/// <summary>
/// Non-generic factory methods for <see cref="DisposableResult{TSuccess, TError}"/>.
/// </summary>
public static class DisposableResult
{
    //------------------------------------------------------------------------------
    // Static factory methods
    //------------------------------------------------------------------------------

    public static DisposableResult<TSuccess, TError>
    Success<TSuccess, TError>(TSuccess value)
        where TSuccess : IDisposable
    {
        Result<TSuccess, TError> result = F.Success(value);
        return new DisposableResult<TSuccess, TError>(result);
    }


    public static DisposableResult<TSuccess, TError>
    Error<TSuccess, TError>(TError error)
        where TSuccess : IDisposable
    {
        Result<TSuccess, TError> result = F.Error(error);
        return new DisposableResult<TSuccess, TError>(result);
    }
}
