module backend.Types

open System

type Response = { id: Guid; message: string }

type Error = { message: string }

type Participant =
    { id: Guid
      name: string
      contribution: int }

type Barbecue =
    { id: Guid
      name: string
      description: string
      date: DateTime }

type BarbecueWithParticipants =
    { id: Guid
      name: string
      description: string
      cost: int
      date: DateTime
      participants: Participant list }

type BarbecueSummary = 
    { id: Guid
      name: string
      description: string
      cost: int
      date: DateTime
      totalParticipants: int }
