using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FnUtil;

public static class IEnumerableExt
{

    /// <summary>
    /// Filters out elements from the collection that match the specified
    /// predicate.  This is the inverse of Where().
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="col">The collection to filter.</param>
    /// <param name="filter">
    /// A predicate function that returns true for elements to exclude.
    /// </param>
    /// <returns>
    /// A collection containing only elements for which the filter returns false.
    /// </returns>
    public static IEnumerable<T>
    Exclude<T>(
        this IEnumerable<T> col,
        Func<T, bool> filter
    ) => col.Where((T item) => !filter(item));


    /// <summary>
    /// Filters out elements from the collection that match any of the specified
    /// predicates.  An element is excluded if any filter returns true for it.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="col">The collection to filter.</param>
    /// <param name="filters">
    /// A collection of predicate functions. An element is excluded if any
    /// filter returns true.
    /// </param>
    /// <returns>
    /// A collection containing only elements for which all filters return false.
    /// </returns>
    public static IEnumerable<T>
    Exclude<T>(
        this IEnumerable<T> col,
        IEnumerable<Func<T, bool>> filters
    ) => col.Where((T item) => !filters.Any((filter) => filter(item)));


    /// <summary>
    /// Reduces the collection to a single value by iteratively applying a
    /// reducer function to each element along with an accumulator.  The reducer
    /// function receives the current accumulator value, the current element,
    /// the current index, and the original collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="Result">The type of the accumulated result.</typeparam>
    /// <param name="col">The collection to reduce.</param>
    /// <param name="reduceFn">
    /// A function that takes the current accumulator, current element, current
    /// index, and the original collection, and returns the new accumulator
    /// value.
    /// </param>
    /// <param name="initialAcc">The initial value of the accumulator.</param>
    /// <returns>The final accumulated value after processing all elements.</returns>
    public static TResult
    Reduce<T, TResult>(
        this IEnumerable<T> col,
        Func<TResult, T, uint, IEnumerable<T>, TResult> reduceFn,
        TResult initialAcc
    )
    {
        TResult acc = initialAcc;
        uint curIndex = 0;
        foreach (var curItem in col)
        {
            acc = reduceFn(acc, curItem, curIndex, col);
            curIndex++;
        }
        return acc;
    }


    /// <summary>
    /// Executes a side-effect function for each element in the collection
    /// without transforming the collection.  This method is useful for
    /// operations like logging or debugging within a method chain.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="col">The collection to iterate over.</param>
    /// <param name="fn">
    /// An action to perform for each element in the collection.
    /// </param>
    /// <returns>The original collection, unchanged.</returns>
    public static IEnumerable<T>
    Tap<T>(
        this IEnumerable<T> col,
        Action<T> fn
    )
    {
        foreach (var cur in col)
        {
            fn(cur);
        }
        return col;
    }
}
