using System.Globalization;

namespace FnUtil.Tests;

public class PipeTests
{
    [Fact]
    public void Pipe_WhenCalled_ReturnsTheExpectedResult()
    {
        int Parse(string str) => Convert.ToInt32(str, CultureInfo.InvariantCulture);
        int Times5(int x) => x * 5;
        int Add3(int x) => x + 3;
        string ToString(int x) => x.ToString(CultureInfo.InvariantCulture);

        var res = Pipe(
            "5",
            Parse,
            Times5,
            Add3,
            ToString
        );
        Assert.Equal("28", res);
    }
}
