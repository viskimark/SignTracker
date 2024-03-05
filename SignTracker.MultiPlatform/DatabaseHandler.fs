namespace SignTracker.MultiPlatform

open System
open System.Net.Http
open System.Text.Json
open System.Threading

module DatabaseHandler =
    let httpClient = new HttpClient()
    let baseUrl = "http://images-entities.gl.at.ply.gg:25998/"
    let getFromJson<'T> url =
        async {
            //httpClient.Timeout <- TimeSpan.FromSeconds 2
            let! result = baseUrl + url |> httpClient.GetStringAsync |> Async.AwaitTask
            if result = "" then failwith "Empty string"
            return JsonSerializer.Deserialize<'T> result
        }

    let sendAsJson<'T> (item: 'T) url =
        async {
            let json = JsonSerializer.Serialize<'T> item 
            let content = new StringContent(json)
            httpClient.PostAsync(baseUrl + url, content) |> Async.AwaitTask |> ignore
        }

    let parseDateTime (dt: DateTime option) : DateOnly option =
        match dt with
        | Some dt -> dt |> DateOnly.FromDateTime |> Some
        | None -> None
    let parseDateOnly (dt: DateOnly option) : DateTime option =
        match dt with
        | Some dt -> dt.ToDateTime TimeOnly.MinValue |> Some
        | None -> None

    let rec getSigns userId =
        async {
            try
                return! getFromJson<Database.Sign seq> $"signs/{userId}"
            with _ ->
                try
                    return LocalDatabase.getSigns userId
                with _ ->
                    return Seq.empty
        }

    let addSign (sign: Database.Sign) (userId: int) =
        async {
            //do! $"submitSign/{userId}" |> sendAsJson<Database.Sign> sign
            do LocalDatabase.addSign sign userId
        }

    let editSign (sign: Database.Sign) =
        async {
            do! $"editSign/0" |> sendAsJson<Database.Sign> sign
        }