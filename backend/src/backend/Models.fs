module backend.Models

open System

[<CLIMutable>]
type Message =
    {
        Text : string
    }

type User = { id: Guid; name: string; email: string; passwordHash: string }

type Participant = { id: Guid; name: string; contribution: int; barbecue_id: Guid }

type Barbecue = { id: Guid; name: string; date: DateTime }