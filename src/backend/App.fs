module backend.App

open Giraffe
open Microsoft.AspNetCore.Http
open backend.Barbecue.Routes

let webApp: HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ subRoute "/api" (choose [ barbecueRoutes () ])
             setStatusCode 404 >=> text "Not Found" ]
