using System;
using static FnUtil.F;

namespace FnUtil;


// Option extension methods.
public static class OptionExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Combines a sequence of Options into a single Option.  If every input
    /// Option is Some, returns Some containing all of the values.  Otherwise,
    /// returns None.  Enumeration of the input stops at the first None, so
    /// later Options are never evaluated.
    ///
    /// This is the Option counterpart of Result's AllOrFirstError.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Options.</typeparam>
    /// <param name="input">The sequence of Options to combine.</param>
    /// <returns>
    /// Some containing all values, in input order, if every input is Some
    /// (including when the input is empty).  Otherwise, None.
    /// </returns>
    public static Option<IEnumerable<T>>
    AllOrNone<T>(this IEnumerable<Option<T>> input)
    {
        List<T> values = [];

        foreach (Option<T> opt in input)
        {
            switch (opt)
            {
                case Some<T> some:
                    values.Add(some.Value);
                    break;
                case None<T>:
                    return none;
                default:
                    throw new ArgumentException("Option must be None or Some.");
            }
        }

        return Some<IEnumerable<T>>(values);
    }


    /// <summary>
    /// Monadic bind operation for Option.  If the input Option is Some, applies
    /// the provided function to the value and returns its Option.  If the input
    /// Option is None, returns None without invoking the function.  This allows
    /// chaining operations that may not produce a value.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the input Option.</typeparam>
    /// <typeparam name="TOut">The type of value contained in the output Option.</typeparam>
    /// <param name="input">The input Option to bind.</param>
    /// <param name="fn">
    /// A function that takes the value and returns a new Option.  This function
    /// is only invoked if the input Option is Some.
    /// </param>
    /// <returns>
    /// The Option returned by fn if the input is Some; otherwise None.
    /// </returns>
    public static Option<TOut>
    Bind<T, TOut>(
        this Option<T> input,
        Func<T, Option<TOut>> fn
    ) =>
        input.Match(
            (val) => fn(val),
            () => none
        );

    /// <summary>
    /// Monadic bind for a Task-wrapped Option with a synchronous function.
    /// </summary>
    public static async Task<Option<TOut>>
    Bind<T, TOut>(
        this Task<Option<T>> taskInput,
        Func<T, Option<TOut>> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.Bind(fn);
    }


    /// <summary>
    /// Monadic bind for a Task-wrapped Option with an asynchronous function.
    /// </summary>
    public static async Task<Option<TOut>>
    BindAsync<T, TOut>(
        this Task<Option<T>> taskInput,
        Func<T, Task<Option<TOut>>> fn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        Option<TOut> result = input switch {
            Some<T> some => await fn(some.Value).ConfigureAwait(false),
            None<T> => none,
            _ => throw new ArgumentException("Option must be None or Some.")
        };
        return result;
    }


    /// <summary>
    /// Returns the contained value if Some, or the provided default value if None.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to extract a value from.</param>
    /// <param name="defaultValue">The value to return if the Option is None.</param>
    /// <returns>The contained value or the default value.</returns>
    public static T
    DefaultValue<T>(this Option<T> opt, T defaultValue) =>
        opt.Match(
            (t) => t,
            () => defaultValue
        );

    /// <summary>
    /// Returns the contained value of a Task-wrapped Option if Some, or the
    /// provided default value if None.
    /// </summary>
    public static async Task<T>
    DefaultValue<T>(this Task<Option<T>> taskInput, T defaultValue)
    {
        var opt = await taskInput.ConfigureAwait(false);
        return opt.DefaultValue(defaultValue);
    }


    /// <summary>
    /// Returns the contained value if Some, or computes a default value if None.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to extract a value from.</param>
    /// <param name="defaultValueFunc">
    /// A function that produces the default value.  This function is only
    /// invoked if the Option is None.
    /// </param>
    /// <returns>The contained value or the computed default value.</returns>
    public static T
    DefaultWith<T>(this Option<T> opt, Func<T> defaultValueFunc) =>
        opt.Match(
            (t) => t,
            () => defaultValueFunc()
        );

    /// <summary>
    /// Returns the contained value of a Task-wrapped Option if Some, or
    /// computes a default value using the provided synchronous function if
    /// None.
    /// </summary>
    public static async Task<T>
    DefaultWith<T>(this Task<Option<T>> taskInput, Func<T> defaultValueFunc)
    {
        var opt = await taskInput.ConfigureAwait(false);
        return opt.DefaultWith(defaultValueFunc);
    }


    /// <summary>
    /// Returns the contained value if Some, or asynchronously computes a
    /// default value if None.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to extract a value from.</param>
    /// <param name="defaultValueFunc">
    /// An async function that produces the default value.  This function is
    /// only invoked if the Option is None.
    /// </param>
    /// <returns>A Task containing the contained value or the computed default value.</returns>
    public static Task<T>
    DefaultWithAsync<T>(this Option<T> opt, Func<Task<T>> defaultValueFunc) =>
        opt.Match(
            (t) => Task.FromResult(t),
            () => defaultValueFunc()
        );

    /// <summary>
    /// Returns the contained value of a Task-wrapped Option if Some, or
    /// computes a default value using the provided asynchronous function if
    /// None.
    /// </summary>
    public static async Task<T>
    DefaultWithAsync<T>(this Task<Option<T>> taskInput, Func<Task<T>> defaultValueFunc)
    {
        var opt = await taskInput.ConfigureAwait(false);
        T result = opt switch {
            Some<T> some => some.Value,
            None<T> => await defaultValueFunc().ConfigureAwait(false),
            _ => throw new ArgumentException("Option must be None or Some.")
        };
        return result;
    }


    /// <summary>
    /// Gates an Option by applying a check function.  If the input Option is
    /// Some, applies the gate function to the value.  If the gate function
    /// returns Some, returns the original input Option.  If the gate function
    /// returns None, returns None.  If the input Option is already None,
    /// returns None without invoking the gate function.  This is useful for
    /// adding checks that don't transform the value but may discard it.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <typeparam name="TFn">
    /// The type of value returned by the gate function (ignored, as only
    /// whether it is Some or None matters).
    /// </typeparam>
    /// <param name="input">The input Option to gate.</param>
    /// <param name="gateFn">
    /// A check function that takes the value and returns an Option.  This
    /// function is only invoked if the input Option is Some.  The value of the
    /// returned Option is ignored; only whether it is Some or None matters.
    /// </param>
    /// <returns>
    /// The original input Option if both the input and the gate function's
    /// result are Some; otherwise None.
    /// </returns>
    public static Option<T>
    Gate<T, TFn>(
        this Option<T> input,
        Func<T, Option<TFn>> gateFn
    ) =>
        input.Bind((val) => gateFn(val).Map((_) => val));

    /// <summary>
    /// Gates a Task-wrapped Option by applying a synchronous check function.
    /// </summary>
    public static async Task<Option<T>>
    Gate<T, TFn>(
        this Task<Option<T>> taskInput,
        Func<T, Option<TFn>> gateFn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.Gate(gateFn);
    }


    /// <summary>
    /// Gates a Task-wrapped Option by applying an asynchronous check function.
    /// </summary>
    public static async Task<Option<T>>
    GateAsync<T, TFn>(
        this Task<Option<T>> taskInput,
        Func<T, Task<Option<TFn>>> gateFn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        switch (input)
        {
            case Some<T> some:
            {
                var gateRes = await gateFn(some.Value).ConfigureAwait(false);
                return gateRes.Map((_) => some.Value);
            }
            case None<T>:
                return none;
            default:
                throw new ArgumentException("Option must be None or Some.");
        }
    }


    /// <summary>
    /// Returns the contained value from an Option.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="input">The Option to extract a value from.</param>
    /// <returns>The contained value if the Option is Some.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the Option is None.
    /// </exception>
    public static T
    GetValue<T>(this Option<T> input)
    {
        return input.Match(
            (t) => t,
            () => throw new InvalidOperationException("Cannot get value from a None option.")
        );
    }

    /// <summary>
    /// Returns the contained value from a Task-wrapped Option.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="input">A task that resolves to an Option.</param>
    /// <returns>A Task containing the value if the Option is Some.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the Option is None.
    /// </exception>
    public static async Task<T>
    GetValue<T>(this Task<Option<T>> input)
    {
        var option = await input.ConfigureAwait(false);
        return option.GetValue();
    }


    /// <summary>
    /// Transforms the value inside an Option using the provided mapping function.
    /// If the Option is None, None is returned without invoking the mapping function.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the input Option.</typeparam>
    /// <typeparam name="TMapped">The type of value contained in the output Option.</typeparam>
    /// <param name="input">The Option to transform.</param>
    /// <param name="mapFn">A function to apply to the contained value if the Option is Some.</param>
    /// <returns>
    /// Some containing the mapped value if the input is Some; otherwise None.
    /// </returns>
    public static Option<TMapped>
    Map<T, TMapped>(
        this Option<T> input,
        Func<T, TMapped> mapFn
    ) =>
        input.Match(
            (val) => Some(mapFn(val)),
            () => none
        );

    /// <summary>
    /// Maps the value of a Task-wrapped Option using a synchronous function.
    /// </summary>
    public static async Task<Option<TMapped>>
    Map<T, TMapped>(
        this Task<Option<T>> taskInput,
        Func<T, TMapped> mapFn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        return input.Map(mapFn);
    }


    /// <summary>
    /// Maps the value of a Task-wrapped Option using an asynchronous function.
    /// </summary>
    public static async Task<Option<TMapped>>
    MapAsync<T, TMapped>(
        this Task<Option<T>> taskInput,
        Func<T, Task<TMapped>> mapFn
    )
    {
        var input = await taskInput.ConfigureAwait(false);
        Option<TMapped> result = input switch {
            Some<T> some => Some(await mapFn(some.Value).ConfigureAwait(false)),
            None<T> => none,
            _ => throw new ArgumentException("Option must be None or Some.")
        };
        return result;
    }


    /// <summary>
    /// Pattern matches on an Option, executing one of two functions depending on
    /// whether the Option contains a value (Some) or is empty (None).
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by both functions.</typeparam>
    /// <param name="opt">The Option to pattern match on.</param>
    /// <param name="someFn">
    /// A function to execute if the Option is Some, receiving the contained value.
    /// </param>
    /// <param name="noneFn">
    /// A function to execute if the Option is None.
    /// </param>
    /// <returns>
    /// The result of executing someFn if the Option is Some, or the result of
    /// executing noneFn if the Option is None.
    /// </returns>
    public static TResult
    Match<T, TResult>(
        this Option<T> opt,
        Func<T, TResult> someFn,
        Func<TResult> noneFn
    ) =>
        opt switch {
            Some<T> some => someFn(some.Value),
            None<T> => noneFn(),
            _ => throw new ArgumentException("Option must be None or Some.")
        };

    /// <summary>
    /// Pattern matches on an Option, executing one of two actions depending on
    /// whether the Option is Some or None.  This overload is for side-effects
    /// only and does not return a value.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to pattern match on.</param>
    /// <param name="someFn">
    /// An action to execute if the Option is Some, receiving the contained value.
    /// </param>
    /// <param name="noneFn">
    /// An action to execute if the Option is None.
    /// </param>
    public static void
    Match<T>(
        this Option<T> opt,
        Action<T> someFn,
        Action noneFn
    )
    {
        opt.Match(
            (t) => {
                someFn(t);
                return 0;
            },
            () => {
                noneFn();
                return 0;
            }
        );
    }

    /// <summary>
    /// Pattern matches on a Task-wrapped Option with synchronous functions,
    /// returning a value.
    /// </summary>
    public static async Task<TResult>
    Match<T, TResult>(
        this Task<Option<T>> taskOpt,
        Func<T, TResult> someFn,
        Func<TResult> noneFn
    )
    {
        var opt = await taskOpt.ConfigureAwait(false);
        return opt.Match(someFn, noneFn);
    }

    /// <summary>
    /// Pattern matches on a Task-wrapped Option with synchronous actions
    /// (side-effects only).
    /// </summary>
    public static async Task
    Match<T>(
        this Task<Option<T>> taskOpt,
        Action<T> someFn,
        Action noneFn
    )
    {
        var opt = await taskOpt.ConfigureAwait(false);
        opt.Match(someFn, noneFn);
    }


    /// <summary>
    /// Pattern matches on a Task-wrapped Option with asynchronous functions,
    /// returning a value.
    /// </summary>
    public static async Task<TResult>
    MatchAsync<T, TResult>(
        this Task<Option<T>> taskOpt,
        Func<T, Task<TResult>> someFn,
        Func<Task<TResult>> noneFn
    )
    {
        var opt = await taskOpt.ConfigureAwait(false);
        TResult result = opt switch {
            Some<T> some => await someFn(some.Value).ConfigureAwait(false),
            None<T> => await noneFn().ConfigureAwait(false),
            _ => throw new ArgumentException("Option must be None or Some.")
        };
        return result;
    }

    /// <summary>
    /// Pattern matches on a Task-wrapped Option with asynchronous actions
    /// (side-effects only).
    /// </summary>
    public static async Task
    MatchAsync<T>(
        this Task<Option<T>> taskOpt,
        Func<T, Task> someFn,
        Func<Task> noneFn
    )
    {
        var opt = await taskOpt.ConfigureAwait(false);
        switch (opt)
        {
            case Some<T> some:
                await someFn(some.Value).ConfigureAwait(false);
                break;
            case None<T>:
                await noneFn().ConfigureAwait(false);
                break;
            default:
                throw new ArgumentException("Option must be None or Some.");
        }
    }


    /// <summary>
    /// Maps the value inside an Option.  Equivalent to Map.
    ///
    /// This method exists so that Options can be used with C# query syntax.  A
    /// query with a single from clause, such as
    /// <c>from x in option select x * 2</c>, is translated by the compiler into
    /// a call to this method.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the input Option.</typeparam>
    /// <typeparam name="TOut">The type of value contained in the output Option.</typeparam>
    /// <param name="input">The input Option.</param>
    /// <param name="selector">
    /// The function to apply to the contained value.  Only invoked if the input
    /// Option is Some.
    /// </param>
    /// <returns>
    /// Some containing the selector's return value if the input is Some;
    /// otherwise None.
    /// </returns>
    public static Option<TOut>
    Select<T, TOut>(
        this Option<T> input,
        Func<T, TOut> selector
    ) =>
        input.Map(selector);


    /// <summary>
    /// Binds an Option to a function returning another Option, then combines
    /// both values using a projection.  Equivalent to Bind followed by Map.
    ///
    /// This method exists so that Options can be used with C# query syntax,
    /// which flattens chains of operations that may not produce a value:
    /// <code>
    /// from userId in FindUserId(userName)
    /// from email in FindEmail(userId)
    /// select $"{userName} &lt;{email}&gt;"
    /// </code>
    /// Each from clause unwraps a value.  If any Option is None, evaluation
    /// stops and the whole expression evaluates to None.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the input Option.</typeparam>
    /// <typeparam name="TNext">
    /// The type of value contained in the Option returned by
    /// <paramref name="next"/>.
    /// </typeparam>
    /// <typeparam name="TOut">The type of value contained in the output Option.</typeparam>
    /// <param name="input">The input Option.</param>
    /// <param name="next">
    /// A function that takes the input's value and returns the next Option.
    /// Only invoked if the input Option is Some.
    /// </param>
    /// <param name="project">
    /// A function that combines both values into the output value.  Only
    /// invoked if both Options are Some.
    /// </param>
    /// <returns>
    /// Some containing the projected value if both Options are Some; otherwise
    /// None.
    /// </returns>
    public static Option<TOut>
    SelectMany<T, TNext, TOut>(
        this Option<T> input,
        Func<T, Option<TNext>> next,
        Func<T, TNext, TOut> project
    ) =>
        input.Bind((a) => next(a).Map((b) => project(a, b)));


    /// <summary>
    /// Extracts the values from all Some Options in a sequence, discarding the
    /// None Options.  The sequence is evaluated lazily.
    ///
    /// This is the Option counterpart of Result's Partition.  Because None
    /// Options carry no data, only the values are returned.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Options.</typeparam>
    /// <param name="input">The sequence of Options.</param>
    /// <returns>
    /// The values of the Some Options, in input order.
    /// </returns>
    public static IEnumerable<T>
    Somes<T>(this IEnumerable<Option<T>> input)
    {
        foreach (Option<T> opt in input)
        {
            if (opt is Some<T> some)
            {
                yield return some.Value;
            }
        }
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Option is None, otherwise
    /// returns the contained value.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to assert.</param>
    /// <param name="errMsg">
    /// Optional custom error message to use if the assertion fails.  If not
    /// provided, a default message will be used.
    /// </param>
    /// <returns>The contained value if the Option is Some.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Option is None.
    /// </exception>
    public static T
    ThrowIfNone<T>(
        this Option<T> opt,
        string? errMsg = null
    ) =>
        opt.Match(
            (t) => t,
            () => throw new InvalidOperationException(errMsg ?? "Expected Some Option but got None.")
        );

    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Option is None,
    /// otherwise returns the contained value.
    /// </summary>
    public static async Task<T>
    ThrowIfNone<T>(
        this Task<Option<T>> taskOpt,
        string? errMsg = null
    )
    {
        var opt = await taskOpt.ConfigureAwait(false);
        return opt.ThrowIfNone(errMsg);
    }


    /// <summary>
    /// Converts a nullable reference to an Option explicitly:
    /// null becomes None, otherwise Some(value).
    /// </summary>
    public static Option<T>
    ToOption<T>(this T value)
    {
        return value is null ? none : Some(value);
    }
}
