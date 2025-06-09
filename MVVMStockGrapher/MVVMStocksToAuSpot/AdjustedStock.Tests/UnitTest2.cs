using Xunit;
using System;
using AuGrapher.AuAdjustedStock;

namespace AdjustedStock.Tests;

public class DateFilterTest
{
    public static IEnumerable<object[]> TestData =>
        new List<object[]>
        {
            new object[] { 100m, 2m, 50m },
            new object[] { 150m, 3m, 50m },
            new object[] { null, 2m, null },
            new object[] { 100m, null, null },
            new object[] { 100m, 0m, null },
            new object[] { null, null, null }
        };

    [Theory]
    [MemberData(nameof(TestData))]
    public void FilteredDates_ReturnsExpectedResult(decimal? stockPrice, decimal? goldSpot, decimal? expected)
    {
        return NotImplementedException; 
    }
}
