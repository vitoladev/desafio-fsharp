module backend.Main

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Giraffe
open backend.App

let errorHandler (ex : Exception) (logger : ILogger) =
    let errorMessage = "An unhandled exception has occurred while executing the request."
    logger.LogError(ex, errorMessage)
    clearResponse >=> setStatusCode 500 >=> text errorMessage

let builder = Environment.GetCommandLineArgs() |> WebApplication.CreateBuilder
builder.Services.AddGiraffe() |> ignore


let app = builder.Build()

app.UseGiraffeErrorHandler(errorHandler) |> ignore
app.UseGiraffe webApp
app.Run()