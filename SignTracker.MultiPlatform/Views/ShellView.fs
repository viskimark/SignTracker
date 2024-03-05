namespace SignTracker.MultiPlatform.Views

open Elmish
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Elmish.ElmishHook
open SignTracker.MultiPlatform.Views
open Avalonia.FuncUI.Types
open SignTracker.MultiPlatform
open Avalonia.Input.GestureRecognizers
open Avalonia.Input
open Avalonia.Animation

module Shell =
    open Shell

    let view state dispatch =
        DockPanel.create [
            DockPanel.children [
                //Carousel.create [
                //    Carousel.init (fun c ->
                //        c.AddHandler(Carousel.PointerPressedEvent, (fun sender args ->
                //            let p = args.GetCurrentPoint(null).Position
                //            dispatch <| Coords (p.X, p.Y)))
                //        c.AddHandler(Carousel.PointerReleasedEvent, (fun sender args ->
                //            let p = args.GetCurrentPoint(null).Position
                //            dispatch <| CarouselNext (p.X, c)
                //        ))
                //    )
                //    Carousel.viewItems [
                //        Border.create [ Border.width 500; Border.height 500; Border.background Avalonia.Media.Colors.Blue; Border.child ( TextBlock.create [ TextBlock.text $"{state.X}" ] ) ]
                //        Border.create [ Border.width 500; Border.height 500; Border.background Avalonia.Media.Colors.Red; Border.child ( TextBlock.create [ TextBlock.text $"{state.X}" ] ) ]
                //    ]
                //]
                match state.ActivePage with
                | SignList -> 
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.dock Dock.Top
                        StackPanel.children [
                            Button.create [
                                Button.content "Refresh"
                                Button.onClick (fun _ -> dispatch (SignListMsg <| SignList.Msg.GetSignsAsync 1))
                            ]
                            Button.create [
                                Button.content "Add sign"
                                Button.onClick (fun _ -> dispatch <| ChangePage Pages.AddSign)
                            ]
                        ]
                    ]
                    SignList.view state.SignListState (SignListMsg >> dispatch)

                | AddSign ->
                    StackPanel.create [
                        StackPanel.children [
                            EditSign.view state.AddSignState (AddSignMsg >> dispatch)
                            Grid.create [
                                Grid.horizontalAlignment HorizontalAlignment.Center
                                Grid.columnDefinitions "*, *"
                                Grid.children [
                                    Button.create [
                                        Button.content "Save"
                                        Button.padding (10, 10)
                                        Button.onClick (fun _ ->
                                            dispatch (AddSignMsg <| EditSign.Msg.SignCreated state.UserId)
                                            dispatch (SignListMsg <| SignList.Msg.GetSignsAsync 1)
                                            dispatch <| ChangePage Pages.SignList
                                        )
                                    ]
                                ]
                            ]
                        ]
                    ]
            ]
        ]