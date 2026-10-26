using System.Text.Json.Serialization;

// CA1716: Option, None, and Some conflict with Visual Basic keywords, but these are
// well-established functional programming type names. The benefit of using standard
// FP nomenclature outweighs the unlikely scenario of VB.NET consumers.
#pragma warning disable CA1716

namespace FnUtil;


/// <summary>
/// Represents an optional value.  It may be a "some" Option and contain a value
/// or it may be a "none" option.
/// </summary>
/// <typeparam name="T">The type of the value when this Option is a Some
/// Option</typeparam>
[JsonConverter(typeof(OptionJsonConverterFactory))]
public abstract record class Option<T>
{
    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

#pragma warning disable CA2225      // Explicit conversion method not needed.
    public static implicit operator Option<T>(None _) => new None<T>();
#pragma warning restore CA2225

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    // Only None<T> and Some<T>, which are declared in this assembly, may derive
    // from Option<T>.  The rest of FnUtil relies on those being the only cases.
    private protected Option() { }

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    public abstract bool IsSome { get; }


    public bool IsNone => !IsSome;
}


/// <summary>
/// Concrete None Option type.
///
/// This type must have a generic type parameter so that it can derive from
/// Option<T>, but this (obviously) is of no use since this is a None Option
/// and will never use type T.  To overcome this deficiency in the C#
/// compiler, None is used as a marker type.
///
/// This type is not public, because clients should use F.none instead.
/// </summary>
/// <typeparam name="T">The type of the value when this Option is a Some
/// Option</typeparam>
internal sealed record class None<T> : Option<T>
{
    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    public override bool IsSome => false;
}


/// <summary>
/// Concrete Some Option type.
///
/// This type is not public, because clients should use F.Some() instead.
/// </summary>
/// <typeparam name="T">The type of the value when this Option is a Some
/// Option</typeparam>
internal sealed record class Some<T> : Option<T>
{
    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    /// <param name="value">The value to wrap.  Must not be null.</param>
    public Some(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(
                nameof(value),
                "Cannot wrap a null value in Some; use none instead.");
        }

        Value = value;
    }

    //------------------------------------------------------------------------------
    // Properties
    //------------------------------------------------------------------------------

    /// <summary>
    /// The wrapped value
    /// </summary>
    public T Value { get; }


    public override bool IsSome => true;
}


////////////////////////////////////////////////////////////////////////////////
// Helper Functions
////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Convenience factory methods and properties.
///
/// To make usage easy, add the following:
/// <code>
/// using static FnUtil.F;
/// </code>
/// </summary>
public static partial class F
{
    //------------------------------------------------------------------------------
    // Static properties
    //------------------------------------------------------------------------------

#pragma warning disable IDE1006 // Lowercase 'none' follows functional programming convention
    public static None none => None.Instance;
#pragma warning restore IDE1006

    //------------------------------------------------------------------------------
    // Static methods
    //------------------------------------------------------------------------------

    public static Option<T>
    Some<T>(T value) => new Some<T>(value);


    // Provides a way of creating a None Option by explicitly defining the
    // Some value type.  This is useful where the type cannot be inferred from the
    // context, such as Task.FromResult(NoneE<int>()).  The appended "E" stands
    // for "explicit."
    public static Option<T>
    NoneE<T>() => new None<T>();
}


////////////////////////////////////////////////////////////////////////////////
// Support
////////////////////////////////////////////////////////////////////////////////


// "Marker type" representing a none Option.  This type uses none of the generic
// type parameters specified in Option.  This type only exists so that Option
// can have an implicit conversion operator where all unused generic type
// parameters can be inferred from the context.
public sealed class None
{
    //------------------------------------------------------------------------------
    // Static properties
    //------------------------------------------------------------------------------

    // The one-and-only instance.  A static initializer runs exactly once, so
    // every caller gets the same instance even when first accessed concurrently.
    public static None Instance { get; } = new();

    //------------------------------------------------------------------------------
    // Constructors
    //------------------------------------------------------------------------------

    private None() { }
}
