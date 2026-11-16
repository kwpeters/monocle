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
    /// <param name="f001002Fn">An async function to apply.</param>
    /// <returns>A task that resolves to the final result.</returns>
    public static async Task<T002>
    PipeAsync<T001, T002>(
        T001 v,
        Func<T001, Task<T002>> f001002Fn
    )
 => await f001002Fn(v).ConfigureAwait(false);


    /// <summary>
    /// Pipes an initial value through two async functions.
    /// </summary>
    /// <typeparam name="T001">The type of the initial value.</typeparam>
    /// <typeparam name="T002">The type returned by the first function.</typeparam>
    /// <typeparam name="T003">The type returned by the second function.</typeparam>
    /// <param name="v">The initial value.</param>
    /// <param name="f001002Fn">The first async function.</param>
    /// <param name="f002003Fn">The second async function.</param>
    /// <returns>A task that resolves to the final result.</returns>
    public static async Task<T003>
    PipeAsync<T001, T002, T003>(
        T001 v,
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        return await f002003Fn(v002).ConfigureAwait(false);
    }


    /// <summary>
    /// Pipes an initial value through 3 async functions.
    /// </summary>
    /// <typeparam name="T001">The type of the initial value.</typeparam>
    /// <typeparam name="T002">The type returned by the first function.</typeparam>
    /// <typeparam name="T003">The type returned by the second function.</typeparam>
    /// <typeparam name="T004">The type returned by the third function.</typeparam>
    /// <param name="v">The initial value.</param>
    /// <param name="f001002Fn">The first async function.</param>
    /// <param name="f002003Fn">The second async function.</param>
    /// <param name="f003004Fn">The third async function.</param>
    /// <returns>A task that resolves to the final result.</returns>
    public static async Task<T004>
    PipeAsync<T001, T002, T003, T004>(
        T001 v,
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        return await f003004Fn(v003).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        return await f004005Fn(v004).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        return await f005006Fn(v005).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        return await f006007Fn(v006).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        return await f007008Fn(v007).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        return await f008009Fn(v008).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        return await f009010Fn(v009).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        return await f010011Fn(v010).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        return await f011012Fn(v011).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        return await f012013Fn(v012).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        return await f013014Fn(v013).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn,
        Func<T014, Task<T015>> f014015Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        T014 v014 = await f013014Fn(v013).ConfigureAwait(false);
        return await f014015Fn(v014).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn,
        Func<T014, Task<T015>> f014015Fn,
        Func<T015, Task<T016>> f015016Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        T014 v014 = await f013014Fn(v013).ConfigureAwait(false);
        T015 v015 = await f014015Fn(v014).ConfigureAwait(false);
        return await f015016Fn(v015).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn,
        Func<T014, Task<T015>> f014015Fn,
        Func<T015, Task<T016>> f015016Fn,
        Func<T016, Task<T017>> f016017Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        T014 v014 = await f013014Fn(v013).ConfigureAwait(false);
        T015 v015 = await f014015Fn(v014).ConfigureAwait(false);
        T016 v016 = await f015016Fn(v015).ConfigureAwait(false);
        return await f016017Fn(v016).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn,
        Func<T014, Task<T015>> f014015Fn,
        Func<T015, Task<T016>> f015016Fn,
        Func<T016, Task<T017>> f016017Fn,
        Func<T017, Task<T018>> f017018Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        T014 v014 = await f013014Fn(v013).ConfigureAwait(false);
        T015 v015 = await f014015Fn(v014).ConfigureAwait(false);
        T016 v016 = await f015016Fn(v015).ConfigureAwait(false);
        T017 v017 = await f016017Fn(v016).ConfigureAwait(false);
        return await f017018Fn(v017).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn,
        Func<T014, Task<T015>> f014015Fn,
        Func<T015, Task<T016>> f015016Fn,
        Func<T016, Task<T017>> f016017Fn,
        Func<T017, Task<T018>> f017018Fn,
        Func<T018, Task<T019>> f018019Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        T014 v014 = await f013014Fn(v013).ConfigureAwait(false);
        T015 v015 = await f014015Fn(v014).ConfigureAwait(false);
        T016 v016 = await f015016Fn(v015).ConfigureAwait(false);
        T017 v017 = await f016017Fn(v016).ConfigureAwait(false);
        T018 v018 = await f017018Fn(v017).ConfigureAwait(false);
        return await f018019Fn(v018).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn,
        Func<T014, Task<T015>> f014015Fn,
        Func<T015, Task<T016>> f015016Fn,
        Func<T016, Task<T017>> f016017Fn,
        Func<T017, Task<T018>> f017018Fn,
        Func<T018, Task<T019>> f018019Fn,
        Func<T019, Task<T020>> f019020Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        T014 v014 = await f013014Fn(v013).ConfigureAwait(false);
        T015 v015 = await f014015Fn(v014).ConfigureAwait(false);
        T016 v016 = await f015016Fn(v015).ConfigureAwait(false);
        T017 v017 = await f016017Fn(v016).ConfigureAwait(false);
        T018 v018 = await f017018Fn(v017).ConfigureAwait(false);
        T019 v019 = await f018019Fn(v018).ConfigureAwait(false);
        return await f019020Fn(v019).ConfigureAwait(false);
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
        Func<T001, Task<T002>> f001002Fn,
        Func<T002, Task<T003>> f002003Fn,
        Func<T003, Task<T004>> f003004Fn,
        Func<T004, Task<T005>> f004005Fn,
        Func<T005, Task<T006>> f005006Fn,
        Func<T006, Task<T007>> f006007Fn,
        Func<T007, Task<T008>> f007008Fn,
        Func<T008, Task<T009>> f008009Fn,
        Func<T009, Task<T010>> f009010Fn,
        Func<T010, Task<T011>> f010011Fn,
        Func<T011, Task<T012>> f011012Fn,
        Func<T012, Task<T013>> f012013Fn,
        Func<T013, Task<T014>> f013014Fn,
        Func<T014, Task<T015>> f014015Fn,
        Func<T015, Task<T016>> f015016Fn,
        Func<T016, Task<T017>> f016017Fn,
        Func<T017, Task<T018>> f017018Fn,
        Func<T018, Task<T019>> f018019Fn,
        Func<T019, Task<T020>> f019020Fn,
        Func<T020, Task<T021>> f020021Fn
    )
    {
        T002 v002 = await f001002Fn(v).ConfigureAwait(false);
        T003 v003 = await f002003Fn(v002).ConfigureAwait(false);
        T004 v004 = await f003004Fn(v003).ConfigureAwait(false);
        T005 v005 = await f004005Fn(v004).ConfigureAwait(false);
        T006 v006 = await f005006Fn(v005).ConfigureAwait(false);
        T007 v007 = await f006007Fn(v006).ConfigureAwait(false);
        T008 v008 = await f007008Fn(v007).ConfigureAwait(false);
        T009 v009 = await f008009Fn(v008).ConfigureAwait(false);
        T010 v010 = await f009010Fn(v009).ConfigureAwait(false);
        T011 v011 = await f010011Fn(v010).ConfigureAwait(false);
        T012 v012 = await f011012Fn(v011).ConfigureAwait(false);
        T013 v013 = await f012013Fn(v012).ConfigureAwait(false);
        T014 v014 = await f013014Fn(v013).ConfigureAwait(false);
        T015 v015 = await f014015Fn(v014).ConfigureAwait(false);
        T016 v016 = await f015016Fn(v015).ConfigureAwait(false);
        T017 v017 = await f016017Fn(v016).ConfigureAwait(false);
        T018 v018 = await f017018Fn(v017).ConfigureAwait(false);
        T019 v019 = await f018019Fn(v018).ConfigureAwait(false);
        T020 v020 = await f019020Fn(v019).ConfigureAwait(false);
        return await f020021Fn(v020).ConfigureAwait(false);
    }
}
