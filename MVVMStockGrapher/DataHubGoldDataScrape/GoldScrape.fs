namespace DataHubGoldDataScrape

module DataHubGoldDataScrape =

    open System
    open System.IO
    open System.Net.Http
    open System.Threading.Tasks

    let downloadFileAsync (url: string) (filePath: string) =
        async {
            use client = new HttpClient()
            try
                let! response = client.GetAsync(url) |> Async.AwaitTask
                response.EnsureSuccessStatusCode() |> ignore

                let! content = response.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
                File.WriteAllBytes(filePath, content)

                printfn "Download successful!"
            with ex ->
                printfn "Download failed: %s" ex.Message
        }

    let run () = 
        downloadFileAsync "https://datahub.io/core/gold-prices/r/monthly.csv" "gold_prices.csv"
        |> Async.RunSynchronously
