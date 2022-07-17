module backend.Barbecue.Handlers

open Giraffe
open System
open backend.Barbecue.Service
open Microsoft.AspNetCore.Http

let handleListBarbecues: HttpHandler = 
    fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
      let! barbecues = listBarbecues()

      return! Successful.OK barbecues next ctx
    }

let handleFindBarbecueById (barbecueIdParam: string): HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
      let barbecueId = Guid.Parse barbecueIdParam
      let barbecue = findBarbecueById barbecueId

      return! Successful.OK barbecue next ctx
    }
