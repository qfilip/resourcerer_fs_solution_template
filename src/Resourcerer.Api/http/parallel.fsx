open System.Net.Http
open System
open System.Text.Json

let uri =  Uri("http://localhost:5000/v1.0/foos")
let client = new HttpClient()
let options = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

let mapAndSend (text: string) = async {
  use message = new HttpRequestMessage(HttpMethod.Patch, uri)
  let data = {| Id = "9064ef80-c5a1-429b-9923-53834bf70e8e"; Text = text |}
  let content = JsonSerializer.Serialize(data, options)
  message.Content <- new StringContent(content, Text.Encoding.UTF8, "application/json")

  return! client.SendAsync(message) |> Async.AwaitTask
}

[1..10]
|> List.map (sprintf "Text %d")
|> List.map mapAndSend
|> Async.Parallel
|> Async.RunSynchronously
|> Array.iter (fun response -> printfn "%A" response.StatusCode)