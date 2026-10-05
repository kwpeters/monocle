using System;
using static FnUtil.F;

namespace FnUtil;


// Option extension methods.
public static class OptionExt
{
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
            Some<T>(var t) => someFn(t),
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
}
