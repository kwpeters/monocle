//
// At least a portion of the code below was created using AI tool GitHub Copilot.
//

using System.Globalization;

namespace FnUtil.Tests;

public class PipeAsyncTests
{
    [Fact]
    public async Task PipeAsync_With2Functions_ReturnsExpectedResult()
    {
        Task<int> ParseAsync(string str) => Task.FromResult(Convert.ToInt32(str, CultureInfo.InvariantCulture));
        Task<int> Times5Async(int x) => Task.FromResult(x * 5);

        var res = await PipeAsync<string, int, int>(
            "5",
            ParseAsync,
            Times5Async
        );

        Assert.Equal(25, res);
    }


    [Fact]
    public async Task PipeAsync_With3Functions_ReturnsExpectedResult()
    {
        Task<int> ParseAsync(string str) => Task.FromResult(Convert.ToInt32(str, CultureInfo.InvariantCulture));
        Task<int> Times5Async(int x) => Task.FromResult(x * 5);
        Task<int> Add3Async(int x) => Task.FromResult(x + 3);

        var res = await PipeAsync<string, int, int, int>(
            "5",
            ParseAsync,
            Times5Async,
            Add3Async
        );

        Assert.Equal(28, res);
    }


    [Fact]
    public async Task PipeAsync_With4Functions_ReturnsExpectedResult()
    {
        Task<int> ParseAsync(string str) => Task.FromResult(Convert.ToInt32(str, CultureInfo.InvariantCulture));
        Task<int> Times5Async(int x) => Task.FromResult(x * 5);
        Task<int> Add3Async(int x) => Task.FromResult(x + 3);
        Task<string> ToStringAsync(int x) => Task.FromResult(x.ToString(CultureInfo.InvariantCulture));

        var res = await PipeAsync<string, int, int, int, string>(
            "5",
            ParseAsync,
            Times5Async,
            Add3Async,
            ToStringAsync
        );

        Assert.Equal("28", res);
    }


    [Fact]
    public async Task PipeAsync_With5Functions_ReturnsExpectedResult()
    {
        Task<int> ParseAsync(string str) => Task.FromResult(Convert.ToInt32(str, CultureInfo.InvariantCulture));
        Task<int> Times5Async(int x) => Task.FromResult(x * 5);
        Task<int> Add3Async(int x) => Task.FromResult(x + 3);
        Task<string> ToStringAsync(int x) => Task.FromResult(x.ToString(CultureInfo.InvariantCulture));
        Task<string> AppendBangAsync(string s) => Task.FromResult(s + "!");

        var res = await PipeAsync(
            "5",
            ParseAsync,
            Times5Async,
            Add3Async,
            ToStringAsync,
            AppendBangAsync
        );

        Assert.Equal("28!", res);
    }


    [Fact]
    public async Task PipeAsync_WithActualAsyncOperations_WorksCorrectly()
    {
        // Simulating actual async operations with delays
        async Task<int> ParseAsync(string str)
        {
            await Task.Delay(10);
            return Convert.ToInt32(str, CultureInfo.InvariantCulture);
        }

        async Task<int> Times5Async(int x)
        {
            await Task.Delay(10);
            return x * 5;
        }

        async Task<int> Add3Async(int x)
        {
            await Task.Delay(10);
            return x + 3;
        }

        async Task<string> ToStringAsync(int x)
        {
            await Task.Delay(10);
            return x.ToString(CultureInfo.InvariantCulture);
        }

        var res = await PipeAsync(
            "7",
            ParseAsync,
            Times5Async,
            Add3Async,
            ToStringAsync
        );

        Assert.Equal("38", res);
    }


    [Fact]
    public async Task PipeAsync_With9Functions_ReturnsExpectedResult()
    {
        Task<int> F1(int x) => Task.FromResult(x + 1);
        Task<int> F2(int x) => Task.FromResult(x * 2);
        Task<int> F3(int x) => Task.FromResult(x - 1);
        Task<int> F4(int x) => Task.FromResult(x + 10);
        Task<int> F5(int x) => Task.FromResult(x / 2);
        Task<int> F6(int x) => Task.FromResult(x + 5);
        Task<int> F7(int x) => Task.FromResult(x * 3);
        Task<int> F8(int x) => Task.FromResult(x - 20);
        Task<string> F9(int x) => Task.FromResult(x.ToString(CultureInfo.InvariantCulture));

        var res = await PipeAsync(
            5,
            F1,
            F2,
            F3,
            F4,
            F5,
            F6,
            F7,
            F8,
            F9
        );

        // 5 + 1 = 6
        // 6 * 2 = 12
        // 12 - 1 = 11
        // 11 + 10 = 21
        // 21 / 2 = 10
        // 10 + 5 = 15
        // 15 * 3 = 45
        // 45 - 20 = 25
        // 25.ToString() = "25"
        Assert.Equal("25", res);
    }


    [Fact]
    public async Task PipeAsync_WithDifferentTypes_HandlesTypeTransitions()
    {
        Task<double> IntToDoubleAsync(int x) => Task.FromResult((double)x);
        Task<string> DoubleToStringAsync(double d) => Task.FromResult($"{d:F2}");
        Task<int> StringToLengthAsync(string s) => Task.FromResult(s.Length);

        var res = await PipeAsync<int, double, string, int>(
            42,
            IntToDoubleAsync,
            DoubleToStringAsync,
            StringToLengthAsync
        );

        Assert.Equal(5, res); // "42.00" has 5 characters
    }
}
