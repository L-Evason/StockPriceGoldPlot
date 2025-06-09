namespace AuGrapher.DbReader;

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

using AuGrapher.AuAdjustedStock;

public static class DbReader
{
    public static bool TickerInDb(string ticker)
    {
        using var connection = new SqliteConnection("Data Source=Stocks.db");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Ticker
            FROM StockMonthlyOpen
            WHERE Ticker = $ticker
            LIMIT 1;
        );";

        command.Parameters.AddWithValue("$ticker", ticker);

        return command.ExecuteScalar() is not null;
    }

    public static bool TickerDataUpToDate(string ticker)
    {
        using var connection = new SqliteConnection("Data Source=Stocks.db");
        connection.Open();

        var command = connection.CreateCommand();
        // Fetch the most recent Month and year
        command.CommandText = @"
            SELECT Year, Month
            FROM StockMonthlyOpen
            WHERE Ticker = $ticker
            ORDER BY Year DESC, Month DESC
            LIMIT 1;
        ";

        command.Parameters.AddWithValue("$ticker", ticker);
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            int dbYear = reader.GetInt32(0);
            int dbMonth = reader.GetInt32(1);

            var now = DateTime.Now;

            return dbYear == now.Year && dbMonth == now.Month;
        }
        return false;
    }
    public static DateTime? RecentStockOpen(string ticker)
    {
        using var connection = new SqliteConnection("Data Source=Stocks.db");
        connection.Open();

        var command = connection.CreateCommand();
        // Fetch the most recent Month and year
        command.CommandText = @"
            SELECT Year, Month
            FROM StockMonthlyOpen
            WHERE Ticker = $ticker
            ORDER BY Year DESC, Month DESC
            LIMIT 1;
        ";

        command.Parameters.AddWithValue("$ticker", ticker);
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            int dbYear = reader.GetInt32(0);
            int dbMonth = reader.GetInt32(1);

            return new DateTime(dbYear, dbMonth, 01);
        }
        return null;
    }
    public static DateTime? OldestStockOpen(string ticker)
    {
        using var connection = new SqliteConnection("Data Source=Stocks.db");
        connection.Open();

        var command = connection.CreateCommand();
        // Fetch the most recent Month and year
        command.CommandText = @"
            SELECT Year, Month
            FROM StockMonthlyOpen
            WHERE Ticker = $ticker
            ORDER BY Year ASC, Month ASC
            LIMIT 1;
        ";

        command.Parameters.AddWithValue("$ticker", ticker);
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            int dbYear = reader.GetInt32(0);
            int dbMonth = reader.GetInt32(1);

            return new DateTime(dbYear, dbMonth, 01);
        }
        return null;
    }

    public static DateTime? RecentAuSpotOpen()
    {
        using var connection = new SqliteConnection("Data Source=Stocks.db");
        connection.Open();

        var command = connection.CreateCommand();
        // Fetch the most recent Month and year
        command.CommandText = @"
            SELECT Year, Month
            FROM GoldMonthlySpotPrice
            ORDER BY Year DESC, Month DESC
            LIMIT 1;
        ";
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            int dbYear = reader.GetInt32(0);
            int dbMonth = reader.GetInt32(1);

            return new DateTime(dbYear, dbMonth, 01);
        }
        return null;
    }

    public static List<(DateTime Date, double? AdjustedValue)> GetAdjustedStockHistory(string ticker)
    {
        var results = new List<(DateTime, double?)>();

        DateTime? startDate = OldestStockOpen(ticker);
        DateTime? endDate = RecentAuSpotOpen();

        if (startDate == null || endDate == null || startDate > endDate) // Null check and logic check
            return results; 

        DateTime start = startDate.Value;
        DateTime end = endDate.Value;

        using var connection = new SqliteConnection("Data Source=Stocks.db");
        connection.Open();

        var stockCommand = connection.CreateCommand();
        stockCommand.CommandText = @"
            SELECT Year, Month, OpenPrice 
            FROM StockMonthlyOpen 
            WHERE Ticker = $ticker
            AND (Year > $startYear OR (Year = $startYear AND Month >= $startMonth))
            AND (Year < $endYear OR (Year = $endYear AND Month <= $endMonth));
        ";
        stockCommand.Parameters.AddWithValue("$ticker", ticker);
        stockCommand.Parameters.AddWithValue("$startYear", start.Year);
        stockCommand.Parameters.AddWithValue("$startMonth", start.Month);
        stockCommand.Parameters.AddWithValue("$endYear", end.Year);
        stockCommand.Parameters.AddWithValue("$endMonth", end.Month);

        using var reader = stockCommand.ExecuteReader();
        var stockData = new List<(int Year, int Month, double OpenPrice)>();
        while (reader.Read())
        {
            int year = reader.GetInt32(0);
            int month = reader.GetInt32(1);
            double openPrice = reader.GetDouble(2);
            stockData.Add((year, month, openPrice));
        }

        var goldCommand = connection.CreateCommand();
        goldCommand.CommandText = @"
            SELECT Year, Month, SpotPrice
            FROM GoldMonthlySpotPrice
            WHERE (Year > $startYear OR (Year = $startYear AND Month >= $startMonth))
            AND (Year < $endYear OR (Year = $endYear AND Month <= $endMonth));
        ";
        goldCommand.Parameters.AddWithValue("$startYear", start.Year);
        goldCommand.Parameters.AddWithValue("$startMonth", start.Month);
        goldCommand.Parameters.AddWithValue("$endYear", end.Year);
        goldCommand.Parameters.AddWithValue("$endMonth", end.Month);

        var goldPrices = new Dictionary<(int Year, int Month), double>();
        using var goldReader = goldCommand.ExecuteReader();
        while (goldReader.Read())
        {
            int year = goldReader.GetInt32(0);
            int month = goldReader.GetInt32(1);
            double spotPrice = goldReader.GetDouble(2);
            goldPrices[(year, month)] = spotPrice;
        }

        foreach (var (year, month, openPrice) in stockData)
        {
            goldPrices.TryGetValue((year, month), out double goldPrice);
            double? adjusted = AuAdjustedStock.AdjustedStock(openPrice, goldPrice);
            var date = new DateTime(year, month, 1);
            results.Add((date, adjusted));
        }

        return results;
    }
}