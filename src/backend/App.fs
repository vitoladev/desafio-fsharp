module backend.App

open Giraffe
open Microsoft.AspNetCore.Http
open backend.Barbecue.Handlers

let webApp: HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ subRoute
                 "/api"
                 (choose [ subRoute
                               "/barbecue"
                               (choose [ GET >=> handleListBarbecues
                                         GET >=> routef "/%s" handleFindBarbecueById
                                         POST >=> handleCreateBarbecue
                                         POST
                                         >=> route "/participant"
                                         >=> handleAddParticipantToBarbecue
                                         DELETE
                                         >=> routef "/participant/%s" handleDeleteParticipantFromBarbecue ])
                            ])
             setStatusCode 404 >=> text "Not Found" ]
