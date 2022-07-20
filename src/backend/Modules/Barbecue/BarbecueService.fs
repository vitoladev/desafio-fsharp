module backend.Barbecue.Service

open System
open backend.Database
open backend.Models
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL
open Model
open backend.Helpers

let barbecueTable = table'<Barbecue> "barbecue"
let participantTable = table'<Participant> "participant"

let listBarbecues () =
    use connection = getDbConnection ()

    select {
        for b in barbecueTable do
            selectAll
    }
    |> connection.SelectAsync<Barbecue>
    |> waitTask

let findBarbecueById (id: Guid) =
    use connection = getDbConnection ()

    select {
        for b in barbecueTable do
            innerJoin p in table<Participant> on (b.id = p.barbecue_id)
            where (b.id = id)
    }
    |> connection.SelectAsync<Barbecue>
    |> waitTask
    |> Seq.exactlyOne

let createBarbecue (barbecue: BarbecuePayload) =
    use connection = getDbConnection ()

    let barbecueId = Guid.NewGuid()

    let newBarbecue =
        { id = barbecueId
          name = barbecue.name
          description = barbecue.description
          date = barbecue.date }

    insert {
        into barbecueTable
        value newBarbecue
    }
    |> connection.InsertAsync
    |> waitTask
    |> ignore

    barbecueId


let addParticipantToBarbecue (participant: AddParticipantPayload) =
    use connection = getDbConnection ()

    let participantId = Guid.NewGuid()

    let newParticipant =
        { id = participantId
          name = participant.name
          contribution = participant.contribution
          barbecue_id = participant.barbecue_id }

    insert {
        into participantTable
        value newParticipant
    }
    |> connection.InsertAsync
    |> waitTask
    |> ignore

    participantId

let removeParticipantFromBarbecue (participantId: Guid) =
    use connection = getDbConnection ()

    delete {
        for p in participantTable do
            where (p.id = participantId)
    }
    |> connection.DeleteAsync
    |> waitTask
    |> ignore
