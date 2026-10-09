namespace FnUtil.Tests;

public class ActionExtTests
{
    // ToFunc (Action → Func<Unit>)

    [Fact]
    public void ToFunc_Action_WhenCalled_InvokesTheAction()
    {
        var called = false;
        Action action = () => called = true;
        var func = action.ToFunc();
        func();
        Assert.True(called);
    }

    [Fact]
    public void ToFunc_Action_ReturnsUnit()
    {
        Action action = () => { };
        var func = action.ToFunc();
        var result = func();
        Assert.Equal(unit, result);
    }

    // ToFunc<T1> (Action<T1> → Func<T1, Unit>)

    [Fact]
    public void ToFunc_ActionT1_WhenCalled_InvokesTheActionWithArgument()
    {
        int captured = 0;
        Action<int> action = (x) => captured = x;
        var func = action.ToFunc();
        func(42);
        Assert.Equal(42, captured);
    }

    [Fact]
    public void ToFunc_ActionT1_ReturnsUnit()
    {
        Action<int> action = (_) => { };
        var func = action.ToFunc();
        var result = func(1);
        Assert.Equal(unit, result);
    }

    // ToFunc<T1, T2> (Action<T1, T2> → Func<T1, T2, Unit>)

    [Fact]
    public void ToFunc_ActionT1T2_WhenCalled_InvokesTheActionWithBothArguments()
    {
        int capturedA = 0;
        string capturedB = "";
        Action<int, string> action = (a, b) => { capturedA = a; capturedB = b; };
        var func = action.ToFunc();
        func(7, "hello");
        Assert.Equal(7, capturedA);
        Assert.Equal("hello", capturedB);
    }

    [Fact]
    public void ToFunc_ActionT1T2_ReturnsUnit()
    {
        Action<int, string> action = (_, _) => { };
        var func = action.ToFunc();
        var result = func(1, "x");
        Assert.Equal(unit, result);
    }
}


public class FEffectTests
{
    // Effect

    [Fact]
    public void Effect_InvokesTheAction()
    {
        var called = false;
        Effect(() => called = true);
        Assert.True(called);
    }

    [Fact]
    public void Effect_ReturnsUnit()
    {
        var result = Effect(() => { });
        Assert.Equal(unit, result);
    }
}
