namespace SignTracker.MultiPlatform

open Elmish
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Elmish.ElmishHook
open Avalonia.FuncUI.Types

module Shell =
    type Pages =
    | SignList
    | AddSign

    type State = {
        SignListState: SignList.State
        AddSignState: EditSign.State
        ActivePage: Pages
        UserId: int
        X: float
    }

    type Msg =
    | SignListMsg of SignList.Msg
    | AddSignMsg of EditSign.Msg
    | ChangePage of Pages
    | Coords of float
    | CarouselNext of float * Carousel

    let init () =
        let userId = 1
        let signListState, signListCmd = SignList.init ()
        let addSignState, addSignCmd = EditSign.init ()
        {
            SignListState = signListState
            AddSignState = addSignState
            ActivePage = SignList
            UserId = userId
            X = 0
        }, Cmd.batch [
            Cmd.map SignListMsg signListCmd
            Cmd.map AddSignMsg addSignCmd
        ]

    let update (msg: Msg) (state: State) =
        match msg with
        | SignListMsg msg ->
            match msg with
            //We need this, because SignExtensions are in the SignList page, but we want to change pages
            | SignList.Msg.EditSign sign ->
                let state', cmd = EditSign.edit sign
                { state with AddSignState = state' },
                Cmd.batch [
                    Cmd.ofMsg <| ChangePage Pages.AddSign
                ]
            //Everything else gets routed back to the SignList page
            |_ ->
                let newState, cmd = SignList.update msg state.SignListState
                { state with SignListState = newState }, Cmd.map SignListMsg cmd

        | AddSignMsg msg ->
            let newState, cmd = EditSign.update msg state.AddSignState
            { state with AddSignState = newState }, Cmd.map AddSignMsg cmd

        | ChangePage page ->
            { state with ActivePage = page }, Cmd.none

        | Coords x ->
            { state with X = x }, Cmd.none

        | CarouselNext (x, c) ->
            let dist = x - state.X
            if dist > 50 then c.Next()
            if dist < -50 then c.Previous()
            state, Cmd.none
