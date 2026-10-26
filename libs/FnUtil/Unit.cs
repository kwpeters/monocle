namespace FnUtil;


public sealed class Unit
{
    //------------------------------------------------------------------------------
    // Static properties
    //------------------------------------------------------------------------------

    // The one-and-only instance.  A static initializer runs exactly once, so
    // every caller gets the same instance even when first accessed concurrently.
    public static Unit Instance { get; } = new();

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private Unit()
    {
    }
}


public static partial class F
{
    //------------------------------------------------------------------------------
    // Static properties
    //------------------------------------------------------------------------------

#pragma warning disable IDE1006 // Naming rule violation: lowercase 'unit' follows functional programming convention
    public static Unit unit => Unit.Instance;
#pragma warning restore IDE1006
}
