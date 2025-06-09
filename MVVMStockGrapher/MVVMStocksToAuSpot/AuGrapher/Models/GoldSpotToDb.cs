namespace AuGrapher.GoldSpotToDb;

using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

public static class GoldSpotToDb
{
    /*
    public static bool GoldSpotDataExists(int year, int month)
    {
        using (var connection = new SqliteConnection("Data Source=Stocks.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT EXISTS(
                    SELECT 1 FROM GoldMonthlySpotPrice WHERE Year = $year AND Month = $month
                );";
            command.Parameters.AddWithValue("$year", year);
            command.Parameters.AddWithValue("$month", month);
            var result = command.ExecuteScalar();
            return Convert.ToInt64(result) == 1;
        }
    } */ // Not needed with Insert or ignore
    public static void AuCsvDataInsert()
    {
        var lines = File.ReadAllLines("gold_prices.csv")
            .Skip(1) // Skip the header
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        using var connection = new SqliteConnection("Data Source=Stocks.db");
        connection.Open();

        using var transaction = connection.BeginTransaction();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR IGNORE INTO GoldMonthlySpotPrice (Year, Month, SpotPrice)
            VALUES ($year, $month, $spotPrice);";

        var yearParam = command.CreateParameter();
        yearParam.ParameterName = "$year";
        command.Parameters.Add(yearParam);

        var monthParam = command.CreateParameter();
        monthParam.ParameterName = "$month";
        command.Parameters.Add(monthParam);

        var priceParam = command.CreateParameter();
        priceParam.ParameterName = "$spotPrice";
        command.Parameters.Add(priceParam);

        char[] delimiters = ['-', ','];

        foreach (string line in lines)
        {
            string[] parts = line.Split(delimiters);

            yearParam.Value = Convert.ToInt32(parts[0]);
            monthParam.Value = Convert.ToInt32(parts[1]);
            priceParam.Value = Convert.ToDouble(parts[2]);

            command.ExecuteNonQuery();
        }

        transaction.Commit();
    } 
}