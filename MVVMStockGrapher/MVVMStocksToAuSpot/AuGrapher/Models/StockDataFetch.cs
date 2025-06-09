namespace AuGrapher.StockDataFetch;

using System;
using System.Diagnostics;

public static class RunPythonyFinnanceScript
{
    public static void FetchHistoricalData(string ticker)
    {
        string pythonExe = "Python"; // Ensure your python.exe file is set as an Environment variable
        string yFinanceScript = "yFinanceFunc.py";
        
        string arguments = $"{yFinanceScript} {ticker}";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonExe,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        Console.WriteLine($"Executing: {pythonExe} {arguments}");
        using (Process process = Process.Start(startInfo))
        {
            using (var reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
            }

            using (var reader = process.StandardError)
            {
                string error = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Error: {error}");
                }
            }

            process.WaitForExit();
        }
    }
}