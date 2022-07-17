module backend.Barbecue.Service

open System
open backend.Database
open backend.Models
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL

let listBarbecues() = 
  use connection = getDbConnection()
  select {
    for b in table<Barbecue> do selectAll
  }
  |> connection.SelectAsync<Barbecue>

let findBarbecueById (id: Guid) =
  use connection = getDbConnection()
  select {
    for b in table<Barbecue> do
    innerJoin p in table<Participant> on (b.id = p.barbecue_id)
    where (b.id = id)
  }
  |> connection.SelectAsync<Barbecue>
  |> waitTask |> Seq.exactlyOne