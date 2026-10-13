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
    /// Combines a tuple of 2 Options, which may have different value types,
    /// into a single Option containing a tuple of their values.  If any
    /// Option is None, returns None.
    ///
    /// Use this to combine a fixed number of independent Options, for
    /// example <c>(o1, o2).AllOrNone()</c>.  To combine a sequence of
    /// Options with the same value type, use the IEnumerable overload.
    /// </summary>
    public static Option<(T1, T2)>
    AllOrNone<T1, T2>(
        this (
            Option<T1>,
            Option<T2>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        select (v1, v2);

    /// <summary>
    /// Combines a tuple of 3 Options, which may have different value types,
    /// into a single Option containing a tuple of their values.  If any
    /// Option is None, returns None.
    ///
    /// Use this to combine a fixed number of independent Options, for
    /// example <c>(o1, o2).AllOrNone()</c>.  To combine a sequence of
    /// Options with the same value type, use the IEnumerable overload.
    /// </summary>
    public static Option<(T1, T2, T3)>
    AllOrNone<T1, T2, T3>(
        this (
            Option<T1>,
            Option<T2>,
            Option<T3>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        select (v1, v2, v3);

    /// <summary>
    /// Combines a tuple of 4 Options, which may have different value types,
    /// into a single Option containing a tuple of their values.  If any
    /// Option is None, returns None.
    ///
    /// Use this to combine a fixed number of independent Options, for
    /// example <c>(o1, o2).AllOrNone()</c>.  To combine a sequence of
    /// Options with the same value type, use the IEnumerable overload.
    /// </summary>
    public static Option<(T1, T2, T3, T4)>
    AllOrNone<T1, T2, T3, T4>(
        this (
            Option<T1>,
            Option<T2>,
            Option<T3>,
            Option<T4>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        from v4 in input.Item4
        select (v1, v2, v3, v4);

    /// <summary>
    /// Combines a tuple of 5 Options, which may have different value types,
    /// into a single Option containing a tuple of their values.  If any
    /// Option is None, returns None.
    ///
    /// Use this to combine a fixed number of independent Options, for
    /// example <c>(o1, o2).AllOrNone()</c>.  To combine a sequence of
    /// Options with the same value type, use the IEnumerable overload.
    /// </summary>
    public static Option<(T1, T2, T3, T4, T5)>
    AllOrNone<T1, T2, T3, T4, T5>(
        this (
            Option<T1>,
            Option<T2>,
            Option<T3>,
            Option<T4>,
            Option<T5>
        ) input
    ) =>
        from v1 in input.Item1
        from v2 in input.Item2
        from v3 in input.Item3
        from v4 in input.Item4
        from v5 in input.Item5
        select (v1, v2, v3, v4, v5);

    /// <summary>
    /// Combines a tuple of 6 Options, which may have different value types,
    /// into a single Option containing a tuple of their values.  If any
    /// Option is None, returns None.
    ///
    /// Use this to combine a fixed number of independent Options, for
    /// example <c>(o1, o2).AllOrNone()</c>.  To combine a sequence of
    /// Options with the same value type, use the IEnumerable overload.
    /// </summary>
    public static Option<(T1, T2, T3, T4, T5, T6)>
    AllOrNone<T1, T2, T3, T4, T5, T6>(
        this (
            Option<T1>,
            Option<T2>,
            Option<T3>,
            Option<T4>,
            Option<T5>,
            Option<T6>
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
    /// Combines a tuple of 7 Options, which may have different value types,
    /// into a single Option containing a tuple of their values.  If any
    /// Option is None, returns None.
    ///
    /// Use this to combine a fixed number of independent Options, for
    /// example <c>(o1, o2).AllOrNone()</c>.  To combine a sequence of
    /// Options with the same value type, use the IEnumerable overload.
    /// </summary>
    public static Option<(T1, T2, T3, T4, T5, T6, T7)>
    AllOrNone<T1, T2, T3, T4, T5, T6, T7>(
        this (
            Option<T1>,
            Option<T2>,
            Option<T3>,
            Option<T4>,
            Option<T5>,
            Option<T6>,
            Option<T7>
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
        Option<T> input = await taskInput.ConfigureAwait(false);
        return input.Bind(fn);
    }


    /// <summary>
    /// Monadic bind for an Option with an asynchronous function.  If the input
    /// Option is Some, awaits the function and returns its Option.  If the
    /// input Option is None, returns None without invoking the function.
    /// </summary>
    public static Task<Option<TOut>>
    BindAsync<T, TOut>(
        this Option<T> input,
        Func<T, Task<Option<TOut>>> fn
    ) =>
        Task.FromResult(input).BindAsync(fn);

    /// <summary>
    /// Monadic bind for a Task-wrapped Option with an asynchronous function.
    /// </summary>
    public static async Task<Option<TOut>>
    BindAsync<T, TOut>(
        this Task<Option<T>> taskInput,
        Func<T, Task<Option<TOut>>> fn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        Option<TOut> result = input switch {
            Some<T> some => await fn(some.Value).ConfigureAwait(false),
            None<T> => none,
            _ => throw new ArgumentException("Option must be None or Some.")
        };
        return result;
    }


    /// <summary>
    /// If the input Option is None, invokes the provided function and returns
    /// its Option.  If the input Option is Some, returns it without invoking
    /// the function.  This allows falling back to an alternative when a
    /// previous operation produced no value.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="input">The input Option.</param>
    /// <param name="fn">
    /// A function that returns the fallback Option.  This function is only
    /// invoked if the input Option is None.
    /// </param>
    /// <returns>
    /// The input Option if it is Some; otherwise the Option returned by fn.
    /// </returns>
    public static Option<T>
    BindNone<T>(
        this Option<T> input,
        Func<Option<T>> fn
    ) =>
        input.IsSome ? input : fn();

    /// <summary>
    /// BindNone for a Task-wrapped Option with a synchronous function.
    /// </summary>
    public static async Task<Option<T>>
    BindNone<T>(
        this Task<Option<T>> taskInput,
        Func<Option<T>> fn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        return input.BindNone(fn);
    }


    /// <summary>
    /// BindNone for an Option with an asynchronous function.
    /// </summary>
    public static Task<Option<T>>
    BindNoneAsync<T>(
        this Option<T> input,
        Func<Task<Option<T>>> fn
    ) =>
        input.IsSome ? Task.FromResult(input) : fn();

    /// <summary>
    /// BindNone for a Task-wrapped Option with an asynchronous function.
    /// </summary>
    public static async Task<Option<T>>
    BindNoneAsync<T>(
        this Task<Option<T>> taskInput,
        Func<Task<Option<T>>> fn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        return await input.BindNoneAsync(fn).ConfigureAwait(false);
    }


    /// <summary>
    /// Maps each item in a sequence using a function that returns an Option,
    /// keeping only the values of the Some results.  The sequence is
    /// evaluated lazily.  Equivalent to Select(fn).Somes().
    /// </summary>
    /// <typeparam name="TIn">The type of the input items.</typeparam>
    /// <typeparam name="TOut">The type of the output values.</typeparam>
    /// <param name="input">The items to map.</param>
    /// <param name="fn">
    /// The mapping function.  Items mapped to None are omitted from the output.
    /// </param>
    /// <returns>The values of the Some results, in input order.</returns>
    public static IEnumerable<TOut>
    Choose<TIn, TOut>(
        this IEnumerable<TIn> input,
        Func<TIn, Option<TOut>> fn
    ) =>
        input.Select(fn).Somes();


    /// <summary>
    /// Maps each item in a sequence using an asynchronous function that returns
    /// an Option, keeping only the values of the Some results.  Items are
    /// mapped one at a time, in order.
    /// </summary>
    /// <typeparam name="TIn">The type of the input items.</typeparam>
    /// <typeparam name="TOut">The type of the output values.</typeparam>
    /// <param name="input">The items to map.</param>
    /// <param name="fn">
    /// The async mapping function.  Items mapped to None are omitted from the
    /// output.
    /// </param>
    /// <returns>A Task for the values of the Some results, in input order.</returns>
    public static async Task<IEnumerable<TOut>>
    ChooseAsync<TIn, TOut>(
        this IEnumerable<TIn> input,
        Func<TIn, Task<Option<TOut>>> fn
    )
    {
        List<TOut> output = [];

        foreach (TIn item in input)
        {
            Option<TOut> opt = await fn(item).ConfigureAwait(false);
            if (opt is Some<TOut> some)
            {
                output.Add(some.Value);
            }
        }

        return output;
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
        Option<T> opt = await taskInput.ConfigureAwait(false);
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
        Option<T> opt = await taskInput.ConfigureAwait(false);
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
        Option<T> opt = await taskInput.ConfigureAwait(false);
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
        Option<T> input = await taskInput.ConfigureAwait(false);
        return input.Gate(gateFn);
    }


    /// <summary>
    /// Gates an Option by applying an asynchronous check function.
    /// </summary>
    public static Task<Option<T>>
    GateAsync<T, TFn>(
        this Option<T> input,
        Func<T, Task<Option<TFn>>> gateFn
    ) =>
        Task.FromResult(input).GateAsync(gateFn);

    /// <summary>
    /// Gates a Task-wrapped Option by applying an asynchronous check function.
    /// </summary>
    public static async Task<Option<T>>
    GateAsync<T, TFn>(
        this Task<Option<T>> taskInput,
        Func<T, Task<Option<TFn>>> gateFn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        switch (input)
        {
            case Some<T> some:
            {
                Option<TFn> gateRes = await gateFn(some.Value).ConfigureAwait(false);
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
        Option<T> option = await input.ConfigureAwait(false);
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
        Option<T> input = await taskInput.ConfigureAwait(false);
        return input.Map(mapFn);
    }


    /// <summary>
    /// Maps the value of an Option using an asynchronous function.
    /// </summary>
    public static Task<Option<TMapped>>
    MapAsync<T, TMapped>(
        this Option<T> input,
        Func<T, Task<TMapped>> mapFn
    ) =>
        Task.FromResult(input).MapAsync(mapFn);

    /// <summary>
    /// Maps the value of a Task-wrapped Option using an asynchronous function.
    /// </summary>
    public static async Task<Option<TMapped>>
    MapAsync<T, TMapped>(
        this Task<Option<T>> taskInput,
        Func<T, Task<TMapped>> mapFn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
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
        Option<T> opt = await taskOpt.ConfigureAwait(false);
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
        Option<T> opt = await taskOpt.ConfigureAwait(false);
        opt.Match(someFn, noneFn);
    }


    /// <summary>
    /// Pattern matches on an Option with asynchronous functions, returning a
    /// value.
    /// </summary>
    public static Task<TResult>
    MatchAsync<T, TResult>(
        this Option<T> opt,
        Func<T, Task<TResult>> someFn,
        Func<Task<TResult>> noneFn
    ) =>
        Task.FromResult(opt).MatchAsync(someFn, noneFn);

    /// <summary>
    /// Pattern matches on an Option with asynchronous actions (side-effects
    /// only).
    /// </summary>
    public static Task
    MatchAsync<T>(
        this Option<T> opt,
        Func<T, Task> someFn,
        Func<Task> noneFn
    ) =>
        Task.FromResult(opt).MatchAsync(someFn, noneFn);

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
        Option<T> opt = await taskOpt.ConfigureAwait(false);
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
        Option<T> opt = await taskOpt.ConfigureAwait(false);
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
    /// Requires that a list contain at least one element.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="input">The list to check.</param>
    /// <returns>Some containing the list if it is not empty; otherwise None.</returns>
    public static Option<IReadOnlyList<T>>
    RequireNonEmpty<T>(this IReadOnlyList<T> input) =>
        input.Count > 0 ? Some(input) : none;


    /// <summary>
    /// Requires that a sequence contain exactly one element.  At most two
    /// elements are enumerated.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="input">The sequence to check.</param>
    /// <returns>
    /// Some containing the single element if there is exactly one; otherwise
    /// None.
    /// </returns>
    public static Option<T>
    RequireSingle<T>(this IEnumerable<T> input)
    {
        using IEnumerator<T> enumerator = input.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return none;
        }

        T first = enumerator.Current;
        return enumerator.MoveNext() ? none : Some(first);
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
    /// If the Option is None, performs a side-effect (for example, logging).
    /// Returns the Option unchanged.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="input">The input Option.</param>
    /// <param name="fn">
    /// The action to invoke.  Only invoked if the input Option is None.
    /// </param>
    /// <returns>The input Option.</returns>
    public static Option<T>
    TapNone<T>(
        this Option<T> input,
        Action fn
    )
    {
        if (input.IsNone)
        {
            fn();
        }

        return input;
    }


    /// <summary>
    /// TapNone for a Task-wrapped Option with a synchronous action.
    /// </summary>
    public static async Task<Option<T>>
    TapNone<T>(
        this Task<Option<T>> taskInput,
        Action fn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        return input.TapNone(fn);
    }


    /// <summary>
    /// TapNone for an Option with an asynchronous action.
    /// </summary>
    public static async Task<Option<T>>
    TapNoneAsync<T>(
        this Option<T> input,
        Func<Task> fn
    )
    {
        if (input.IsNone)
        {
            await fn().ConfigureAwait(false);
        }

        return input;
    }


    /// <summary>
    /// TapNone for a Task-wrapped Option with an asynchronous action.
    /// </summary>
    public static async Task<Option<T>>
    TapNoneAsync<T>(
        this Task<Option<T>> taskInput,
        Func<Task> fn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        return await input.TapNoneAsync(fn).ConfigureAwait(false);
    }


    /// <summary>
    /// If the Option is Some, performs a side-effect (for example, logging)
    /// using the contained value.  Returns the Option unchanged.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="input">The input Option.</param>
    /// <param name="fn">
    /// The action to invoke with the contained value.  Only invoked if the
    /// input Option is Some.
    /// </param>
    /// <returns>The input Option.</returns>
    public static Option<T>
    TapSome<T>(
        this Option<T> input,
        Action<T> fn
    )
    {
        if (input is Some<T> some)
        {
            fn(some.Value);
        }

        return input;
    }


    /// <summary>
    /// TapSome for a Task-wrapped Option with a synchronous action.
    /// </summary>
    public static async Task<Option<T>>
    TapSome<T>(
        this Task<Option<T>> taskInput,
        Action<T> fn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        return input.TapSome(fn);
    }


    /// <summary>
    /// TapSome for an Option with an asynchronous action.
    /// </summary>
    public static async Task<Option<T>>
    TapSomeAsync<T>(
        this Option<T> input,
        Func<T, Task> fn
    )
    {
        if (input is Some<T> some)
        {
            await fn(some.Value).ConfigureAwait(false);
        }

        return input;
    }


    /// <summary>
    /// TapSome for a Task-wrapped Option with an asynchronous action.
    /// </summary>
    public static async Task<Option<T>>
    TapSomeAsync<T>(
        this Task<Option<T>> taskInput,
        Func<T, Task> fn
    )
    {
        Option<T> input = await taskInput.ConfigureAwait(false);
        return await input.TapSomeAsync(fn).ConfigureAwait(false);
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
        Option<T> opt = await taskOpt.ConfigureAwait(false);
        return opt.ThrowIfNone(errMsg);
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Option is Some.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to assert.</param>
    /// <param name="errMsg">
    /// Optional custom error message to use if the assertion fails.  If not
    /// provided, a default message including the contained value will be used.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Option is Some.
    /// </exception>
    public static void
    ThrowIfSome<T>(
        this Option<T> opt,
        string? errMsg = null
    )
    {
        if (opt is Some<T> some)
        {
            throw new InvalidOperationException(errMsg ?? $"Expected None Option but got Some: {some.Value}");
        }
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Option is Some.  The
    /// exception message is produced from the contained value by the specified
    /// function.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to assert.</param>
    /// <param name="errMsgFn">
    /// A function that produces the exception message from the contained
    /// value.  Only invoked if the Option is Some.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Option is Some.
    /// </exception>
    public static void
    ThrowIfSome<T>(
        this Option<T> opt,
        Func<T, string> errMsgFn
    )
    {
        if (opt is Some<T> some)
        {
            throw new InvalidOperationException(errMsgFn(some.Value));
        }
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Option is Some.
    /// </summary>
    public static async Task
    ThrowIfSome<T>(
        this Task<Option<T>> taskOpt,
        string? errMsg = null
    )
    {
        Option<T> opt = await taskOpt.ConfigureAwait(false);
        opt.ThrowIfSome(errMsg);
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Option is Some.
    /// The exception message is produced from the contained value by the
    /// specified function.
    /// </summary>
    public static async Task
    ThrowIfSome<T>(
        this Task<Option<T>> taskOpt,
        Func<T, string> errMsgFn
    )
    {
        Option<T> opt = await taskOpt.ConfigureAwait(false);
        opt.ThrowIfSome(errMsgFn);
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Option is Some.  The
    /// exception message is produced from the contained value by the specified
    /// asynchronous function.
    /// </summary>
    public static async Task
    ThrowIfSomeAsync<T>(
        this Option<T> opt,
        Func<T, Task<string>> errMsgFn
    )
    {
        if (opt is Some<T> some)
        {
            string msg = await errMsgFn(some.Value).ConfigureAwait(false);
            throw new InvalidOperationException(msg);
        }
    }


    /// <summary>
    /// Throws an InvalidOperationException if the Task-wrapped Option is Some.
    /// The exception message is produced from the contained value by the
    /// specified asynchronous function.
    /// </summary>
    public static async Task
    ThrowIfSomeAsync<T>(
        this Task<Option<T>> taskOpt,
        Func<T, Task<string>> errMsgFn
    )
    {
        Option<T> opt = await taskOpt.ConfigureAwait(false);
        await opt.ThrowIfSomeAsync(errMsgFn).ConfigureAwait(false);
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


    /// <summary>
    /// Converts a nullable value type to an Option explicitly:
    /// null becomes None, otherwise Some(value).
    /// </summary>
    public static Option<T>
    ToOption<T>(this T? value) where T : struct =>
        value.HasValue ? Some(value.Value) : none;

    /// <summary>
    /// Converts a bool to an Option.  Returns Some(value) when the condition is
    /// true; otherwise returns None.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="condition">The condition.</param>
    /// <param name="value">The value to wrap when the condition is true.</param>
    public static Option<T>
    ToOption<T>(
        this bool condition,
        T value
    ) =>
        condition ? Some(value) : none;
}
