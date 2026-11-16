using static FnUtil.F;

namespace FnUtil;


public static class ActionExt
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static Func<Unit>
    ToFunc(
        this Action actionFn
    )
        => () => { actionFn(); return unit; };


    public static Func<T1, Unit>
    ToFunc<T1>(
        this Action<T1> actionFn
    )
        => t => { actionFn(t); return unit; };


    public static Func<T1, T2, Unit>
    ToFunc<T1, T2>(
        this Action<T1, T2> actionFn
    )
        => (T1 t1, T2 t2) => { actionFn(t1, t2); return unit; };
}


public static partial class F
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    /// <summary>
    /// Invokes the action and returns unit.
    /// </summary>
    /// <param name="fn">The void returning action function.</param>
    /// <returns>unit</returns>
    public static Unit Effect(Action fn)
    {
        fn();
        return unit;
    }
}
