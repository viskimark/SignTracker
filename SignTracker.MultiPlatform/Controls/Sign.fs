namespace SignTracker.MultiPlatform

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.FuncUI.Builder
open Avalonia.Layout

type Sign() =
    inherit Decorator()

    static member LineNumberProp = AvaloniaProperty.Register<Sign, string>("LineNumber", "85")
    member this.LineNumber
        with get () = this.GetValue(Sign.LineNumberProp)
        and set value = this.SetValue(Sign.LineNumberProp, value) |> ignore

    static member TerminusAProp = AvaloniaProperty.Register<Sign, string>("TerminusA", "Örs vezér tere M+H")
    member this.TerminusA
        with get () = this.GetValue(Sign.TerminusAProp)
        and set value = this.SetValue(Sign.TerminusAProp, value) |> ignore

    static member TerminusBProp = AvaloniaProperty.Register<Sign, string>("TerminusB", "Kőbánya-Kispest M")
    member this.TerminusB
        with get () = this.GetValue(Sign.TerminusBProp)
        and set value = this.SetValue(Sign.TerminusBProp, value) |> ignore

    static member FontProp = AvaloniaProperty.Register<Sign, Media.FontFamily>("Font", Media.FontFamily.Default)
    member this.Font
        with get () = this.GetValue(Sign.FontProp)
        and set value = this.SetValue(Sign.FontProp, value) |> ignore

    static member lineNum<'t when 't :> Sign> (lineNum: string) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<string>(Sign.LineNumberProp, lineNum, ValueNone)

    static member terminusA<'t when 't :> Sign> (terminusA: string) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<string>(Sign.TerminusAProp, terminusA, ValueNone)

    static member terminusB<'t when 't :> Sign> (terminusB: string) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<string>(Sign.TerminusBProp, terminusB, ValueNone)

    static member font<'t when 't :> Sign> (font: Media.FontFamily) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<Media.FontFamily>(Sign.FontProp, font, ValueNone)

    static member child<'t when 't :> Sign> (value: IView option) : IAttr<'t> =
        AttrBuilder<'t>.CreateContentSingle(Sign.ChildProperty, value)

    static member child<'t when 't :> Sign>(value: IView) : IAttr<'t> =
        value |> Some |> Sign.child


module Sign =
    let private create' (lineNum: string) (terminusA: string) (terminusB: string) (font: Media.FontFamily) =
        Border.create [
            Border.background Media.Colors.White
            Border.height 195
            Border.width 900
            Border.cornerRadius 25
            //Border.clipToBounds true

            Border.child (
                Grid.create [
                    let colDef =
                        [ for i in 0..lineNum.Length - 1 do
                            "Auto, " ]
                        |> List.fold ( fun state str -> state + str ) ""
                        |> sprintf "*, %s*, Auto, 2*"
                    Grid.columnDefinitions colDef
                    Grid.rowDefinitions "*, *"
                    //Grid.showGridLines true

                    Grid.children [
                        for i in 0..lineNum.Length - 1 do
                            TextBlock.create [
                                Grid.column <| i + 1
                                Grid.rowSpan 3
                                Grid.horizontalAlignment HorizontalAlignment.Center
                                Grid.verticalAlignment VerticalAlignment.Center

                                TextBlock.text <| sprintf "%c" lineNum[i]

                                TextBlock.fontSize 180
                                TextBlock.fontWeight Media.FontWeight.Bold
                                TextBlock.fontFamily font

                                let leftMargin =
                                    if i > 0 then
                                        if lineNum.Length > 2 then
                                            if lineNum[i - 1] = '1' then -27.5 else -17.5
                                        else -10
                                    else 0
                                let rightMargin =
                                    if i = lineNum.Length - 1 then 20 / lineNum.Length else 0
                                TextBlock.margin (leftMargin, 0, rightMargin, -30)

                                TextBlock.foreground Media.Colors.Black
                            ]
                        
                        let whichBorder =
                            let borderWidth = 4
                            if terminusA.Length > terminusB.Length then
                                [ borderWidth; 0 ]
                            else [ 0; borderWidth ]

                        // -----
                        // Terminus A
                        // -----
                        Border.create [
                            Border.borderThickness (0, 0, 0, whichBorder[0])
                            Border.borderBrush Media.Colors.Black

                            Grid.row 0
                            Grid.column <| lineNum.Length + 2
                            Grid.horizontalAlignment HorizontalAlignment.Left
                            Grid.verticalAlignment VerticalAlignment.Bottom
                            Border.child (

                                TextBlock.create [
                                    TextBlock.text <| terminusA.ToUpper()
                                    TextBlock.clipToBounds false

                                    TextBlock.margin (10, 0, 10, 0)
                                    TextBlock.fontFamily font
                                    TextBlock.fontWeight Media.FontWeight.Bold
                                    TextBlock.fontSize 50

                                    TextBlock.init (fun textBlock ->
                                        TextBlock.SetLetterSpacing(textBlock, -2.75)
                                    )
                                    TextBlock.foreground Media.Colors.Black

                                ]
                            )
                        ]

                        // -----
                        // Terminus B
                        // -----

                        Border.create [
                            Border.borderThickness (0, whichBorder[1], 0, 0)
                            Border.borderBrush Media.Colors.Black

                            Grid.row 1
                            Grid.column <| lineNum.Length + 2
                            Grid.horizontalAlignment HorizontalAlignment.Left
                            Grid.verticalAlignment VerticalAlignment.Top

                            Border.child (
                                TextBlock.create [
                                    TextBlock.text <| terminusB.ToUpper()
                                    TextBlock.clipToBounds false

                                    TextBlock.margin (10, 10, 10, 0)
                                    TextBlock.fontFamily font
                                    TextBlock.fontWeight Media.FontWeight.Bold
                                    TextBlock.fontSize 50

                                    TextBlock.init (fun textBlock ->
                                        TextBlock.SetLetterSpacing(textBlock, -2.75)
                                    )

                                    TextBlock.foreground Media.Colors.Black
                                ]
                            )
                        ]
                    ]
                ]
            )
        ]

    let create (attrs: IAttr<Sign> list) : IView<Sign> =
        let lineNum =
            match List.tryFind (fun (attr: IAttr<Sign>) -> attr.UniqueName = "LineNumber") attrs with
            | Some attr -> attr.Property.Value.Value.ToString()
            | None -> ""
            
        let terminusA =
            match List.tryFind (fun (attr: IAttr<Sign>) -> attr.UniqueName = "TerminusA") attrs with
            | Some attr -> attr.Property.Value.Value.ToString()
            | None -> ""

        let terminusB =
            match List.tryFind (fun (attr: IAttr<Sign>) -> attr.UniqueName = "TerminusB") attrs with
            | Some attr -> attr.Property.Value.Value.ToString()
            | None -> ""

        let font =
            match List.tryFind (fun (attr: IAttr<Sign>) -> attr.UniqueName = "Font") attrs with
            | Some attr -> attr.Property.Value.Value :?> Media.FontFamily
            | None -> Media.FontFamily.Default

        [
            Sign.child (
                create' lineNum terminusA terminusB font
            )
        ]
        |> List.append attrs
        |> ViewBuilder.Create<Sign>