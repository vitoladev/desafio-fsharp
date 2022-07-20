module backend.Database

open Npgsql

let connectionString = System.Environment.GetEnvironmentVariable "DB_CONNECTION"

let getDbConnection() =
  let connection = new NpgsqlConnection("postgres://postgres:5432@localhost/postgres")

  connection.Open()
  connection
