module backend.User

open System
open backend.Database
open backend.Models
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL
open Helpers

let listUsers _ =
  use connection = getDbConnection()
  select {
    for u in table<User> do selectAll
  }
  |> connection.SelectAsync<User>
  |> waitTask

let findUserById (id: Guid) =
  use connection = getDbConnection()
  select {
    for u in table<User> do
    where (u.id = id)
  } |> connection.SelectAsync<User>
  |> waitTask |> Seq.exactlyOne

let createUser (payload: User) = 
  use connection = getDbConnection()
  insert {
    into table<User>
    value { id = Guid.NewGuid(); name = payload.name; email = payload.email; passwordHash = "" }
  } |> connection.InsertAsync |> waitTask |> ignore