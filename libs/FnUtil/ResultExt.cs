//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

using static FnUtil.F;

namespace FnUtil;


public static class ResultExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Converts a nullable reference to a Result explicitly.
    /// Returns Success(value) when value is non-null; otherwise returns
    /// Error(errorFactory()).
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="value">The nullable input value.</param>
    /// <param name="errorFactory">
    /// Factory for the error value. This factory is invoked only when value is null.
    /// </param>
    public static Result<TSuccess, TError>
    ToResult<TSuccess, TError>(
        this TSuccess? value,
        Func<TError> errorFactory
    ) where TSuccess : class
    {
        ArgumentNullException.ThrowIfNull(errorFactory);

        return value is null ?
            Error(errorFactory()) :
            Success(value);
    }

    /// <summary>
    /// Converts an Option to a Result explicitly.
    /// Returns Success(value) when the Option is Some; otherwise returns
    /// Error(errorFactory()).
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="input">The Option input value.</param>
    /// <param name="errorFactory">
    /// Factory for the error value. This factory is invoked only when the Option is None.
    /// </param>
    public static Result<TSuccess, TError>
    ToResult<TSuccess, TError>(
        this Option<TSuccess> input,
        Func<TError> errorFactory
    )
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(errorFactory);

        return input.Match<TSuccess, Result<TSuccess, TError>>(
            someFn: (value) => Success(value),
            noneFn: () => Error(errorFactory())
        );
    }

    /// <summary>
    /// Converts a Result to an Option explicitly.
    /// Returns Some(value) when the Result is success; otherwise returns none.
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="input">The Result input value.</param>
    public static Option<TSuccess>
    ToOption<TSuccess, TError>(this Result<TSuccess, TError> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.Match<TSuccess, TError, Option<TSuccess>>(
            successFn: (value) => Some(value),
            errorFn: (_) => none
        );
    }

    /// <summary>
    /// Converts a Task-wrapped Result to an Option explicitly.
    /// Returns Some(value) when the Result is success; otherwise returns none.
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="taskInput">A task that resolves to a Result.</param>
    public static async Task<Option<TSuccess>>
    ToOption<TSuccess, TError>(this Task<Result<TSuccess, TError>> taskInput)
    {
        ArgumentNullException.ThrowIfNull(taskInput);

        var input = await taskInput.ConfigureAwait(false);
        return input.ToOption();
    }


    /// <summary>
    /// Monadic bind operation for Result.  If the input Result is successful,
    /// applies the provided function to the success value and returns its
    /// Result. If the input Result is an error, propagates the error without
    /// invoking the function.  This allows chaining operations that may fail.
    /// </summary>
    /// <typeparam name="TInSuccess">
    /// The type of the success value in the input Result.
    /// </typeparam>
    /// <typeparam name="TOutSuccess">
    /// The type of the success value in the output Result.
    /// </typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result to bind.</param>
    /// <param name="fn">
    /// A function that takes the success value and returns a new Result. This
    /// function is only invoked if the input Result is successful.
    /// </param>
    /// <returns>
    /// The Result returned by fn if the input is successful, or the original
    /// error if the input is an error.
    /// </returns>
    public static Result<TOutSuccess, TError>
    Bind<TInSuccess, TOutSuccess, TError>(
        this Result<TInSuccess, TError> input,
        Func<TInSuccess, Result<TOutSuccess, TError>> fn
    )
    {
        return input.Match(
            successFn: (val) => fn(val),
            errorFn: (err) => Error(err)
        );
    }

    /// <summary>
    /// Monadic bind for a Task-wrapped Result with a synchronous function.
    /// </summary>
    public static async Task<Result<TOutSuccess, TError>>
    Bind<TInSuccess, TOutSuccess, TError>(
        this Task<Result<TInSuccess, TError>> taskInput,
        Func<TInSuccess, Result<TOutSuccess, TError>> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.Bind(fn);
    }

    /// <summary>
    /// Monadic bind for a Task-wrapped Result with an asynchronous function.
    /// </summary>
    public static async Task<Result<TOutSuccess, TError>>
    BindAsync<TInSuccess, TOutSuccess, TError>(
        this Task<Result<TInSuccess, TError>> taskInput,
        Func<TInSuccess, Task<Result<TOutSuccess, TError>>> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        Result<TOutSuccess, TError> result = input switch {
            SuccessResult<TInSuccess, TError>(var s) => await fn(s).ConfigureAwait(false),
            ErrorResult<TInSuccess, TError>(var e) => Error(e),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


    /// <summary>
    /// Returns the success value of a Result or a default value if the Result
    /// is an error.
    /// </summary>
    /// <typeparam name="TSuccess"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <param name="input"></param>
    /// <param name="defaultValue"></param>
    /// <returns>
    /// The success value if the Result is successful, or the default value if the Result is an error.
    /// </returns>
    public static TSuccess
    DefaultValue<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        TSuccess defaultValue
    )
    {
        TSuccess val = input.Match(
            (s) => s,
            (e) => defaultValue
        );
        return val;
    }

    /// <summary>
    /// Returns the success value of a Task-wrapped Result or a default value
    /// if the Result is an error.
    /// </summary>
    public static async Task<TSuccess>
    DefaultValue<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        TSuccess defaultValue
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.DefaultValue(defaultValue);
    }


    /// <summary>
    /// Returns the success value of a Result or computes a default value using
    /// the provided function if the Result is an error.  This allows the
    /// default value to be computed based on the error value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="defaultValueFunc">
    /// A function that takes the error value and returns a default success
    /// value. This function is only invoked if the input Result is an error.
    /// </param>
    /// <returns>
    /// The success value if the Result is successful, or the computed default
    /// value if the Result is an error.
    /// </returns>
    public static TSuccess
    DefaultWith<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Func<TError, TSuccess> defaultValueFunc
    )
    {
        TSuccess val = input.Match(
            (s) => s,
            (e) => defaultValueFunc(e)
        );
        return val;
    }

    /// <summary>
    /// Returns the success value of a Task-wrapped Result or computes a default
    /// value using the provided synchronous function if the Result is an error.
    /// </summary>
    public static async Task<TSuccess>
    DefaultWith<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TError, TSuccess> defaultValueFunc
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.DefaultWith(defaultValueFunc);
    }

    /// <summary>
    /// Returns the success value of a Task-wrapped Result or computes a default
    /// value using the provided asynchronous function if the Result is an
    /// error.
    /// </summary>
    public static async Task<TSuccess>
    DefaultWithAsync<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TError, Task<TSuccess>> defaultValueFunc
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        TSuccess result = input switch {
            SuccessResult<TSuccess, TError>(var s) => s,
            ErrorResult<TSuccess, TError>(var e) => await defaultValueFunc(e).ConfigureAwait(false),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


    /// <summary>
    /// Gates a Result by applying a validation function. If the input Result is
    /// successful, applies the gate function to validate the success value. If
    /// the gate function returns success, returns the original input Result. If
    /// the gate function returns an error, propagates that error. If the input
    /// Result is already an error, propagates it without invoking the gate
    /// function.  This is useful for adding validation steps that don't
    /// transform the value but may fail the operation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TFnSuccess">
    /// The type of the success value returned by the gate function (typically
    /// ignored, as only success/error status matters).
    /// </typeparam>
    /// <param name="input">The input Result to gate.</param>
    /// <param name="gateFn">
    /// A validation function that takes the success value and returns a Result.
    /// This function is only invoked if the input Result is successful. The
    /// success value of this function is ignored; only whether it succeeds or
    /// fails matters.
    /// </param>
    /// <returns>
    /// The original input Result if both the input and gate function are
    /// successful, or the first error encountered (either from the input or the
    /// gate function).
    /// </returns>
    public static Result<TSuccess, TError>
    Gate<TSuccess, TError, TFnSuccess>(
        this Result<TSuccess, TError> input,
        Func<TSuccess, Result<TFnSuccess, TError>> gateFn
    )
    {
        return input.Match(
            (sInput) => {
                Result<TFnSuccess, TError> gateRes = gateFn(sInput);
                Result<TSuccess, TError> ret = gateRes.Match<TFnSuccess, TError, Result<TSuccess, TError>>(
                    (_) => Success(sInput),
                    (e) => Error(e)
                );
                return ret;
            },
            (e) => Error(e)
        );
    }

    /// <summary>
    /// Gates a Task-wrapped Result by applying a synchronous validation
    /// function.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    Gate<TSuccess, TError, TFnSuccess>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TSuccess, Result<TFnSuccess, TError>> gateFn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.Gate(gateFn);
    }

    /// <summary>
    /// Gates a Task-wrapped Result by applying an asynchronous validation
    /// function.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    GateAsync<TSuccess, TError, TFnSuccess>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TSuccess, Task<Result<TFnSuccess, TError>>> gateFn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        switch (input)
        {
            case SuccessResult<TSuccess, TError>(var s):
            {
                var gateRes = await gateFn(s).ConfigureAwait(false);
                return gateRes.Match<TFnSuccess, TError, Result<TSuccess, TError>>(
                    (_) => Success(s),
                    (e) => Error(e)
                );
            }
            case ErrorResult<TSuccess, TError>(var e):
                return Error(e);
            default:
                throw new ArgumentException("Result must be SuccessResult or ErrorResult.");
        }
    }


    /// <summary>
    /// Returns the error value from a Result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <returns>The error value if the Result is an error.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the Result is successful.
    /// </exception>
    public static TError
    GetError<TSuccess, TError>(this Result<TSuccess, TError> input)
    {
        return input.Match(
            (s) => throw new InvalidOperationException("Cannot access Error on a SuccessResult."),
            (e) => e
        );
    }

    /// <summary>
    /// Returns the error value from a Task-wrapped Result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="taskInput">A task that resolves to a Result.</param>
    /// <returns>A task containing the error value if the Result is an error.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the Result is successful.
    /// </exception>
    public static async Task<TError>
    GetError<TSuccess, TError>(this Task<Result<TSuccess, TError>> taskInput)
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.GetError();
    }


    /// <summary>
    /// Returns the success value from a Result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <returns>The success value if the Result is successful.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the Result is an error.
    /// </exception>
    public static TSuccess
    GetValue<TSuccess, TError>(this Result<TSuccess, TError> input)
    {
        return input.Match(
            (s) => s,
            (e) => throw new InvalidOperationException("Cannot access Value on an ErrorResult.")
        );
    }

    /// <summary>
    /// Returns the success value from a Task-wrapped Result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="taskInput">A task that resolves to a Result.</param>
    /// <returns>A task containing the success value if the Result is successful.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the Result is an error.
    /// </exception>
    public static async Task<TSuccess>
    GetValue<TSuccess, TError>(this Task<Result<TSuccess, TError>> taskInput)
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.GetValue();
    }


    //--------------------------------------------------------------------------
    // MapSuccess
    //--------------------------------------------------------------------------

    /// <summary>
    /// Maps the success value of a Result to a new value using the provided
    /// function.  If the input Result is successful, applies the function to
    /// the success value and wraps the result in a successful Result. If the
    /// input Result is an error, propagates the error without invoking the
    /// function.
    /// </summary>
    /// <typeparam name="TInSuccess">
    /// The type of the success value in the input Result.
    /// </typeparam>
    /// <typeparam name="TOutSuccess">
    /// The type of the success value in the output Result.
    /// </typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result to map.</param>
    /// <param name="fn">
    /// A function that transforms the success value. This function is only
    /// invoked if the input Result is successful.
    /// </param>
    /// <returns>
    /// A successful Result containing the transformed value if the input is
    /// successful, or the original error if the input is an error.
    /// </returns>
    public static Result<TOutSuccess, TError>
    MapSuccess<TInSuccess, TOutSuccess, TError>(
        this Result<TInSuccess, TError> input,
        Func<TInSuccess, TOutSuccess> fn
    )
    {
        return input.Match<TInSuccess, TError, Result<TOutSuccess, TError>>(
            successFn: (val) => Success(fn(val)),
            errorFn: (err) => Error(err)
        );
    }

    /// <summary>
    /// Maps the success value of a Task-wrapped Result using a synchronous
    /// function.
    /// </summary>
    public static async Task<Result<TOutSuccess, TError>>
    MapSuccess<TInSuccess, TOutSuccess, TError>(
        this Task<Result<TInSuccess, TError>> taskInput,
        Func<TInSuccess, TOutSuccess> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.MapSuccess(fn);
    }

    /// <summary>
    /// Maps the success value of a Task-wrapped Result using an asynchronous
    /// function.
    /// </summary>
    public static async Task<Result<TOutSuccess, TError>>
    MapSuccessAsync<TInSuccess, TOutSuccess, TError>(
        this Task<Result<TInSuccess, TError>> taskInput,
        Func<TInSuccess, Task<TOutSuccess>> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        Result<TOutSuccess, TError> result = input switch {
            SuccessResult<TInSuccess, TError>(var s) => Success(await fn(s).ConfigureAwait(false)),
            ErrorResult<TInSuccess, TError>(var e) => Error(e),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


    /// <summary>
    /// Maps the error value of a Result to a new value using the provided
    /// function.  If the input Result is successful, propagates the success
    /// value without invoking the function. If the input Result is an error,
    /// applies the function to the error value and wraps the result in a
    /// successful Result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TInError">The type of the error value in the input Result.</typeparam>
    /// <typeparam name="TOutError">The type of the error value in the output Result.</typeparam>
    /// <param name="input">The input Result to map.</param>
    /// <param name="fn">
    /// A function that transforms the error value. This function is only
    /// invoked if the input Result is an error.
    /// </param>
    /// <returns>
    /// A successful Result containing the transformed value if the input is
    /// an error, or the original success if the input is successful.
    /// </returns>
    public static Result<TSuccess, TOutError>
    MapError<TSuccess, TInError, TOutError>(
        this Result<TSuccess, TInError> input,
        Func<TInError, TOutError> fn
    )
    {
        return input.Match<TSuccess, TInError, Result<TSuccess, TOutError>>(
            (s) => Success(s),
            (e) => Error(fn(e))
        );
    }

    /// <summary>
    /// Maps the error value of a Task-wrapped Result using a synchronous
    /// function.
    /// </summary>
    public static async Task<Result<TSuccess, TOutError>>
    MapError<TSuccess, TInError, TOutError>(
        this Task<Result<TSuccess, TInError>> taskInput,
        Func<TInError, TOutError> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.MapError(fn);
    }

    /// <summary>
    /// Maps the error value of a Task-wrapped Result using an asynchronous
    /// function.
    /// </summary>
    public static async Task<Result<TSuccess, TOutError>>
    MapErrorAsync<TSuccess, TInError, TOutError>(
        this Task<Result<TSuccess, TInError>> taskInput,
        Func<TInError, Task<TOutError>> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        Result<TSuccess, TOutError> result = input switch {
            SuccessResult<TSuccess, TInError>(var s) => Success(s),
            ErrorResult<TSuccess, TInError>(var e) => Error(await fn(e).ConfigureAwait(false)),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


    /// <summary>
    /// Pattern matches on a Result, executing one of two functions depending on
    /// whether the Result is successful or an error, and returns a value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by both functions.</typeparam>
    /// <param name="res">The Result to pattern match on.</param>
    /// <param name="successFn">
    /// A function to execute if the Result is successful, receiving the success
    /// value.
    /// </param>
    /// <param name="errorFn">
    /// A function to execute if the Result is an error, receiving the error
    /// value.
    /// </param>
    /// <returns>
    /// The result of executing successFn if the Result is successful, or the
    /// result of executing errorFn if the Result is an error.
    /// </returns>
    public static TResult
    Match<TSuccess, TError, TResult>(
        this Result<TSuccess, TError> res,
        Func<TSuccess, TResult> successFn,
        Func<TError, TResult> errorFn
    )
    {
        return res switch {
            SuccessResult<TSuccess, TError>(var s) => successFn(s),
            ErrorResult<TSuccess, TError>(var e) => errorFn(e),
            // This line should not be necessary, but we are dealing with C#.
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
    }

    /// <summary>
    /// Pattern matches on a Result, executing one of two actions depending on
    /// whether the Result is successful or an error.  This overload is for
    /// side-effects only and does not return a value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="res">The Result to pattern match on.</param>
    /// <param name="successFn">
    /// An action to execute if the Result is successful, receiving the success
    /// value.
    /// </param>
    /// <param name="errorFn">
    /// An action to execute if the Result is an error, receiving the error
    /// value.
    /// </param>
    public static void
    Match<TSuccess, TError>(
        this Result<TSuccess, TError> res,
        Action<TSuccess> successFn,
        Action<TError> errorFn
    )
    {
        res.Match(
            (s) => {
                successFn(s);
                return 0;
            },
            (e) => {
                errorFn(e);
                return 0;
            }
        );
    }

    /// <summary>
    /// Pattern matches on a Task-wrapped Result with synchronous functions,
    /// returning a value.
    /// </summary>
    public static async Task<TResult>
    Match<TSuccess, TError, TResult>(
        this Task<Result<TSuccess, TError>> taskRes,
        Func<TSuccess, TResult> successFn,
        Func<TError, TResult> errorFn
    )
    {
        var res = await taskRes.ConfigureAwait(false);
        return res.Match(successFn, errorFn);
    }

    /// <summary>
    /// Pattern matches on a Task-wrapped Result with synchronous actions
    /// (side-effects only).
    /// </summary>
    public static async Task
    Match<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskRes,
        Action<TSuccess> successFn,
        Action<TError> errorFn
    )
    {
        var res = await taskRes.ConfigureAwait(false);
        res.Match(successFn, errorFn);
    }

    /// <summary>
    /// Pattern matches on a Task-wrapped Result with asynchronous functions,
    /// returning a value.
    /// </summary>
    public static async Task<TResult>
    MatchAsync<TSuccess, TError, TResult>(
        this Task<Result<TSuccess, TError>> taskRes,
        Func<TSuccess, Task<TResult>> successFn,
        Func<TError, Task<TResult>> errorFn
    )
    {
        var res = await taskRes.ConfigureAwait(false);
        TResult result = res switch {
            SuccessResult<TSuccess, TError>(var s) => await successFn(s).ConfigureAwait(false),
            ErrorResult<TSuccess, TError>(var e) => await errorFn(e).ConfigureAwait(false),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }

    /// <summary>
    /// Pattern matches on a Task-wrapped Result with asynchronous actions
    /// (side-effects only).
    /// </summary>
    public static async Task
    MatchAsync<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskRes,
        Func<TSuccess, Task> successFn,
        Func<TError, Task> errorFn
    )
    {
        var res = await taskRes.ConfigureAwait(false);
        switch (res)
        {
            case SuccessResult<TSuccess, TError>(var s):
                await successFn(s).ConfigureAwait(false);
                break;
            case ErrorResult<TSuccess, TError>(var e):
                await errorFn(e).ConfigureAwait(false);
                break;
            default:
                throw new ArgumentException("Result must be SuccessResult or ErrorResult.");
        }
    }


    /// <summary>
    /// Partitions a sequence of Results into two separate collections: one
    /// containing all success values and another containing all error values.
    /// This is useful for processing a batch of Results and separating the
    /// successful outcomes from the failures.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success values.</typeparam>
    /// <typeparam name="TError">The type of the error values.</typeparam>
    /// <param name="input">
    /// A sequence of Results to partition into successes and errors.
    /// </param>
    /// <returns>
    /// A tuple containing two collections: the first contains all success
    /// values from successful Results, and the second contains all error values
    /// from error Results. The order of values in each collection corresponds
    /// to the order they appeared in the input sequence.
    /// </returns>
    public static (IEnumerable<TSuccess>, IEnumerable<TError>)
    Partition<TSuccess, TError>(
        this IEnumerable<Result<TSuccess, TError>> input
    )
    {
        List<TSuccess> successes = [];
        List<TError> failures = [];

        foreach (Result<TSuccess, TError> res in input)
        {
            res.Match(
                successes.Add,
                failures.Add
            );
        }

        return (successes, failures);
    }

    /// <summary>
    /// Partitions a sequence of Task Results into two separate collections: one
    /// containing all success values and another containing all error values.
    /// </summary>
    public static async Task<(IEnumerable<TS>, IEnumerable<TE>)>
    PartitionAsync<TS, TE>(
        this IEnumerable<Task<Result<TS, TE>>> input
    )
    {
        List<TS> successes = [];
        List<TE> failures = [];

        foreach (Task<Result<TS, TE>> taskResult in input)
        {
            Result<TS, TE> res = await taskResult.ConfigureAwait(false);
            res.Match(
                successes.Add,
                failures.Add
            );
        }

        return (successes, failures);
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Result is an error, otherwise returns the success value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="res">The Result to assert.</param>
    /// <param name="errMsg">
    /// Optional custom error message to use if the assertion fails. If not
    /// provided, a default message including the error value will be used.
    /// </param>
    /// <returns>The success value if the Result is successful.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Result is an error instead of a success.
    /// </exception>
    public static TSuccess
    ThrowIfError<TSuccess, TError>(
        this Result<TSuccess, TError> res,
        string? errMsg = null
    )
    {
        return res.Match(
            (s) => s,
            (e) => throw new InvalidOperationException(errMsg ?? $"Expected successful Result but got error: {e}")
        );
    }

    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Result is an error, otherwise returns the success value.
    /// </summary>
    public static async Task<TSuccess>
    ThrowIfError<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        string? errMsg = null
    )
    {
        var res = await taskResult.ConfigureAwait(false);
        return res.ThrowIfError(errMsg);
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Result is a success, otherwise returns the error value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="res">The Result to assert.</param>
    /// <param name="errMsg">
    /// Optional custom error message to use if the assertion fails. If not
    /// provided, a default message including the success value will be used.
    /// </param>
    /// <returns>The error value if the Result is an error.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Result is a success instead of an error.
    /// </exception>
    public static TError
    ThrowIfSuccess<TSuccess, TError>(
        this Result<TSuccess, TError> res,
        string? errMsg = null
    )
    {
        return res.Match(
            (s) => throw new InvalidOperationException(errMsg ?? $"Expected error Result but got success: {s}"),
            (e) => e
        );
    }

    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Result is a success, otherwise returns the error value.
    /// </summary>
    public static async Task<TError>
    ThrowIfSuccess<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        string? errMsg = null
    )
    {
        var res = await taskResult.ConfigureAwait(false);
        return res.ThrowIfSuccess(errMsg);
    }
}
