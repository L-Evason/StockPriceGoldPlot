using Microsoft.Data.Sqlite;

namespace AuGrapher.StockDataContext;

public static class TableGeneration
{   
    public static void GenerateGoldTable()
    {
        string connectionString = "Data Source=Stocks.db";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string tableQuery = @"
            CREATE TABLE IF NOT EXISTS GoldMonthlySpotPrice (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Year INTEGER NOT NULL,
                Month INTEGER NOT NULL,
                SpotPrice REAL NOT NULL,
                UNIQUE(Year, Month)
            );";

            using var command = new SqliteCommand(tableQuery, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public static void GenerateStockTable()
    {
        string connectionString = "Data Source=Stocks.db";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string tableQuery = @"
            CREATE TABLE IF NOT EXISTS StockMonthlyOpen (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Ticker TEXT NOT NULL,
                Year INTEGER NOT NULL,
                Month INTEGER NOT NULL,
                OpenPrice REAL NOT NULL,
                UNIQUE(Ticker, Year, Month)
            );";

            using var command = new SqliteCommand(tableQuery, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}