module backend.Barbecue.Routes

open Giraffe
open Handlers

let barbecueRoutes() = subRoute
                               "/barbecue"
                               (choose [ 
                                        GET >=> routef "/%s" handleFindBarbecueById
                                        GET >=> handleListBarbecues
                                        POST >=> route "/participant" >=> handleAddParticipantToBarbecue
                                        POST >=> handleCreateBarbecue
                                        DELETE >=> routef "/participant/%s" handleDeleteParticipantFromBarbecue ])