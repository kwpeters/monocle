//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

namespace FnUtil;

public static partial class F
{
    /// <summary>
    /// Pipes an initial value through a single async function.
    /// </summary>
    /// <typeparam name="T001">The type of the initial value.</typeparam>
    /// <typeparam name="T002">The type returned by the function.</typeparam>
    /// <param name="v">The initial value.</param>
    /// <param name="f001002">An async function to apply.</param>
    /// <returns>A task that resolves to the final result.</returns>
    public static async Task<T002>
    PipeAsync<T001, T002>(
        T001 v,
        Func<T001, Task<T002>> f001002
    )
 => await f001002(v).ConfigureAwait(false);


    /// <summary>
    /// Pipes an initial value through two async functions.
    /// </summary>
    /// <typeparam name="T001">The type of the initial value.</typeparam>
    /// <typeparam name="T002">The type returned by the first function.</typeparam>
    /// <typeparam name="T003">The type returned by the second function.</typeparam>
    /// <param name="v">The initial value.</param>
    /// <param name="f001002">The first async function.</param>
    /// <param name="f002003">The second async function.</param>
    /// <returns>A task that resolves to the final result.</returns>
    public static async Task<T003>
    PipeAsync<T001, T002, T003>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        return await f002003(v002).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 3 async functions.
    /// </summary>
    /// <typeparam name="T001">The type of the initial value.</typeparam>
    /// <typeparam name="T002">The type returned by the first function.</typeparam>
    /// <typeparam name="T003">The type returned by the second function.</typeparam>
    /// <typeparam name="T004">The type returned by the third function.</typeparam>
    /// <param name="v">The initial value.</param>
    /// <param name="f001002">The first async function.</param>
    /// <param name="f002003">The second async function.</param>
    /// <param name="f003004">The third async function.</param>
    /// <returns>A task that resolves to the final result.</returns>
    public static async Task<T004>
    PipeAsync<T001, T002, T003, T004>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        return await f003004(v003).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 4 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T005>
    PipeAsync<T001, T002, T003, T004, T005>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        return await f004005(v004).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 5 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T006>
    PipeAsync<T001, T002, T003, T004, T005, T006>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        return await f005006(v005).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 6 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T007>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        return await f006007(v006).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 7 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T008>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        return await f007008(v007).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 8 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T009>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        return await f008009(v008).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 9 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T010>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        return await f009010(v009).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 10 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T011>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        return await f010011(v010).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 11 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T012>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        return await f011012(v011).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 12 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T013>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        return await f012013(v012).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 13 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T014>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        return await f013014(v013).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 14 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T015>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014, T015>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014,
        Func<T014, Task<T015>> f014015
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        var v014 = await f013014(v013).ConfigureAwait(false);
        return await f014015(v014).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 15 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T016>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014, T015, T016>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014,
        Func<T014, Task<T015>> f014015,
        Func<T015, Task<T016>> f015016
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        var v014 = await f013014(v013).ConfigureAwait(false);
        var v015 = await f014015(v014).ConfigureAwait(false);
        return await f015016(v015).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 16 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T017>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014, T015, T016, T017>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014,
        Func<T014, Task<T015>> f014015,
        Func<T015, Task<T016>> f015016,
        Func<T016, Task<T017>> f016017
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        var v014 = await f013014(v013).ConfigureAwait(false);
        var v015 = await f014015(v014).ConfigureAwait(false);
        var v016 = await f015016(v015).ConfigureAwait(false);
        return await f016017(v016).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 17 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T018>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014, T015, T016, T017, T018>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014,
        Func<T014, Task<T015>> f014015,
        Func<T015, Task<T016>> f015016,
        Func<T016, Task<T017>> f016017,
        Func<T017, Task<T018>> f017018
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        var v014 = await f013014(v013).ConfigureAwait(false);
        var v015 = await f014015(v014).ConfigureAwait(false);
        var v016 = await f015016(v015).ConfigureAwait(false);
        var v017 = await f016017(v016).ConfigureAwait(false);
        return await f017018(v017).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 18 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T019>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014, T015, T016, T017, T018, T019>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014,
        Func<T014, Task<T015>> f014015,
        Func<T015, Task<T016>> f015016,
        Func<T016, Task<T017>> f016017,
        Func<T017, Task<T018>> f017018,
        Func<T018, Task<T019>> f018019
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        var v014 = await f013014(v013).ConfigureAwait(false);
        var v015 = await f014015(v014).ConfigureAwait(false);
        var v016 = await f015016(v015).ConfigureAwait(false);
        var v017 = await f016017(v016).ConfigureAwait(false);
        var v018 = await f017018(v017).ConfigureAwait(false);
        return await f018019(v018).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 19 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T020>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014, T015, T016, T017, T018, T019, T020>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014,
        Func<T014, Task<T015>> f014015,
        Func<T015, Task<T016>> f015016,
        Func<T016, Task<T017>> f016017,
        Func<T017, Task<T018>> f017018,
        Func<T018, Task<T019>> f018019,
        Func<T019, Task<T020>> f019020
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        var v014 = await f013014(v013).ConfigureAwait(false);
        var v015 = await f014015(v014).ConfigureAwait(false);
        var v016 = await f015016(v015).ConfigureAwait(false);
        var v017 = await f016017(v016).ConfigureAwait(false);
        var v018 = await f017018(v017).ConfigureAwait(false);
        var v019 = await f018019(v018).ConfigureAwait(false);
        return await f019020(v019).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 20 async functions.
    /// </summary>
    /// <remarks>
    /// To use synchronous functions, wrap them: x =&gt;
    /// Task.FromResult(syncFunc(x))
    /// </remarks>
    public static async Task<T021>
    PipeAsync<T001, T002, T003, T004, T005, T006, T007, T008, T009, T010, T011, T012, T013, T014, T015, T016, T017, T018, T019, T020, T021>(
        T001 v,
        Func<T001, Task<T002>> f001002,
        Func<T002, Task<T003>> f002003,
        Func<T003, Task<T004>> f003004,
        Func<T004, Task<T005>> f004005,
        Func<T005, Task<T006>> f005006,
        Func<T006, Task<T007>> f006007,
        Func<T007, Task<T008>> f007008,
        Func<T008, Task<T009>> f008009,
        Func<T009, Task<T010>> f009010,
        Func<T010, Task<T011>> f010011,
        Func<T011, Task<T012>> f011012,
        Func<T012, Task<T013>> f012013,
        Func<T013, Task<T014>> f013014,
        Func<T014, Task<T015>> f014015,
        Func<T015, Task<T016>> f015016,
        Func<T016, Task<T017>> f016017,
        Func<T017, Task<T018>> f017018,
        Func<T018, Task<T019>> f018019,
        Func<T019, Task<T020>> f019020,
        Func<T020, Task<T021>> f020021
    )
    {
        var v002 = await f001002(v).ConfigureAwait(false);
        var v003 = await f002003(v002).ConfigureAwait(false);
        var v004 = await f003004(v003).ConfigureAwait(false);
        var v005 = await f004005(v004).ConfigureAwait(false);
        var v006 = await f005006(v005).ConfigureAwait(false);
        var v007 = await f006007(v006).ConfigureAwait(false);
        var v008 = await f007008(v007).ConfigureAwait(false);
        var v009 = await f008009(v008).ConfigureAwait(false);
        var v010 = await f009010(v009).ConfigureAwait(false);
        var v011 = await f010011(v010).ConfigureAwait(false);
        var v012 = await f011012(v011).ConfigureAwait(false);
        var v013 = await f012013(v012).ConfigureAwait(false);
        var v014 = await f013014(v013).ConfigureAwait(false);
        var v015 = await f014015(v014).ConfigureAwait(false);
        var v016 = await f015016(v015).ConfigureAwait(false);
        var v017 = await f016017(v016).ConfigureAwait(false);
        var v018 = await f017018(v017).ConfigureAwait(false);
        var v019 = await f018019(v018).ConfigureAwait(false);
        var v020 = await f019020(v019).ConfigureAwait(false);
        return await f020021(v020).ConfigureAwait(false);
    }
}
