namespace FnUtil.Tests;


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
