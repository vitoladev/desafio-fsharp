module backend.Barbecue.Handlers

open System
open backend.Barbecue.Service
open Microsoft.AspNetCore.Http
open Giraffe
open backend.Barbecue.Validator
open backend.Helpers

let handleListBarbecues: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let barbecues = listBarbecues () |> waitTask

        Successful.OK barbecues next ctx

let handleFindBarbecueById (barbecueIdParam: string) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let barbecueId = Guid.Parse barbecueIdParam
            let barbecue = findBarbecueById barbecueId

            return! Successful.OK barbecue next ctx
        }

let handleCreateBarbecue: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! validationResult = ThothSerializer.ReadBody ctx validateBarbecuePayload

            match validationResult with
            | Ok payload ->
                let! result = createBarbecue payload

                match result with
                | Ok barbecue -> return! Successful.CREATED barbecue next ctx
                | Error err -> return! (setStatusCode 400 >=> json err) next ctx


            | err -> return! (setStatusCode 422 >=> json err) next ctx
        }


let handleAddParticipantToBarbecue: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! validationResult = ThothSerializer.ReadBody ctx validateAddParticipantPayload

            match validationResult with
            | Ok payload ->
                let! result = addParticipantToBarbecue payload

                match result with
                | Ok participant -> return! Successful.CREATED participant next ctx
                | Error err -> return! (setStatusCode 400 >=> json err) next ctx
            | err -> return! (setStatusCode 422 >=> json err) next ctx
        }

let handleDeleteParticipantFromBarbecue (participantIdParam: string) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let participantId = Guid.Parse participantIdParam

            removeParticipantFromBarbecue participantId
            |> waitTask
            |> ignore

            return! Successful.NO_CONTENT next ctx
        }
