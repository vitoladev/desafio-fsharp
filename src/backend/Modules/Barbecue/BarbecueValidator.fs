module backend.Barbecue.Validator

open System
open Thoth.Json.Net

type AddParticipantPayload = { name: string; contribution: int; barbecue_id: Guid }

let validateAddParticipantPayload: Decoder<AddParticipantPayload> =
    Decode.object(
        fun get -> {
            AddParticipantPayload.name = get.Required.Field "name" Decode.string
            AddParticipantPayload.contribution = get.Required.Field "contribution" Decode.int
            AddParticipantPayload.barbecue_id = get.Required.Field "barbecue_id" Decode.guid
        }
    )

type ParticipantPayload = { name: string; contribution: int; }

let validateParticipantPayload: Decoder<ParticipantPayload> =
    Decode.object (
        fun get -> {
            ParticipantPayload.name = get.Required.Field "name" Decode.string
            ParticipantPayload.contribution = get.Required.Field "contribution" Decode.int
        }
    )


type BarbecuePayload =
    { name: string
      description: string
      date: DateTime
      participants: ParticipantPayload list }

let validateBarbecuePayload: Decoder<BarbecuePayload> =
    Decode.object (
        fun get -> {
                BarbecuePayload.name = get.Required.Field "name" Decode.string
                BarbecuePayload.description = get.Required.Field "description" Decode.string
                BarbecuePayload.date = get.Required.Field "date" Decode.datetime
                BarbecuePayload.participants = get.Required.Field "participants" (Decode.list validateParticipantPayload)
        }
    )
