module backend.Database

open Npgsql.FSharp

let connectionString = System.Environment.GetEnvironmentVariable "DB_CONNECTION"

let getDbConnection() =
  use connection =
      connectionString
      |> Sql.connect
      |> Sql.createConnection

  connection.Open()
  connection


let waitTask t = t |> Async.AwaitTask |> Async.RunSynchronously
