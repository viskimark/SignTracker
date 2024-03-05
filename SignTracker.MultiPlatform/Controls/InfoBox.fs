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

type InfoBox() =
    inherit Decorator()

    static member TextProp = AvaloniaProperty.Register<InfoBox, string>("Text", "Info")
    member this.Text
        with get () = this.GetValue(InfoBox.TextProp)
        and set value = this.SetValue(InfoBox.TextProp, value) |> ignore

    static member PadProp = AvaloniaProperty.Register<InfoBox, double>("Pad", 15.)
    member this.Pad
        with get () = this.GetValue(InfoBox.PadProp)
        and set value = this.SetValue(InfoBox.PadProp, value) |> ignore

    static member FontProp = AvaloniaProperty.Register<InfoBox, Media.FontFamily>("Font", Media.FontFamily.Default)
    member this.Font
        with get () = this.GetValue(InfoBox.FontProp)
        and set value = this.SetValue(InfoBox.FontProp, value) |> ignore

    static member text<'t when 't :> InfoBox> (text: string) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<string>(InfoBox.TextProp, text, ValueNone)

    static member pad<'t when 't :> InfoBox> (amount: double) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<double>(InfoBox.PadProp, amount, ValueNone)

    static member font<'t when 't :> InfoBox> (font: Media.FontFamily) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<Media.FontFamily>(InfoBox.FontProp, font, ValueNone)

    static member child<'t when 't :> InfoBox> (value: IView option) : IAttr<'t> =
        AttrBuilder<'t>.CreateContentSingle(InfoBox.ChildProperty, value)

    static member child<'t when 't :> InfoBox>(value: IView) : IAttr<'t> =
        value |> Some |> InfoBox.child


module InfoBox =
    let private create' (text: string) (padding: double) (font: Media.FontFamily) =

        let height = (195. / 2.) - padding
        let width = height * 3.0 //4.5

        Border.create [
            Border.background Media.Colors.White
            Border.height <| height
            Border.width <| width
            Border.cornerRadius 25

            Border.child (
                TextBlock.create [
                    TextBlock.text text
                    TextBlock.foreground Media.Colors.Black
                    TextBlock.fontFamily font
                    TextBlock.clipToBounds false

                    //TextBlock.margin (10, 0, 10, 0)
                    TextBlock.fontWeight Media.FontWeight.DemiBold
                    TextBlock.fontSize 50

                    TextBlock.textAlignment Media.TextAlignment.Center
                    TextBlock.verticalAlignment VerticalAlignment.Center

                    TextBlock.init (fun textBlock ->
                        TextBlock.SetLetterSpacing(textBlock, -2.75)
                    )
                ]
            )
        ]

    let create (attrs: IAttr<InfoBox> list) : IView<InfoBox> =
        let text =
            match List.tryFind (fun (attr: IAttr<InfoBox>) -> attr.UniqueName = "Text") attrs with
            | Some attr -> attr.Property.Value.Value.ToString()
            | None -> "None"

        let padding =
            match List.tryFind (fun (attr: IAttr<InfoBox>) -> attr.UniqueName = "Pad") attrs with
            | Some attr -> attr.Property.Value.Value :?> double
            | None -> 0.

        let font =
            match List.tryFind (fun (attr: IAttr<InfoBox>) -> attr.UniqueName = "Font") attrs with
            | Some attr -> attr.Property.Value.Value :?> Media.FontFamily
            | None -> Media.FontFamily.Default

        [
            InfoBox.child (
                create' text padding font
            )
        ]
        |> List.append attrs
        |> ViewBuilder.Create<InfoBox>