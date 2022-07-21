module backend.Barbecue.Service

open System
open Npgsql.FSharp
open Validator
open backend.Helpers
open System.Threading.Tasks
open backend.Types

type FindBarbecueQueryResult =
    { id: Guid
      name: string
      description: string
      date: DateTime
      participant: Participant }

type CreateBarbecueInsertResult = { id: Guid }

let CreateBarbecueInsert =
    """INSERT INTO barbecue (id, name, description, date) VALUES (@barbecueId, @name, @description, @date)"""

let AddParticipantInsert =
    """INSERT INTO participant (id, name, contribution, barbecue_id)
                VALUES(gen_random_uuid(), @name, @contribution, @barbecueId) RETURNING id"""

let RemoveParticipantDelete = """DELETE FROM barbecue WHERE @barbecueId"""

let ListBarbecuesQuery =
    """
SELECT DISTINCT ON (barbecue.id)
        barbecue.id,
        barbecue.name,
        barbecue.description,
        barbecue.date,
        coalesce(SUM(participant.contribution) over(partition by barbecue.id), 0) as barbecue_cost,
        COUNT(participant.id) over(partition by barbecue.id) as barbecue_participants
    FROM barbecue
        LEFT JOIN participant ON participant.barbecue_id = barbecue.id
"""

let FindBarbecueByIdQuery =
    """
SELECT barbecue.id,
        barbecue.name,
        barbecue.description,
        barbecue.date,
        participant.id as participant_id,
        participant.name as participant_name,
        participant.contribution as participant_contribution
    FROM barbecue
        INNER JOIN participant ON participant.barbecue_id = barbecue.id
        WHERE barbecue.id = @barbecueId
"""


let listBarbecues _ =
    connectionString
    |> Sql.connect
    |> Sql.query ListBarbecuesQuery
    |> Sql.executeAsync (fun read ->
        { id = read.uuid "id"
          name = read.string "name"
          description = read.string "description"
          date = read.dateTime "date"
          cost = read.int "barbecue_cost"
          totalParticipants = read.int "barbecue_participants" })

let findBarbecueById (id: Guid) =
    let barbecue =
        connectionString
        |> Sql.connect
        |> Sql.query FindBarbecueByIdQuery
        |> Sql.parameters [ "@barbecueId", Sql.uuid id ]
        |> Sql.executeAsync (fun read ->
            { id = read.uuid "id"
              name = read.string "name"
              description = read.string "description"
              date = read.dateTime "date"
              participant =
                { id = read.uuid "participant_id"
                  name = read.string "participant_name"
                  contribution = read.int "participant_contribution" } })
        |> waitTask

    let participants = List.map (fun b -> b.participant) barbecue

    let barbecueInfo =
        { id = barbecue[0].id
          name = barbecue[0].name
          description = barbecue[0].description
          date = barbecue[0].date
          cost =
            participants
            |> List.map (fun p -> p.contribution)
            |> List.sum
          participants = participants }

    barbecueInfo

let createBarbecue (barbecue: BarbecuePayload) : Task<Result<Response, Error>> =
    task {
        try
            let barbecueId = Guid.NewGuid()

            let participants =
                barbecue.participants
                |> List.map (fun p ->
                    [ "@name", Sql.text p.name
                      "@contribution", Sql.int p.contribution
                      "@barbecueId", Sql.uuid barbecueId ])

            let barbecueParams =
                [ "@barbecueId", Sql.uuid barbecueId
                  "@name", Sql.string barbecue.name
                  "@description", Sql.string barbecue.description
                  "@date", Sql.timestamptz barbecue.date ]

            connectionString
            |> Sql.connect
            |> Sql.executeTransactionAsync [ CreateBarbecueInsert, [ barbecueParams ]
                                             AddParticipantInsert, participants ]
            |> waitTask
            |> ignore

            return
                Ok
                    { id = barbecueId
                      message = "BARBECUE_CREATED_SUCCESSFULLY" }
        with
        | _ -> return Error { message = "FAILED_TO_CREATE_BARBECUE" }
    }


let addParticipantToBarbecue (participant: AddParticipantPayload) =
    task {
        try
            let! participant =
                connectionString
                |> Sql.connect
                |> Sql.query AddParticipantInsert
                |> Sql.parameters [ "@name", Sql.text participant.name
                                    "@contribution", Sql.int participant.contribution
                                    "@barbecueId", Sql.uuid participant.barbecue_id ]
                |> Sql.executeRowAsync (fun reader -> { id = reader.uuid "id" })

            return
                Ok
                    { message = "PARTICIPANT_ADDED_SUCCESSFULLY"
                      id = participant.id }
        with
        | _ -> return Error { message = "FAILED_TO_ADD_PARTICIPANT" }
    }



let removeParticipantFromBarbecue (participantId: Guid) =
    connectionString
    |> Sql.connect
    |> Sql.query RemoveParticipantDelete
    |> Sql.parameters [ "@barbecueId", Sql.uuid participantId ]
    |> Sql.executeNonQueryAsync
