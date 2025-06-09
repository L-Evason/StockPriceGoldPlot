namespace AuGrapher.StockPriceToDb;

using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using StockDataFetch;

public static class StockPriceToDb
{
    public static void InsertCsvDataToDb(string ticker)
    {
        // Generate CSV file
        RunPythonyFinnanceScript.FetchHistoricalData(ticker);

        string csvFilePath = Path.Combine($"{ticker}_monthly_open.csv");



        var lines = File.ReadAllLines(csvFilePath)
            .Skip(1) // Skip the header
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
        if (!lines.Any())
        {
            // Handle empty csv or improper ticker
        }
        else
        {
            using var connection = new SqliteConnection("Data Source=Stocks.db");
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO StockMonthlyOpen (Ticker, Year, Month, OpenPrice)
                VALUES ($ticker, $year, $month, $openPrice);";

            var tickerParam = command.CreateParameter();
            tickerParam.ParameterName = "$ticker";
            command.Parameters.Add(tickerParam);

            var yearParam = command.CreateParameter();
            yearParam.ParameterName = "$year";
            command.Parameters.Add(yearParam);

            var monthParam = command.CreateParameter();
            monthParam.ParameterName = "$month";
            command.Parameters.Add(monthParam);

            var priceParam = command.CreateParameter();
            priceParam.ParameterName = "$openPrice";
            command.Parameters.Add(priceParam);

            char[] delimiters = ['-', ','];

            foreach (string line in lines)
            {
                string[] parts = line.Split(delimiters);

                tickerParam.Value = ticker;
                yearParam.Value = Convert.ToInt32(parts[0]);
                monthParam.Value = Convert.ToInt32(parts[1]);
                priceParam.Value = Convert.ToDouble(parts[3]);

                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        // Delete the generated csv
        File.Delete($"{ticker}_monthly_open.csv");
    }
}