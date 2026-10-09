namespace FnUtil;

public static class TaskUtil
{

    ////////////////////////////////////////////////////////////////////////////
    // All() overloads
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Takes multiple Tasks as input and returns a single Task that resolves
    /// when all of the input Tasks resolve with a tuple of their results.  It
    /// faults when any of the input Tasks fault.  This is a more type safe
    /// version of Task.WhenAll().
    /// </summary>
    /// <returns>If all Tasks resolve, the returned Task resolves with a tuple
    /// containing their returned values.  Otherwise, the returned Task faults
    /// with the first fault.</returns>
    public static async Task<(T1, T2)>
    All<T1, T2>(
        Task<T1> t1,
        Task<T2> t2
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false));


    public static async Task<(T1, T2, T3)>
    All<T1, T2, T3>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false));


    public static async Task<(T1, T2, T3, T4)>
    All<T1, T2, T3, T4>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3,
        Task<T4> t4
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false), await t4.ConfigureAwait(false));


    public static async Task<(T1, T2, T3, T4, T5)>
    All<T1, T2, T3, T4, T5>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3,
        Task<T4> t4,
        Task<T5> t5
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false), await t4.ConfigureAwait(false), await t5.ConfigureAwait(false));


    public static async Task<(T1, T2, T3, T4, T5, T6)>
    All<T1, T2, T3, T4, T5, T6>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3,
        Task<T4> t4,
        Task<T5> t5,
        Task<T6> t6
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false), await t4.ConfigureAwait(false), await t5.ConfigureAwait(false), await t6.ConfigureAwait(false));


    public static async Task<(T1, T2, T3, T4, T5, T6, T7)>
    All<T1, T2, T3, T4, T5, T6, T7>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3,
        Task<T4> t4,
        Task<T5> t5,
        Task<T6> t6,
        Task<T7> t7
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false), await t4.ConfigureAwait(false), await t5.ConfigureAwait(false), await t6.ConfigureAwait(false), await t7.ConfigureAwait(false));


    public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8)>
    All<T1, T2, T3, T4, T5, T6, T7, T8>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3,
        Task<T4> t4,
        Task<T5> t5,
        Task<T6> t6,
        Task<T7> t7,
        Task<T8> t8
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false), await t4.ConfigureAwait(false), await t5.ConfigureAwait(false), await t6.ConfigureAwait(false), await t7.ConfigureAwait(false), await t8.ConfigureAwait(false));


    public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>
    All<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3,
        Task<T4> t4,
        Task<T5> t5,
        Task<T6> t6,
        Task<T7> t7,
        Task<T8> t8,
        Task<T9> t9
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false), await t4.ConfigureAwait(false), await t5.ConfigureAwait(false), await t6.ConfigureAwait(false), await t7.ConfigureAwait(false), await t8.ConfigureAwait(false), await t9.ConfigureAwait(false));


    public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>
    All<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        Task<T1> t1,
        Task<T2> t2,
        Task<T3> t3,
        Task<T4> t4,
        Task<T5> t5,
        Task<T6> t6,
        Task<T7> t7,
        Task<T8> t8,
        Task<T9> t9,
        Task<T10> t10
    )
 => (await t1.ConfigureAwait(false), await t2.ConfigureAwait(false), await t3.ConfigureAwait(false), await t4.ConfigureAwait(false), await t5.ConfigureAwait(false), await t6.ConfigureAwait(false), await t7.ConfigureAwait(false), await t8.ConfigureAwait(false), await t9.ConfigureAwait(false), await t10.ConfigureAwait(false));
}
