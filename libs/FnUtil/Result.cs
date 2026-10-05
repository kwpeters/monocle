using System.Reflection.Metadata.Ecma335;

namespace FnUtil;


/// <summary>
/// A result that could either succeed and yield a value or fail with an error
/// </summary>
/// <typeparam name="TSuccess">The type of successful values</typeparam>
/// <typeparam name="TError">The type of error values</typeparam>
public abstract record Result<TSuccess, TError>
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

#pragma warning disable CA2225      // Explicit conversion method not needed.
    public static implicit operator Result<TSuccess, TError>(ResultSuccessType<TSuccess> s) => new SuccessResult<TSuccess, TError>(s.Value);

    public static implicit operator Result<TSuccess, TError>(ResultErrorType<TError> e) => new ErrorResult<TSuccess, TError>(e.Error);
#pragma warning restore CA2225

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    public abstract bool IsSuccess { get; }
    public bool IsError => !this.IsSuccess;
}

/// <summary>
/// Concrete Result type wrapping a success value.
///
/// This type is not public, because clients should use F.Success() instead.
/// </summary>
/// <typeparam name="TSuccess">The type of successful values</typeparam>
/// <typeparam name="TError">The type of error values</typeparam>
/// <param name="Value">The successful value</param>
internal sealed record SuccessResult<TSuccess, TError>(TSuccess Value) : Result<TSuccess, TError>
{
    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    public override bool IsSuccess => true;
}


/// <summary>
/// Concrete Result type wrapping an error value.
///
/// This type is not public, because clients should use F.Error() instead.
/// </summary>
/// <typeparam name="TSuccess">The type of successful values</typeparam>
/// <typeparam name="TError">The type of error values</typeparam>
/// <param name="Error"></param>
internal sealed record ErrorResult<TSuccess, TError>(TError Error) : Result<TSuccess, TError>
{
    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    public override bool IsSuccess => false;
}


////////////////////////////////////////////////////////////////////////////////
// Helper Functions
////////////////////////////////////////////////////////////////////////////////
public static partial class F
{
    //------------------------------------------------------------------------------
    // Static factory methods
    //------------------------------------------------------------------------------

    public static ResultSuccessType<TSuccess>
    Success<TSuccess>(TSuccess value) => new(value);

    // Provides a way of creating a success Result by explicitly defining the
    // error type.  The appended "E" stands for "explicit."
    public static Result<TSuccess, TError>
    SuccessE<TSuccess, TError>(TSuccess value)
    {
        Result<TSuccess, TError> res = Success(value);
        return res;
    }

    public static ResultErrorType<TError>
    Error<TError>(TError error) => new(error);

    // Provides a way of creating an error Result by explicitly defining the
    // success type.  The appended "E" stands for "explicit."
    public static Result<TSuccess, TError>
    ErrorE<TSuccess, TError>(TError error)
    {
        Result<TSuccess, TError> res = Error(error);
        return res;
    }
}


////////////////////////////////////////////////////////////////////////////////
// Support
////////////////////////////////////////////////////////////////////////////////

// "Marker type" representing a successful Result.  This type only uses the
// generic type parameters needed and does not use all of the type parameters
// specified in Result.  This type only exists so that Result can have an
// implicit conversion operator where all unused generic type parameters can be
// inferred from the context.
public readonly record struct ResultSuccessType<TSuccess>(TSuccess Value);

// "Marker type" representing an error Result.  This type only uses the generic
// type parameters needed and does not use all of the type parameters specified
// in Result.  This type only exists so that Result can have an implicit
// conversion operator where all unused generic type parameters can be inferred
// from the context.
public readonly record struct ResultErrorType<TError>(TError Error);
