using Xunit;
using System;
using AuGrapher.AuAdjustedStock;

namespace AdjustedStock.Tests;

public class AuPriceAdjustTest
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
    public void AdjustedStock_ReturnsExpectedResult(decimal? stockPrice, decimal? goldSpot, decimal? expected)
    {
        var result = AuAdjustedStock.AdjustedStock(stockPrice, goldSpot);
        Assert.Equal(expected, result);
    }
}
