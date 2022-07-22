module backend.Helpers
open Newtonsoft.Json
open Thoth.Json.Net
open Newtonsoft.Json.Linq
open Microsoft.AspNetCore.Http
open System.Text
open Giraffe
open System

let waitTask t = t |> Async.AwaitTask |> Async.RunSynchronously

let connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=postgres"

type ThothSerializer (?caseStrategy : CaseStrategy, ?extra : ExtraCoders, ?skipNullField : bool) =
    static let Utf8EncodingWithoutBom = new UTF8Encoding(false)
    static let DefaultBufferSize = 1024

    /// Responds a JSON
    static member RespondRawJson (body: JToken) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                ctx.SetContentType "application/json; charset=utf-8"
                let stream = new System.IO.StreamWriter(ctx.Response.Body, Utf8EncodingWithoutBom, DefaultBufferSize, true)
                let jsonWriter = new JsonTextWriter(stream)
                do! body.WriteToAsync(jsonWriter)
                do! jsonWriter.FlushAsync()
                return Some ctx
            }

    /// Responds a JSON
    static member RespondJson (body: 'T) (encoder: Encoder<'T>) =
        encoder body |> ThothSerializer.RespondRawJson

    /// Responds a JSON array by writing items
    /// into response stream one by one
    static member RespondRawJsonSeq (items: JToken seq) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                ctx.SetStatusCode 200
                ctx.SetContentType "application/json; charset=utf-8"
                let stream =
                    new System.IO.StreamWriter(ctx.Response.Body, Utf8EncodingWithoutBom, DefaultBufferSize, true)
                let jsonWriter = new JsonTextWriter(stream)
                jsonWriter.WriteStartArray()
                for item in items do
                    do! item.WriteToAsync(jsonWriter)
                jsonWriter.WriteEndArray()
                do! jsonWriter.FlushAsync()
                return Some ctx
            }

    /// Responds a JSON array by serializing items
    /// into response stream one by one
    static member RespondJsonSeq (items: 'T seq) (encoder: Encoder<'T>) =
        items |> Seq.map encoder |> ThothSerializer.RespondRawJsonSeq

    static member ReadBodyRaw (ctx: HttpContext) =
        task {
            try
                use stream = new System.IO.StreamReader(ctx.Request.Body, Utf8EncodingWithoutBom, true, DefaultBufferSize, true)
                use jsonReader = new JsonTextReader(stream)
                let! json = JValue.ReadFromAsync jsonReader
                return Ok json
            with
                | :? Newtonsoft.Json.JsonReaderException as ex ->
                    return Error("Given an invalid JSON: " + ex.Message)
        }

    static member ReadBody (ctx: HttpContext) (decoder: Decoder<'T>) =
        task {
            match! ThothSerializer.ReadBodyRaw ctx with
            | Ok json -> return Decode.fromValue "$" decoder json
            | Error e -> return Error e
        }
