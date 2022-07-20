module backend.Models

open System

type Response = {
    id: Guid;
    message: string;
}

type User = { id: Guid; name: string; email: string; passwordHash: string }

type Participant = { id: Guid; name: string; contribution: int; barbecue_id: Guid }

type Barbecue = { id: Guid; name: string; description: string; date: DateTime }