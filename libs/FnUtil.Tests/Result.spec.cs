namespace FnUtil.Tests;


public class ResultSuccessTypeTests
{
    [Fact]
    public void Value_StoresProvidedValue()
    {
        var s = new ResultSuccessType<int>(42);
        Assert.Equal(42, s.Value);
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        var a = new ResultSuccessType<int>(42);
        var b = new ResultSuccessType<int>(42);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var a = new ResultSuccessType<int>(1);
        var b = new ResultSuccessType<int>(2);
        Assert.NotEqual(a, b);
    }
}


public class ResultErrorTypeTests
{
    [Fact]
    public void Error_StoresProvidedError()
    {
        var e = new ResultErrorType<string>("oops");
        Assert.Equal("oops", e.Error);
    }

    [Fact]
    public void Equality_SameError_AreEqual()
    {
        var a = new ResultErrorType<string>("oops");
        var b = new ResultErrorType<string>("oops");
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentErrors_AreNotEqual()
    {
        var a = new ResultErrorType<string>("one");
        var b = new ResultErrorType<string>("two");
        Assert.NotEqual(a, b);
    }
}


public class SuccessResultTests
{

    [Fact]
    public void CanBeEasilyCreated()
    {
        _ = Success(5);
    }
}


public class ErrorResultTests
{

    [Fact]
    public void CanBeEasilyCreated()
    {
        _ = Error("Error message.");
    }
}


public class ResultTests
{

    [Fact]
    public void IsSuccessful_WhenWrappingASuccessResult_ReturnsTrue()
    {
        Result<int, string> res = Success(5);
        Assert.True(res.IsSuccess);
    }


    [Fact]
    public void IsSuccessful_WhenWrappingAnErrorResult_ReturnsFalse()
    {
        Result<int, string> res = Error("Error message");
        Assert.False(res.IsSuccess);
    }


    [Fact]
    public void IsError_WhenWrappingASuccessResult_ReturnsFalse()
    {
        Result<int, string> res = Success(5);
        Assert.False(res.IsError);
    }


    [Fact]
    public void IsError_WhenWrappingAnErrorResult_ReturnsTrue()
    {
        Result<int, string> res = Error("Error message");
        Assert.True(res.IsError);
    }
}
