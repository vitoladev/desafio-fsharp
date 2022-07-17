module backend.App

open Giraffe
open Microsoft.AspNetCore.Http
open backend.Barbecue.Handlers
open backend.HttpHandlers

let webApp: HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        subRoute "/api"
            (choose [
                subRoute "/barbecue" (choose [
                    GET >=> route "/" >=> handleListBarbecues
                    GET >=> routef "/%s" handleFindBarbecueById
                ])
                GET >=> choose [
                    route "/hello" >=> handleGetHello
                ]
            ])
        setStatusCode 404 >=> text "Not Found" ]