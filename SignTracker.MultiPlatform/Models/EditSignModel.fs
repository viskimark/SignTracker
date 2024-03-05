namespace SignTracker.MultiPlatform

open Elmish
open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Hosts
open SignTracker.MultiPlatform

module EditSign =
    type State = {
        // Sign members
        id: int option
        lineNum: string
        terminusA: string
        terminusB: string
        lineType: Database.LineType
        signType: Database.SignType
        hasDateMade: bool
        dateMade: System.DateOnly option
        dateGot: System.DateOnly
        price: decimal
        worth: decimal
        // Page members
        isEdit: bool
    }

    let emptySign: Database.Sign = {
        Id = None
        Line = Some ""
        TerminusA = Some ""
        TerminusB = Some ""
        LineType = Database.LineType.Bus
        SignType = Database.SignType.Újbalos
        DateMade =  System.DateTime.Today |> System.DateOnly.FromDateTime |> Some
        DateGot = System.DateTime.Today |> System.DateOnly.FromDateTime
        Price = 0M
        Worth = 0M
        IsSold = false
        DateSold = None
    }

    type Msg =
    | SignCreated of userId: int
    | LineNumChanged of string
    | TerminusAChanged of string
    | TerminusBChanged of string
    | LineTypeChanged of Database.LineType
    | SignTypeChanged of Database.SignType
    | HasDateMadeChanged
    | DateMadeChanged of System.DateTime System.Nullable
    | DateGotChanged of System.DateTime System.Nullable
    | PriceChanged of decimal
    | WorthChanged of decimal

    let init () =
        {
            id = None
            lineNum = "10"
            terminusA = "Felső sor"
            terminusB = "Alsó sor"
            lineType = Database.LineType.Bus
            signType = Database.SignType.Újbalos
            hasDateMade = true
            dateMade =  System.DateTime.Today |> System.DateOnly.FromDateTime |> Some
            dateGot = System.DateTime.Today |> System.DateOnly.FromDateTime
            price = 0M
            worth = 0M
            isEdit = false
        }, Cmd.none

    let edit (sign: Database.Sign) =
        let hasDateMade, dateMade =
            match sign.DateMade with
            | Some dt -> true, Some dt
            | None -> false, None
        {
            id = sign.Id
            lineNum = if sign.Line.IsSome then sign.Line.Value else ""
            terminusA = if sign.TerminusA.IsSome then sign.TerminusA.Value else ""
            terminusB = if sign.TerminusB.IsSome then sign.TerminusB.Value else ""
            lineType = sign.LineType
            signType = sign.SignType
            hasDateMade = hasDateMade
            dateMade =  dateMade
            dateGot = sign.DateGot
            price = sign.Price
            worth = sign.Worth
            isEdit = true
        }, Cmd.none

    let parseOption str =
        match str with
        | Some str -> str
        | None -> ""
    
    let update (msg: Msg) (state: State) =
        match msg with
        | SignCreated userId -> 
            let newSign : Database.Sign = {
                Id = state.id
                Line = if state.lineNum.Length > 0 then Some state.lineNum else None
                TerminusA = if state.terminusA.Length > 0 then Some state.terminusA else None
                TerminusB = if state.terminusB.Length > 0 then Some state.terminusB else None
                LineType = state.lineType
                SignType = state.signType
                DateMade = state.dateMade
                DateGot = state.dateGot
                Price = state.price
                Worth = state.worth
                IsSold = false
                DateSold = None
            }
            if state.isEdit then
                DatabaseHandler.editSign newSign |> Async.StartImmediate
            else
                DatabaseHandler.addSign newSign userId |> Async.StartImmediate
            init ()
        | LineNumChanged str -> { state with lineNum = str }, Cmd.none
        | TerminusAChanged str -> { state with terminusA = str }, Cmd.none
        | TerminusBChanged str -> { state with terminusB = str }, Cmd.none
        | LineTypeChanged t -> { state with lineType = t }, Cmd.none
        | SignTypeChanged t -> { state with signType = t }, Cmd.none
        | HasDateMadeChanged -> { state with hasDateMade = not state.hasDateMade }, Cmd.ofMsg (DateMadeChanged <| System.Nullable())
        | DateMadeChanged dt ->
            let dateOnly =
                if state.hasDateMade then
                    if dt.HasValue then
                        dt.Value |> System.DateOnly.FromDateTime |> Some
                    else System.DateTime.Today |> System.DateOnly.FromDateTime |> Some
                else None
            { state with dateMade = dateOnly }, Cmd.none
        | DateGotChanged dt ->
            let dateOnly =
                if dt.HasValue then
                    System.DateOnly.FromDateTime <| dt.Value
                else System.DateTime.Today |> System.DateOnly.FromDateTime
            { state with dateGot = dateOnly }, Cmd.none
        | PriceChanged d -> { state with price = d }, Cmd.none
        | WorthChanged d -> { state with worth = d }, Cmd.none
