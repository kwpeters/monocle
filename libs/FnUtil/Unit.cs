namespace FnUtil;


// using Unit = System.ValueTuple;


public sealed class Unit
{
    //------------------------------------------------------------------------------
    // Static fields
    //------------------------------------------------------------------------------

    // The one-and-only instance.
    private static
    Unit? _instance;

    //------------------------------------------------------------------------------
    // Static properties
    //------------------------------------------------------------------------------

    public static Unit Instance
    {
        get {
            _instance ??= new Unit();
            return _instance;
        }
    }

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private Unit()
    {
    }
}


public partial class F
{
    //------------------------------------------------------------------------------
    // Static properties
    //------------------------------------------------------------------------------

#pragma warning disable IDE1006 // Naming rule violation: lowercase 'unit' follows functional programming convention
    public static Unit unit => Unit.Instance;
#pragma warning restore IDE1006
}
