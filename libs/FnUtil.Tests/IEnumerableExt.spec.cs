namespace FnUtil.Tests;


[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Don't need to optimize unit tests. Clarity more important.")]
public class IEnumerableExtTests
{
    [Fact]
    public void Exclude_WithSingleFilter_ExcludesMatchingElements()
    {
        var numbers = new int[] { 1, 2, 3, 4, 5, 6 };
        var result = numbers.Exclude(x => x % 2 == 0);
        Assert.Equal(new int[] { 1, 3, 5 }, result);
    }


    [Fact]
    public void Exclude_WithSingleFilter_WhenNoMatches_ReturnsAllElements()
    {
        var numbers = new int[] { 1, 3, 5 };
        var result = numbers.Exclude(x => x % 2 == 0);
        Assert.Equal(new int[] { 1, 3, 5 }, result);
    }


    [Fact]
    public void Exclude_WithSingleFilter_WhenAllMatch_ReturnsEmpty()
    {
        var numbers = new int[] { 2, 4, 6 };
        var result = numbers.Exclude(x => x % 2 == 0);
        Assert.Empty(result);
    }


    [Fact]
    public void Exclude_WithSingleFilter_WhenEmptyCollection_ReturnsEmpty()
    {
        var numbers = Array.Empty<int>();
        var result = numbers.Exclude(x => x % 2 == 0);
        Assert.Empty(result);
    }


    [Fact]
    public void Exclude_WithMultipleFilters_ExcludesElementsMatchingAnyFilter()
    {
        var numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var filters = new Func<int, bool>[]
        {
            x => x % 2 == 0,  // Even numbers
            x => x % 3 == 0   // Multiples of 3
        };
        var result = numbers.Exclude(filters);
        // Excludes: 2, 3, 4, 6, 8, 9, 10
        // Keeps: 1, 5, 7
        Assert.Equal(new int[] { 1, 5, 7 }, result);
    }


    [Fact]
    public void Exclude_WithMultipleFilters_WhenNoFilters_ReturnsAllElements()
    {
        var numbers = new int[] { 1, 2, 3, 4, 5 };
        var filters = Array.Empty<Func<int, bool>>();
        var result = numbers.Exclude(filters);
        Assert.Equal(new int[] { 1, 2, 3, 4, 5 }, result);
    }


    [Fact]
    public void Exclude_WithMultipleFilters_WhenNoMatches_ReturnsAllElements()
    {
        var numbers = new int[] { 1, 3, 5 };
        var filters = new Func<int, bool>[]
        {
            x => x % 2 == 0,  // Even numbers
            x => x > 10       // Greater than 10
        };
        var result = numbers.Exclude(filters);
        Assert.Equal(new int[] { 1, 3, 5 }, result);
    }


    [Fact]
    public void Exclude_WithMultipleFilters_WhenAllMatch_ReturnsEmpty()
    {
        var numbers = new int[] { 2, 4, 6 };
        var filters = new Func<int, bool>[]
        {
            x => x % 2 == 0  // Even numbers
        };
        var result = numbers.Exclude(filters);
        Assert.Empty(result);
    }


    [Fact]
    public void Reduce_WhenCalled_ReturnsTheExpectedResults()
    {
        var res = new int[] { 1, 2, 3 }.Reduce(
            (acc, val, idx, col) => acc + val,
            0
        );
        Assert.Equal(6, res);
    }


    [Fact]
    public void Tap_CallsActionForEachElement()
    {
        var numbers = new int[] { 1, 2, 3 };
        var seen = new List<int>();
        numbers.Tap(seen.Add);
        Assert.Equal(new int[] { 1, 2, 3 }, seen);
    }


    [Fact]
    public void Tap_ReturnsOriginalCollection()
    {
        var numbers = new int[] { 1, 2, 3 };
        var result = numbers.Tap((_) => { });
        Assert.Same(numbers, result);
    }


    [Fact]
    public void Tap_WhenEmptyCollection_DoesNotCallAction()
    {
        var called = false;
        Array.Empty<int>().Tap((_) => called = true);
        Assert.False(called);
    }

}
