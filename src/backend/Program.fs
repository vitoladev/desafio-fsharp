module backend.Main

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Giraffe
open backend.App
open Thoth.Json.Giraffe

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let builder = Environment.GetCommandLineArgs() |> WebApplication.CreateBuilder
builder.Services.AddGiraffe() |> ignore

let app = builder.Build()

app.UseGiraffeErrorHandler(errorHandler) |> ignore
app.UseGiraffe webApp
app.Run()