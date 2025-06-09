namespace AuGrapher.AuAdjustedStock;

public static class AuAdjustedStock
{
    public static double? AdjustedStock(double? stockPrice, double? goldOuncePrice)
    {
        if (stockPrice is null || goldOuncePrice is null || goldOuncePrice == 0)
        {
            // return null if there is no data from either for the given month or the Au value is 0
            return null;
        }
        else
        {
            return stockPrice / goldOuncePrice;
        }
    }
}