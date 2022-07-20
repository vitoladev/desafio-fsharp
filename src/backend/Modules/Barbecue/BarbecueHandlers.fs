module backend.Barbecue.Handlers

open System
open backend.Barbecue.Service
open Microsoft.AspNetCore.Http
open Giraffe
open backend.Barbecue.Model
open backend.Models
open backend.Helpers

let handleListBarbecues: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let barbecues = listBarbecues ()

            return! Successful.OK barbecues next ctx
        }

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
                let barbecueId = createBarbecue payload

                let response: Response =
                    { id = barbecueId
                      message = "BARBECUE_CREATED_SUCCESSFULLY" }

                return! Successful.CREATED (json response) next ctx
            | err -> return! (setStatusCode 422 >=> json err) next ctx

        // match validationResult with
        // | _ ->

        // | ValidationError err -> return! (setStatusCode 422 >=> json (Error err)) next ctx
        }


let handleAddParticipantToBarbecue: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! validationResult = ThothSerializer.ReadBody ctx validateAddParticipantPayload

            match validationResult with
            | Ok payload ->
                let participantId = addParticipantToBarbecue payload

                let response: Response =
                    { id = participantId
                      message = "PARTICIPANT_ADDED_SUCCESSFULLY" }

                return! Successful.CREATED (json response) next ctx
            | err -> return! (setStatusCode 422 >=> json err) next ctx

        }

let handleDeleteParticipantFromBarbecue (barbecueIdParam: string) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let participantId = Guid.Parse barbecueIdParam

            removeParticipantFromBarbecue participantId
            |> ignore

            return! Successful.NO_CONTENT next ctx
        }
