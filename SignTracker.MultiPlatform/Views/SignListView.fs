namespace SignTracker.MultiPlatform.Views

open Elmish
open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open SignTracker.MultiPlatform

module SignList =
    open SignList
    let view (state: State) (dispatch) =
        ScrollViewer.create [
            ScrollViewer.dock Dock.Bottom
            ScrollViewer.verticalScrollBarVisibility Primitives.ScrollBarVisibility.Hidden
            ScrollViewer.content (
                StackPanel.create [
                    StackPanel.children [
                        for sign in state.signs do
                            Viewbox.create [
                                Viewbox.stretch Media.Stretch.Fill
                                Viewbox.maxWidth 1000
                                Viewbox.child (
                                    SignExtension.create [
                                        SignExtension.padding (0, 10)
                                        SignExtension.sign sign
                                        SignExtension.onTapped (fun _ -> dispatch (EditSign sign))
                                    ]
                                )
                            ]
                    ]
                ]
            )
        ]
