using System;
using static FnUtil.F;

namespace FnUtil;


// Option extension methods.
public static class OptionExt
{
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
    /// Returns the contained value if Some, or the provided default value if None.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to extract a value from.</param>
    /// <param name="defaultValue">The value to return if the Option is None.</param>
    /// <returns>The contained value or the default value.</returns>
    public static T
    GetOrElse<T>(this Option<T> opt, T defaultValue) =>
        opt.Match(
            (t) => t,
            () => defaultValue
        );

    /// <summary>
    /// Returns the contained value if Some, or computes a fallback value if None.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to extract a value from.</param>
    /// <param name="fallback">A function that produces the fallback value.</param>
    /// <returns>The contained value or the computed fallback value.</returns>
    public static T
    GetOrElse<T>(this Option<T> opt, Func<T> fallback) =>
        opt.Match(
            (t) => t,
            () => fallback()
        );

    /// <summary>
    /// Returns the contained value if Some, or asynchronously computes a
    /// fallback value if None.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the Option.</typeparam>
    /// <param name="opt">The Option to extract a value from.</param>
    /// <param name="fallback">An async function that produces the fallback value.</param>
    /// <returns>A Task containing the contained value or the computed fallback value.</returns>
    public static Task<T>
    GetOrElse<T>(this Option<T> opt, Func<Task<T>> fallback) =>
        opt.Match(
            (t) => Task.FromResult(t),
            () => fallback()
        );


    //--------------------------------------------------------------------------
    // GetValue
    //--------------------------------------------------------------------------

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
}
