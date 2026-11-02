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
    /// Combines a sequence of Results into a single Result.  If every input
    /// Result is successful, returns a successful Result containing all of the
    /// success values.  Otherwise, returns an error Result containing every
    /// error value.  The entire input sequence is always examined.
    ///
    /// Use this when every problem should be reported at once (for example,
    /// when validating all fields of a form).  To stop at the first error
    /// instead, use AllOrFirstError.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success values.</typeparam>
    /// <typeparam name="TError">The type of the error values.</typeparam>
    /// <param name="input">The sequence of Results to combine.</param>
    /// <returns>
    /// A successful Result containing all success values if every input is
    /// successful (including when the input is empty).  Otherwise, an error
    /// Result containing all error values; this collection is never empty.
    /// Values appear in the same order as in the input sequence.
    /// </returns>
    public static Result<IEnumerable<TSuccess>, IEnumerable<TError>>
    AllOrErrors<TSuccess, TError>(
        this IEnumerable<Result<TSuccess, TError>> input
    )
    {
        (IEnumerable<TSuccess> successes, IEnumerable<TError> errors) = input.Partition();

        return errors.Any() ? Error(errors) : Success(successes);
    }


    /// <summary>
    /// Combines a sequence of Results into a single Result.  If every input
    /// Result is successful, returns a successful Result containing all of the
    /// success values.  Otherwise, returns the first error.  Enumeration of the
    /// input stops at the first error, so later Results are never evaluated.
    ///
    /// This behaves like JavaScript's Promise.all().  To collect every error
    /// instead, use AllOrErrors.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success values.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The sequence of Results to combine.</param>
    /// <returns>
    /// A successful Result containing all success values, in input order, if
    /// every input is successful (including when the input is empty).
    /// Otherwise, an error Result containing the first error encountered.
    /// </returns>
    public static Result<IEnumerable<TSuccess>, TError>
    AllOrFirstError<TSuccess, TError>(
        this IEnumerable<Result<TSuccess, TError>> input
    )
    {
        List<TSuccess> successes = [];

        foreach (Result<TSuccess, TError> res in input)
        {
            switch (res)
            {
                case SuccessResult<TSuccess, TError>(var s):
                    successes.Add(s);
                    break;
                case ErrorResult<TSuccess, TError>(var e):
                    return Error(e);
                default:
                    throw new ArgumentException("Result must be SuccessResult or ErrorResult.");
            }
        }

        return Success<IEnumerable<TSuccess>>(successes);
    }


    /// <summary>
    /// Combines a tuple of 2 Results, which may have different success types,
    /// into a single Result containing a tuple of their success values.  If
    /// any Result is an error, returns the first error in tuple order.
    ///
    /// Use this to combine a fixed number of independent Results, for
    /// example <c>(r1, r2).AllOrFirstError()</c>.  To combine a sequence of
    /// Results with the same success type, use the IEnumerable overload.
    /// </summary>
    public static Result<(T1, T2), TError>
    AllOrFirstError<T1, T2, TError>(
        this (
            Result<T1, TError>,
            Result<T2, TError>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        select (v1, v2);


    /// <summary>
    /// Combines a tuple of 3 Results, which may have different success types,
    /// into a single Result containing a tuple of their success values.  If
    /// any Result is an error, returns the first error in tuple order.
    ///
    /// Use this to combine a fixed number of independent Results, for
    /// example <c>(r1, r2).AllOrFirstError()</c>.  To combine a sequence of
    /// Results with the same success type, use the IEnumerable overload.
    /// </summary>
    public static Result<(T1, T2, T3), TError>
    AllOrFirstError<T1, T2, T3, TError>(
        this (
            Result<T1, TError>,
            Result<T2, TError>,
            Result<T3, TError>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        select (v1, v2, v3);


    /// <summary>
    /// Combines a tuple of 4 Results, which may have different success types,
    /// into a single Result containing a tuple of their success values.  If
    /// any Result is an error, returns the first error in tuple order.
    ///
    /// Use this to combine a fixed number of independent Results, for
    /// example <c>(r1, r2).AllOrFirstError()</c>.  To combine a sequence of
    /// Results with the same success type, use the IEnumerable overload.
    /// </summary>
    public static Result<(T1, T2, T3, T4), TError>
    AllOrFirstError<T1, T2, T3, T4, TError>(
        this (
            Result<T1, TError>,
            Result<T2, TError>,
            Result<T3, TError>,
            Result<T4, TError>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        from v4 in input.Item4
        select (v1, v2, v3, v4);


    /// <summary>
    /// Combines a tuple of 5 Results, which may have different success types,
    /// into a single Result containing a tuple of their success values.  If
    /// any Result is an error, returns the first error in tuple order.
    ///
    /// Use this to combine a fixed number of independent Results, for
    /// example <c>(r1, r2).AllOrFirstError()</c>.  To combine a sequence of
    /// Results with the same success type, use the IEnumerable overload.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5), TError>
    AllOrFirstError<T1, T2, T3, T4, T5, TError>(
        this (
            Result<T1, TError>,
            Result<T2, TError>,
            Result<T3, TError>,
            Result<T4, TError>,
            Result<T5, TError>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        from v4 in input.Item4
        from v5 in input.Item5
        select (v1, v2, v3, v4, v5);


    /// <summary>
    /// Combines a tuple of 6 Results, which may have different success types,
    /// into a single Result containing a tuple of their success values.  If
    /// any Result is an error, returns the first error in tuple order.
    ///
    /// Use this to combine a fixed number of independent Results, for
    /// example <c>(r1, r2).AllOrFirstError()</c>.  To combine a sequence of
    /// Results with the same success type, use the IEnumerable overload.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5, T6), TError>
    AllOrFirstError<T1, T2, T3, T4, T5, T6, TError>(
        this (
            Result<T1, TError>,
            Result<T2, TError>,
            Result<T3, TError>,
            Result<T4, TError>,
            Result<T5, TError>,
            Result<T6, TError>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        from v4 in input.Item4
        from v5 in input.Item5
        from v6 in input.Item6
        select (v1, v2, v3, v4, v5, v6);


    /// <summary>
    /// Combines a tuple of 7 Results, which may have different success types,
    /// into a single Result containing a tuple of their success values.  If
    /// any Result is an error, returns the first error in tuple order.
    ///
    /// Use this to combine a fixed number of independent Results, for
    /// example <c>(r1, r2).AllOrFirstError()</c>.  To combine a sequence of
    /// Results with the same success type, use the IEnumerable overload.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5, T6, T7), TError>
    AllOrFirstError<T1, T2, T3, T4, T5, T6, T7, TError>(
        this (
            Result<T1, TError>,
            Result<T2, TError>,
            Result<T3, TError>,
            Result<T4, TError>,
            Result<T5, TError>,
            Result<T6, TError>,
            Result<T7, TError>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        from v4 in input.Item4
        from v5 in input.Item5
        from v6 in input.Item6
        from v7 in input.Item7
        select (v1, v2, v3, v4, v5, v6, v7);


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
        Result<TInSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.Bind(fn);
    }


    /// <summary>
    /// Monadic bind for a Result with an asynchronous function.  If the input
    /// Result is successful, awaits the function and returns its Result.  If
    /// the input Result is an error, propagates the error without invoking
    /// the function.
    /// </summary>
    public static Task<Result<TOutSuccess, TError>>
    BindAsync<TInSuccess, TOutSuccess, TError>(
        this Result<TInSuccess, TError> input,
        Func<TInSuccess, Task<Result<TOutSuccess, TError>>> fn
    ) =>
        Task.FromResult(input).BindAsync(fn);

    /// <summary>
    /// Monadic bind for a Task-wrapped Result with an asynchronous function.
    /// </summary>
    public static async Task<Result<TOutSuccess, TError>>
    BindAsync<TInSuccess, TOutSuccess, TError>(
        this Task<Result<TInSuccess, TError>> taskInput,
        Func<TInSuccess, Task<Result<TOutSuccess, TError>>> fn
    )
    {
        Result<TInSuccess, TError> input = await taskInput.ConfigureAwait(false);
        Result<TOutSuccess, TError> result = input switch {
            SuccessResult<TInSuccess, TError>(var s) => await fn(s).ConfigureAwait(false),
            ErrorResult<TInSuccess, TError>(var e) => Error(e),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


    /// <summary>
    /// If the input Result is an error, applies the provided function to the
    /// error value and returns its Result.  If the input Result is successful,
    /// returns the success without invoking the function.  This allows falling
    /// back to an alternative when a previous operation failed.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TInError">The type of the error value in the input Result.</typeparam>
    /// <typeparam name="TOutError">The type of the error value in the output Result.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="fn">
    /// A function that takes the error value and returns a new Result.  This
    /// function is only invoked if the input Result is an error.
    /// </param>
    /// <returns>
    /// The original success if the input is successful, or the Result returned
    /// by fn if the input is an error.
    /// </returns>
    public static Result<TSuccess, TOutError>
    BindError<TSuccess, TInError, TOutError>(
        this Result<TSuccess, TInError> input,
        Func<TInError, Result<TSuccess, TOutError>> fn
    ) =>
        input.Match<TSuccess, TInError, Result<TSuccess, TOutError>>(
            (s) => Success(s),
            (e) => fn(e)
        );


    /// <summary>
    /// BindError for a Task-wrapped Result with a synchronous function.
    /// </summary>
    public static async Task<Result<TSuccess, TOutError>>
    BindError<TSuccess, TInError, TOutError>(
        this Task<Result<TSuccess, TInError>> taskInput,
        Func<TInError, Result<TSuccess, TOutError>> fn
    )
    {
        Result<TSuccess, TInError> input = await taskInput.ConfigureAwait(false);
        return input.BindError(fn);
    }


    /// <summary>
    /// BindError for a Result with an asynchronous function.
    /// </summary>
    public static Task<Result<TSuccess, TOutError>>
    BindErrorAsync<TSuccess, TInError, TOutError>(
        this Result<TSuccess, TInError> input,
        Func<TInError, Task<Result<TSuccess, TOutError>>> fn
    ) =>
        input.Match<TSuccess, TInError, Task<Result<TSuccess, TOutError>>>(
            (s) => Task.FromResult<Result<TSuccess, TOutError>>(Success(s)),
            (e) => fn(e)
        );


    /// <summary>
    /// BindError for a Task-wrapped Result with an asynchronous function.
    /// </summary>
    public static async Task<Result<TSuccess, TOutError>>
    BindErrorAsync<TSuccess, TInError, TOutError>(
        this Task<Result<TSuccess, TInError>> taskInput,
        Func<TInError, Task<Result<TSuccess, TOutError>>> fn
    )
    {
        Result<TSuccess, TInError> input = await taskInput.ConfigureAwait(false);
        return await input.BindErrorAsync(fn).ConfigureAwait(false);
    }


    /// <summary>
    /// Checks if the result is successful and the success value equals the given value.
    /// Returns false for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The result to check.</param>
    /// <param name="value">The value to compare against the success value.</param>
    /// <returns>True if the result is successful and contains the given value; false otherwise.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; success = Success(42);
    /// bool isTrue = success.Contains(42);  // true
    ///
    /// Result&lt;int, string&gt; error = Error("failed");
    /// bool isFalse = error.Contains(42);  // false
    /// </code>
    /// </example>
    public static bool
    Contains<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        TSuccess value
    ) where TSuccess : IEquatable<TSuccess>
    {
        return input.Match(
            (s) => s.Equals(value),
            (e) => false
        );
    }


    /// <summary>
    /// Checks if a Task-wrapped result is successful and the success value equals the given value.
    /// Returns false for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="taskInput">The task wrapping a result to check.</param>
    /// <param name="value">The value to compare against the success value.</param>
    /// <returns>A task that completes with true if the result is successful and contains the given value; false otherwise.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; successResult = Success(42);
    /// Task&lt;Result&lt;int, string&gt;&gt; taskResult = Task.FromResult(successResult);
    /// bool isTrue = await taskResult.Contains(42);  // true
    /// </code>
    /// </example>
    public static async Task<bool>
    Contains<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        TSuccess value
    ) where TSuccess : IEquatable<TSuccess>
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.Contains(value);
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
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
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
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.DefaultWith(defaultValueFunc);
    }


    /// <summary>
    /// Returns the success value of a Result or computes a default value using
    /// the provided asynchronous function if the Result is an error.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="defaultValueFunc">
    /// An asynchronous function that takes the error value and returns a
    /// default success value. This function is only invoked if the input
    /// Result is an error.
    /// </param>
    /// <returns>
    /// A Task containing the success value if the Result is successful, or the
    /// computed default value if the Result is an error.
    /// </returns>
    public static Task<TSuccess>
    DefaultWithAsync<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Func<TError, Task<TSuccess>> defaultValueFunc
    ) =>
        input.Match(
            (s) => Task.FromResult(s),
            (e) => defaultValueFunc(e)
        );


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
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        TSuccess result = input switch {
            SuccessResult<TSuccess, TError>(var s) => s,
            ErrorResult<TSuccess, TError>(var e) => await defaultValueFunc(e).ConfigureAwait(false),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


    /// <summary>
    /// Extracts the error values from all error Results in a sequence,
    /// discarding the successful Results.  The sequence is evaluated lazily.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success values.</typeparam>
    /// <typeparam name="TError">The type of the error values.</typeparam>
    /// <param name="input">The sequence of Results.</param>
    /// <returns>The error values, in input order.</returns>
    public static IEnumerable<TError>
    Errors<TSuccess, TError>(
        this IEnumerable<Result<TSuccess, TError>> input
    )
    {
        foreach (Result<TSuccess, TError> res in input)
        {
            if (res is ErrorResult<TSuccess, TError>(var e))
            {
                yield return e;
            }
        }
    }


    /// <summary>
    /// Invokes each function in sequence until one returns an error.  If every
    /// function succeeds, returns a successful Result containing all of the
    /// success values.  Otherwise, returns the first error; the remaining
    /// functions are not invoked.
    ///
    /// To run a fixed number of operations with different success types, use
    /// query syntax or the tuple overloads of AllOrFirstError instead.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success values.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="fns">The functions to invoke, in order.</param>
    /// <returns>
    /// A successful Result containing all success values, in order, if every
    /// function succeeds.  Otherwise, the first error.
    /// </returns>
    public static Result<IEnumerable<TSuccess>, TError>
    ExecuteWhileSuccessful<TSuccess, TError>(
        this IEnumerable<Func<Result<TSuccess, TError>>> fns
    ) =>
        fns.Select((fn) => fn()).AllOrFirstError();


    /// <summary>
    /// Checks if the result is successful and the success value satisfies the predicate.
    /// Returns false for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The result to check.</param>
    /// <param name="predicate">The predicate to apply to the success value.</param>
    /// <returns>True if the result is successful and the predicate returns true; false otherwise.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; success = Success(42);
    /// bool isTrue = success.Exists(x =&gt; x &gt; 40);  // true
    ///
    /// Result&lt;int, string&gt; error = Error("failed");
    /// bool isFalse = error.Exists(x =&gt; x &gt; 40);  // false
    /// </code>
    /// </example>
    public static bool
    Exists<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Func<TSuccess, bool> predicate
    )
    {
        return input.Match(
            (s) => predicate(s),
            (e) => false
        );
    }


    /// <summary>
    /// Checks if a Task-wrapped result is successful and the success value satisfies the predicate.
    /// Returns false for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="taskInput">The task wrapping a result to check.</param>
    /// <param name="predicate">The predicate to apply to the success value.</param>
    /// <returns>A task that completes with true if the result is successful and the predicate returns true; false otherwise.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; successResult = Success(42);
    /// Task&lt;Result&lt;int, string&gt;&gt; taskResult = Task.FromResult(successResult);
    /// bool isTrue = await taskResult.Exists(x =&gt; x &gt; 40);  // true
    /// </code>
    /// </example>
    public static async Task<bool>
    Exists<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TSuccess, bool> predicate
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.Exists(predicate);
    }


    /// <summary>
    /// Checks if a result is successful and applies an asynchronous predicate to the success value.
    /// Returns false for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The result to check.</param>
    /// <param name="predicate">The asynchronous predicate to apply to the success value.</param>
    /// <returns>A task that completes with true if the result is successful and the predicate returns true; false otherwise.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; success = Success(42);
    /// bool isTrue = await success.ExistsAsync(async x =&gt;
    /// {
    ///     await Task.Delay(1);
    ///     return x &gt; 40;
    /// });  // true
    /// </code>
    /// </example>
    public static async Task<bool>
    ExistsAsync<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Func<TSuccess, Task<bool>> predicate
    )
    {
        return await input.Match(
            async (s) => await predicate(s).ConfigureAwait(false),
            (e) => Task.FromResult(false)
        ).ConfigureAwait(false);
    }


    /// <summary>
    /// Checks if a Task-wrapped result is successful and applies an asynchronous predicate to the success value.
    /// Returns false for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="taskInput">The task wrapping a result to check.</param>
    /// <param name="predicate">The asynchronous predicate to apply to the success value.</param>
    /// <returns>A task that completes with true if the result is successful and the predicate returns true; false otherwise.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; successResult = Success(42);
    /// Task&lt;Result&lt;int, string&gt;&gt; taskResult = Task.FromResult(successResult);
    /// bool isTrue = await taskResult.ExistsAsync(async x =&gt;
    /// {
    ///     await Task.Delay(1);
    ///     return x &gt; 40;
    /// });  // true
    /// </code>
    /// </example>
    public static async Task<bool>
    ExistsAsync<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TSuccess, Task<bool>> predicate
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return await input.ExistsAsync(predicate).ConfigureAwait(false);
    }


    /// <summary>
    /// Folds the success value into an accumulator state. Returns the initial state unchanged for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TState">The type of the accumulator state.</typeparam>
    /// <param name="input">The result to fold.</param>
    /// <param name="foldFn">A function that combines the accumulator state with the success value.</param>
    /// <param name="initialState">The initial state value.</param>
    /// <returns>The folded state if successful; the initial state if an error.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; success = Success(5);
    /// int result = success.Fold((state, value) =&gt; state + value, 10);  // 15
    ///
    /// Result&lt;int, string&gt; error = Error("failed");
    /// int resultError = error.Fold((state, value) =&gt; state + value, 10);  // 10
    /// </code>
    /// </example>
    public static TState
    Fold<TSuccess, TError, TState>(
        this Result<TSuccess, TError> input,
        Func<TState, TSuccess, TState> foldFn,
        TState initialState
    )
    {
        return input.Match(
            (s) => foldFn(initialState, s),
            (e) => initialState
        );
    }


    /// <summary>
    /// Folds a Task-wrapped result's success value into an accumulator state.
    /// Returns the initial state unchanged for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TState">The type of the accumulator state.</typeparam>
    /// <param name="taskInput">The task wrapping a result to fold.</param>
    /// <param name="foldFn">A function that combines the accumulator state with the success value.</param>
    /// <param name="initialState">The initial state value.</param>
    /// <returns>A task that completes with the folded state if successful; the initial state if an error.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; successResult = Success(5);
    /// Task&lt;Result&lt;int, string&gt;&gt; taskResult = Task.FromResult(successResult);
    /// int foldedState = await taskResult.Fold((state, value) =&gt; state + value, 10);  // 15
    /// </code>
    /// </example>
    public static async Task<TState>
    Fold<TSuccess, TError, TState>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TState, TSuccess, TState> foldFn,
        TState initialState
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.Fold(foldFn, initialState);
    }


    /// <summary>
    /// Folds the success value into an accumulator state using an asynchronous folder function.
    /// Returns the initial state unchanged for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TState">The type of the accumulator state.</typeparam>
    /// <param name="input">The result to fold.</param>
    /// <param name="foldFn">An asynchronous function that combines the accumulator state with the success value.</param>
    /// <param name="initialState">The initial state value.</param>
    /// <returns>A task that completes with the folded state if successful; the initial state if an error.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; success = Success(5);
    /// int result = await success.FoldAsync(async (state, value) =&gt;
    /// {
    ///     await Task.Delay(1);
    ///     return state + value;
    /// }, 10);  // 15
    /// </code>
    /// </example>
    public static async Task<TState>
    FoldAsync<TSuccess, TError, TState>(
        this Result<TSuccess, TError> input,
        Func<TState, TSuccess, Task<TState>> foldFn,
        TState initialState
    )
    {
        return await input.Match(
            async (s) => await foldFn(initialState, s).ConfigureAwait(false),
            (e) => Task.FromResult(initialState)
        ).ConfigureAwait(false);
    }


    /// <summary>
    /// Folds a Task-wrapped result's success value into an accumulator state using an asynchronous folder function.
    /// Returns the initial state unchanged for an error result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TState">The type of the accumulator state.</typeparam>
    /// <param name="taskInput">The task wrapping a result to fold.</param>
    /// <param name="foldFn">An asynchronous function that combines the accumulator state with the success value.</param>
    /// <param name="initialState">The initial state value.</param>
    /// <returns>A task that completes with the folded state if successful; the initial state if an error.</returns>
    /// <example>
    /// <code>
    /// Result&lt;int, string&gt; successResult = Success(5);
    /// Task&lt;Result&lt;int, string&gt;&gt; taskResult = Task.FromResult(successResult);
    /// int result = await taskResult.FoldAsync(async (state, value) =&gt;
    /// {
    ///     await Task.Delay(1);
    ///     return state + value;
    /// }, 10);  // 15
    /// </code>
    /// </example>
    public static async Task<TState>
    FoldAsync<TSuccess, TError, TState>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TState, TSuccess, Task<TState>> foldFn,
        TState initialState
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return await input.FoldAsync(foldFn, initialState).ConfigureAwait(false);
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
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.Gate(gateFn);
    }


    /// <summary>
    /// Gates a Result by applying an asynchronous validation function.
    /// </summary>
    public static Task<Result<TSuccess, TError>>
    GateAsync<TSuccess, TError, TFnSuccess>(
        this Result<TSuccess, TError> input,
        Func<TSuccess, Task<Result<TFnSuccess, TError>>> gateFn
    ) =>
        Task.FromResult(input).GateAsync(gateFn);

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
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        switch (input)
        {
            case SuccessResult<TSuccess, TError>(var s):
            {
                Result<TFnSuccess, TError> gateRes = await gateFn(s).ConfigureAwait(false);
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
    GetError<TSuccess, TError>(
        this Result<TSuccess, TError> input
    )
    {
        return input.ThrowIfSuccess();
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
    GetError<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
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
    GetValue<TSuccess, TError>(
        this Result<TSuccess, TError> input
    )
    {
        return input.ThrowIfError();
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
    GetValue<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.GetValue();
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
        Result<TSuccess, TInError> input = await taskInput.ConfigureAwait(false);
        return input.MapError(fn);
    }


    /// <summary>
    /// Maps the error value of a Result using an asynchronous function.
    /// </summary>
    public static Task<Result<TSuccess, TOutError>>
    MapErrorAsync<TSuccess, TInError, TOutError>(
        this Result<TSuccess, TInError> input,
        Func<TInError, Task<TOutError>> fn
    ) =>
        Task.FromResult(input).MapErrorAsync(fn);


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
        Result<TSuccess, TInError> input = await taskInput.ConfigureAwait(false);
        Result<TSuccess, TOutError> result = input switch {
            SuccessResult<TSuccess, TInError>(var s) => Success(s),
            ErrorResult<TSuccess, TInError>(var e) => Error(await fn(e).ConfigureAwait(false)),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


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
        Result<TInSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.MapSuccess(fn);
    }


    /// <summary>
    /// Maps the success value of a Result using an asynchronous function.
    /// </summary>
    public static Task<Result<TOutSuccess, TError>>
    MapSuccessAsync<TInSuccess, TOutSuccess, TError>(
        this Result<TInSuccess, TError> input,
        Func<TInSuccess, Task<TOutSuccess>> fn
    ) =>
        Task.FromResult(input).MapSuccessAsync(fn);


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
        Result<TInSuccess, TError> input = await taskInput.ConfigureAwait(false);
        Result<TOutSuccess, TError> result = input switch {
            SuccessResult<TInSuccess, TError>(var s) => Success(await fn(s).ConfigureAwait(false)),
            ErrorResult<TInSuccess, TError>(var e) => Error(e),
            _ => throw new ArgumentException("Result must be SuccessResult or ErrorResult.")
        };
        return result;
    }


    /// <summary>
    /// Maps each item in a sequence using a function that may fail, stopping
    /// at the first failure.  The remaining items are not mapped.
    /// </summary>
    /// <typeparam name="TIn">The type of the input items.</typeparam>
    /// <typeparam name="TOut">The type of the mapped values.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The items to map.</param>
    /// <param name="fn">The mapping function.</param>
    /// <returns>
    /// A successful Result containing all mapped values, in input order, if
    /// every mapping succeeds.  Otherwise, the first error.
    /// </returns>
    public static Result<IEnumerable<TOut>, TError>
    MapWhileSuccessful<TIn, TOut, TError>(
        this IEnumerable<TIn> input,
        Func<TIn, Result<TOut, TError>> fn
    ) =>
        input.Select(fn).AllOrFirstError();


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
        Result<TSuccess, TError> res = await taskRes.ConfigureAwait(false);
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
        Result<TSuccess, TError> res = await taskRes.ConfigureAwait(false);
        res.Match(successFn, errorFn);
    }


    /// <summary>
    /// Pattern matches on a Result with asynchronous functions, returning a
    /// value.
    /// </summary>
    public static Task<TResult>
    MatchAsync<TSuccess, TError, TResult>(
        this Result<TSuccess, TError> res,
        Func<TSuccess, Task<TResult>> successFn,
        Func<TError, Task<TResult>> errorFn
    ) =>
        Task.FromResult(res).MatchAsync(successFn, errorFn);


    /// <summary>
    /// Pattern matches on a Result with asynchronous actions (side-effects
    /// only).
    /// </summary>
    public static Task
    MatchAsync<TSuccess, TError>(
        this Result<TSuccess, TError> res,
        Func<TSuccess, Task> successFn,
        Func<TError, Task> errorFn
    ) =>
        Task.FromResult(res).MatchAsync(successFn, errorFn);


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
        Result<TSuccess, TError> res = await taskRes.ConfigureAwait(false);
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
        Result<TSuccess, TError> res = await taskRes.ConfigureAwait(false);
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
    /// Partitions a sequence of Task-wrapped Results into two separate
    /// collections: one containing all success values and another containing
    /// all error values.  The tasks are awaited in order.
    /// </summary>
    public static async Task<(IEnumerable<TSuccess>, IEnumerable<TError>)>
    Partition<TSuccess, TError>(
        this IEnumerable<Task<Result<TSuccess, TError>>> input
    )
    {
        List<TSuccess> successes = [];
        List<TError> failures = [];

        foreach (Task<Result<TSuccess, TError>> taskResult in input)
        {
            Result<TSuccess, TError> res = await taskResult.ConfigureAwait(false);
            res.Match(
                successes.Add,
                failures.Add
            );
        }

        return (successes, failures);
    }


    /// <summary>
    /// Requires that a list contain at least one element.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="input">The list to check.</param>
    /// <param name="errorFactory">
    /// Factory for the error value.  This factory is invoked only when the
    /// list is empty.
    /// </param>
    /// <returns>
    /// A successful Result containing the list if it is not empty; otherwise
    /// Error(errorFactory()).
    /// </returns>
    public static Result<IReadOnlyList<T>, TError>
    RequireNonEmpty<T, TError>(
        this IReadOnlyList<T> input,
        Func<TError> errorFactory
    ) =>
        input.Count > 0 ? Success(input) : Error(errorFactory());


    /// <summary>
    /// Requires that a sequence contain exactly one element.  At most two
    /// elements are enumerated.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="input">The sequence to check.</param>
    /// <param name="errorFactory">
    /// Factory for the error value.  This factory is invoked only when the
    /// sequence does not contain exactly one element.
    /// </param>
    /// <returns>
    /// A successful Result containing the single element if there is exactly
    /// one; otherwise Error(errorFactory()).
    /// </returns>
    public static Result<T, TError>
    RequireSingle<T, TError>(
        this IEnumerable<T> input,
        Func<TError> errorFactory
    )
    {
        using IEnumerator<T> enumerator = input.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return Error(errorFactory());
        }

        T first = enumerator.Current;
        return enumerator.MoveNext() ? Error(errorFactory()) : Success(first);
    }


    /// <summary>
    /// Maps the success value of a Result.  Equivalent to MapSuccess.
    ///
    /// This method exists so that Results can be used with C# query syntax.  A
    /// query with a single from clause, such as
    /// <c>from x in result select x * 2</c>, is translated by the compiler
    /// into a call to this method.
    /// </summary>
    /// <typeparam name="TInSuccess">
    /// The type of the success value in the input Result.
    /// </typeparam>
    /// <typeparam name="TOutSuccess">
    /// The type of the success value in the output Result.
    /// </typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="selector">
    /// The function to apply to the success value.  Only invoked if the input
    /// Result is successful.
    /// </param>
    /// <returns>
    /// A successful Result containing the selector's return value if the input
    /// is successful, or the original error if the input is an error.
    /// </returns>
    public static Result<TOutSuccess, TError>
    Select<TInSuccess, TOutSuccess, TError>(
        this Result<TInSuccess, TError> input,
        Func<TInSuccess, TOutSuccess> selector
    ) =>
        input.MapSuccess(selector);


    /// <summary>
    /// Binds a Result to a function returning another Result, then combines
    /// both success values using a projection.  Equivalent to Bind followed
    /// by MapSuccess.
    ///
    /// This method exists so that Results can be used with C# query syntax,
    /// which flattens chains of operations that may fail:
    /// <code>
    /// from first in ValidateName(firstName)
    /// from last in ValidateName(lastName)
    /// select new Person { FirstName = first, LastName = last }
    /// </code>
    /// Each from clause unwraps a success value.  If any Result is an error,
    /// evaluation stops and the whole expression evaluates to that error.
    /// </summary>
    /// <typeparam name="TInSuccess">
    /// The type of the success value in the input Result.
    /// </typeparam>
    /// <typeparam name="TNextSuccess">
    /// The type of the success value in the Result returned by
    /// <paramref name="next"/>.
    /// </typeparam>
    /// <typeparam name="TOutSuccess">
    /// The type of the success value in the output Result.
    /// </typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="next">
    /// A function that takes the input's success value and returns the next
    /// Result.  Only invoked if the input Result is successful.
    /// </param>
    /// <param name="project">
    /// A function that combines both success values into the output value.
    /// Only invoked if both Results are successful.
    /// </param>
    /// <returns>
    /// A successful Result containing the projected value if both Results are
    /// successful.  Otherwise, the first error encountered.
    /// </returns>
    public static Result<TOutSuccess, TError>
    SelectMany<TInSuccess, TNextSuccess, TOutSuccess, TError>(
        this Result<TInSuccess, TError> input,
        Func<TInSuccess, Result<TNextSuccess, TError>> next,
        Func<TInSuccess, TNextSuccess, TOutSuccess> project
    ) =>
        input.Bind((a) => next(a).MapSuccess((b) => project(a, b)));


    /// <summary>
    /// Extracts the success values from all successful Results in a sequence,
    /// discarding the error Results.  The sequence is evaluated lazily.
    ///
    /// This is the Result counterpart of Option's Somes.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success values.</typeparam>
    /// <typeparam name="TError">The type of the error values.</typeparam>
    /// <param name="input">The sequence of Results.</param>
    /// <returns>The success values, in input order.</returns>
    public static IEnumerable<TSuccess>
    Successes<TSuccess, TError>(
        this IEnumerable<Result<TSuccess, TError>> input
    )
    {
        foreach (Result<TSuccess, TError> res in input)
        {
            if (res is SuccessResult<TSuccess, TError>(var s))
            {
                yield return s;
            }
        }
    }


    /// <summary>
    /// Performs a side-effect (for example, logging) using the Result and
    /// returns the Result unchanged.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="fn">The action to invoke with the Result.</param>
    /// <returns>The input Result.</returns>
    public static Result<TSuccess, TError>
    Tap<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Action<Result<TSuccess, TError>> fn
    )
    {
        fn(input);
        return input;
    }


    /// <summary>
    /// Tap for a Task-wrapped Result with a synchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    Tap<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Action<Result<TSuccess, TError>> fn
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.Tap(fn);
    }


    /// <summary>
    /// Tap for a Result with an asynchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapAsync<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Func<Result<TSuccess, TError>, Task> fn
    )
    {
        await fn(input).ConfigureAwait(false);
        return input;
    }


    /// <summary>
    /// Tap for a Task-wrapped Result with an asynchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapAsync<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<Result<TSuccess, TError>, Task> fn
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return await input.TapAsync(fn).ConfigureAwait(false);
    }


    /// <summary>
    /// If the Result is an error, performs a side-effect (for example,
    /// logging) using the error value.  Returns the Result unchanged.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="fn">
    /// The action to invoke with the error value.  Only invoked if the input
    /// Result is an error.
    /// </param>
    /// <returns>The input Result.</returns>
    public static Result<TSuccess, TError>
    TapError<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Action<TError> fn
    )
    {
        if (input is ErrorResult<TSuccess, TError>(var e))
        {
            fn(e);
        }

        return input;
    }


    /// <summary>
    /// TapError for a Task-wrapped Result with a synchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapError<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Action<TError> fn
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.TapError(fn);
    }


    /// <summary>
    /// TapError for a Result with an asynchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapErrorAsync<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Func<TError, Task> fn
    )
    {
        if (input is ErrorResult<TSuccess, TError>(var e))
        {
            await fn(e).ConfigureAwait(false);
        }

        return input;
    }


    /// <summary>
    /// TapError for a Task-wrapped Result with an asynchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapErrorAsync<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TError, Task> fn
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return await input.TapErrorAsync(fn).ConfigureAwait(false);
    }


    /// <summary>
    /// If the Result is successful, performs a side-effect (for example,
    /// logging) using the success value.  Returns the Result unchanged.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="input">The input Result.</param>
    /// <param name="fn">
    /// The action to invoke with the success value.  Only invoked if the input
    /// Result is successful.
    /// </param>
    /// <returns>The input Result.</returns>
    public static Result<TSuccess, TError>
    TapSuccess<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Action<TSuccess> fn
    )
    {
        if (input is SuccessResult<TSuccess, TError>(var s))
        {
            fn(s);
        }

        return input;
    }


    /// <summary>
    /// TapSuccess for a Task-wrapped Result with a synchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapSuccess<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Action<TSuccess> fn
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.TapSuccess(fn);
    }


    /// <summary>
    /// TapSuccess for a Result with an asynchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapSuccessAsync<TSuccess, TError>(
        this Result<TSuccess, TError> input,
        Func<TSuccess, Task> fn
    )
    {
        if (input is SuccessResult<TSuccess, TError>(var s))
        {
            await fn(s).ConfigureAwait(false);
        }

        return input;
    }


    /// <summary>
    /// TapSuccess for a Task-wrapped Result with an asynchronous action.
    /// </summary>
    public static async Task<Result<TSuccess, TError>>
    TapSuccessAsync<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput,
        Func<TSuccess, Task> fn
    )
    {
        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return await input.TapSuccessAsync(fn).ConfigureAwait(false);
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
    /// Throws an InvalidOperationException if the Result is an error, otherwise
    /// returns the success value.  The exception message is produced from the
    /// error value by the specified function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="res">The Result to assert.</param>
    /// <param name="errMsgFn">
    /// A function that produces the exception message from the error value.
    /// Only invoked if the Result is an error.
    /// </param>
    /// <returns>The success value if the Result is successful.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Result is an error instead of a success.
    /// </exception>
    public static TSuccess
    ThrowIfError<TSuccess, TError>(
        this Result<TSuccess, TError> res,
        Func<TError, string> errMsgFn
    ) =>
        res.Match(
            (s) => s,
            (e) => throw new InvalidOperationException(errMsgFn(e))
        );


    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Result is an error, otherwise returns the success value.
    /// </summary>
    public static async Task<TSuccess>
    ThrowIfError<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        string? errMsg = null
    )
    {
        Result<TSuccess, TError> res = await taskResult.ConfigureAwait(false);
        return res.ThrowIfError(errMsg);
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Result is an
    /// error, otherwise returns the success value.  The exception message is
    /// produced from the error value by the specified function.
    /// </summary>
    public static async Task<TSuccess>
    ThrowIfError<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TError, string> errMsgFn
    )
    {
        Result<TSuccess, TError> res = await taskResult.ConfigureAwait(false);
        return res.ThrowIfError(errMsgFn);
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
    /// Throws an InvalidOperationException if the Result is a success,
    /// otherwise returns the error value.  The exception message is produced
    /// from the success value by the specified function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="res">The Result to assert.</param>
    /// <param name="errMsgFn">
    /// A function that produces the exception message from the success value.
    /// Only invoked if the Result is a success.
    /// </param>
    /// <returns>The error value if the Result is an error.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Result is a success instead of an error.
    /// </exception>
    public static TError
    ThrowIfSuccess<TSuccess, TError>(
        this Result<TSuccess, TError> res,
        Func<TSuccess, string> errMsgFn
    ) =>
        res.Match(
            (s) => throw new InvalidOperationException(errMsgFn(s)),
            (e) => e
        );


    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Result is a success, otherwise returns the error value.
    /// </summary>
    public static async Task<TError>
    ThrowIfSuccess<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        string? errMsg = null
    )
    {
        Result<TSuccess, TError> res = await taskResult.ConfigureAwait(false);
        return res.ThrowIfSuccess(errMsg);
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Result is a
    /// success, otherwise returns the error value.  The exception message is
    /// produced from the success value by the specified function.
    /// </summary>
    public static async Task<TError>
    ThrowIfSuccess<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, string> errMsgFn
    )
    {
        Result<TSuccess, TError> res = await taskResult.ConfigureAwait(false);
        return res.ThrowIfSuccess(errMsgFn);
    }


    /// <summary>
    /// Converts a Result to an Option explicitly.
    /// Returns Some(value) when the Result is success; otherwise returns none.
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="input">The Result input value.</param>
    public static Option<TSuccess>
    ToOption<TSuccess, TError>(
        this Result<TSuccess, TError> input
    )
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
    ToOption<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskInput
    )
    {
        ArgumentNullException.ThrowIfNull(taskInput);

        Result<TSuccess, TError> input = await taskInput.ConfigureAwait(false);
        return input.ToOption();
    }


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

        return value is null ? Error(errorFactory()) : Success(value);
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
    /// Converts a nullable value type to a Result explicitly.
    /// Returns Success(value) when value has a value; otherwise returns
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
    ) where TSuccess : struct
    {
        ArgumentNullException.ThrowIfNull(errorFactory);

        return value.HasValue ? Success(value.Value) : Error(errorFactory());
    }


    /// <summary>
    /// Converts a bool to a Result.  Returns Success(successValue) when the
    /// condition is true; otherwise returns Error(errorValue).
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="condition">The condition.</param>
    /// <param name="successValue">The value to wrap when the condition is true.</param>
    /// <param name="errorValue">The error to wrap when the condition is false.</param>
    public static Result<TSuccess, TError>
    ToResult<TSuccess, TError>(
        this bool condition,
        TSuccess successValue,
        TError errorValue
    ) =>
        condition ? Success(successValue) : Error(errorValue);

    /// <summary>
    /// Converts a Task-wrapped Option to a Result explicitly.
    /// Returns Success(value) when the Option is Some; otherwise returns
    /// Error(errorFactory()).
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <param name="taskInput">A task that resolves to an Option.</param>
    /// <param name="errorFactory">
    /// Factory for the error value. This factory is invoked only when the Option is None.
    /// </param>
    public static async Task<Result<TSuccess, TError>>
    ToResult<TSuccess, TError>(
        this Task<Option<TSuccess>> taskInput,
        Func<TError> errorFactory
    )
    {
        ArgumentNullException.ThrowIfNull(taskInput);
        ArgumentNullException.ThrowIfNull(errorFactory);

        Option<TSuccess> input = await taskInput.ConfigureAwait(false);
        return input.ToResult(errorFactory);
    }
}
