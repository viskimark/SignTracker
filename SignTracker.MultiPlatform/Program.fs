namespace SignTracker.MultiPlatform

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Builder
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.Elmish
open Avalonia.Layout
open SignTracker.MultiPlatform.Views

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "SignTracker"

        Elmish.Program.mkProgram Shell.init Shell.update Shell.view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.run

type MainView() as this =
    inherit HostControl()
    do
        Elmish.Program.mkProgram Shell.init Shell.update Shell.view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        LocalDatabase.init ()

        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? ISingleViewApplicationLifetime as singleLifetime ->
            let mainView = MainView()
            singleLifetime.MainView <- mainView
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            let mainWindow = MainWindow()
            desktopLifetime.MainWindow <- mainWindow
        |_ -> ()
