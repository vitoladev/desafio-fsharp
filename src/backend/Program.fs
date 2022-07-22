module backend.Main

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Giraffe
open backend.App
open Microsoft.Extensions.DependencyInjection


let errorHandler (ex: Exception) (logger: ILogger) =
    let errorMessage =
        "An unhandled exception has occurred while executing the request."

    logger.LogError(ex, errorMessage)

    clearResponse
    >=> setStatusCode 500
    >=> text errorMessage

let configureServices (services: IServiceCollection) = services.AddGiraffe() |> ignore

let builder =
    Environment.GetCommandLineArgs()
    |> WebApplication.CreateBuilder

let configureApp (app: IApplicationBuilder) =
    app
        .UseGiraffeErrorHandler(errorHandler)
        .UseGiraffe(webApp)

configureServices (builder.Services)

let app = builder.Build()

configureApp (app)
app.Run()

type Program() =
    class
    end
