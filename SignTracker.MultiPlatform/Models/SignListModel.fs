namespace SignTracker.MultiPlatform

open Elmish
open SignTracker.MultiPlatform

module SignList =
    type State = {
        signs: Database.Sign seq
    }

    type Msg =
    | GetSignsAsync of userId: int
    | UpdateSigns of signs: Database.Sign seq
    | EditSign of sign: Database.Sign

    let init () =
        {
            signs = Seq.empty
        }, Cmd.ofMsg <| GetSignsAsync 1

    
    let update (msg: Msg) (state: State) =
        match msg with
        | GetSignsAsync userId -> 
            let updateAsync dispatch =
                async {
                    let! newSigns = DatabaseHandler.getSigns userId
                    dispatch <| UpdateSigns newSigns
                } |> Async.StartImmediate
            
            state, Cmd.ofEffect updateAsync
        | UpdateSigns newSigns ->
            { state with signs = newSigns }, Cmd.none
