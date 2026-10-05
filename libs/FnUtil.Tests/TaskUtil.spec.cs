namespace FnUtil.Tests;

public class TasksTests
{

    [Fact]
    public async Task All_WhenAllTasksSucceed_TheExpectedTupleIsReturned()
    {
        async Task<bool> Task1()
        {
            await Task.Delay(5);
            return true;
        }

        async Task<string> Task2()
        {
            await Task.Delay(10);
            return "Anders would be ashamed.";
        }

        var (boolVal, strVal) = await TaskUtil.All(Task1(), Task2());
        Assert.True(boolVal);
        Assert.Equal("Anders would be ashamed.", strVal);
    }


    [Fact]
    public async Task All_WhenATaskFaults_TheReturnedTaskFaults()
    {
        static async Task<bool> task1()
        {
            await Task.Delay(5);
            return true;
        }

        static async Task<string> task2()
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Error message");
        }

        try
        {
            var (boolVal, strVal) = await TaskUtil.All(task1(), task2());
            Assert.Fail("Should never get here.");
        }
        catch (System.InvalidOperationException e)
        {
            Assert.Equal("Error message", e.Message);
        }
    }

}
