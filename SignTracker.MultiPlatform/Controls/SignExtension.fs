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
open SignTracker.MultiPlatform.Database
open Avalonia.Interactivity

type SignExtension() =
    inherit Decorator()

    //static member LineTypeProp = AvaloniaProperty.Register<SignExtension, LineType>("LineType", LineType.Bus)
    //member this.LineType
    //    with get () = this.GetValue(SignExtension.LineTypeProp)
    //    and set value = this.SetValue(SignExtension.LineTypeProp, value) |> ignore

    //static member SignTypeProp = AvaloniaProperty.Register<SignExtension, SignType>("SignType", SignType.Újbalos)
    //member this.SignType
    //    with get () = this.GetValue(SignExtension.SignTypeProp)
    //    and set value = this.SetValue(SignExtension.SignTypeProp, value) |> ignore

    //static member DateMadeProp = AvaloniaProperty.Register<SignExtension, System.DateOnly>("DateMade", System.DateOnly.Parse "2020-01-01")
    //member this.DateMade
    //    with get () = this.GetValue(SignExtension.DateMadeProp)
    //    and set value = this.SetValue(SignExtension.DateMadeProp, value) |> ignore

    //static member DateGotProp = AvaloniaProperty.Register<SignExtension, System.DateOnly>("DateGot", System.DateOnly.FromDateTime System.DateTime.Today)
    //member this.DateGot
    //    with get () = this.GetValue(SignExtension.DateGotProp)
    //    and set value = this.SetValue(SignExtension.DateGotProp, value) |> ignore

    //static member PriceProp = AvaloniaProperty.Register<SignExtension, decimal>("Price", 5000M)
    //member this.Price
    //    with get () = this.GetValue(SignExtension.PriceProp)
    //    and set value = this.SetValue(SignExtension.PriceProp, value) |> ignore

    //static member WorthProp = AvaloniaProperty.Register<SignExtension, decimal>("Worth", 5000M)
    //member this.Worth
    //    with get () = this.GetValue(SignExtension.WorthProp)
    //    and set value = this.SetValue(SignExtension.WorthProp, value) |> ignore

    //static member IsSoldProp = AvaloniaProperty.Register<SignExtension, bool>("IsSold", false)
    //member this.IsSold
    //    with get () = this.GetValue(SignExtension.IsSoldProp)
    //    and set value = this.SetValue(SignExtension.IsSoldProp, value) |> ignore

    //static member DateSoldProp = AvaloniaProperty.Register<SignExtension, System.DateOnly option>("DateSold", None)
    //member this.DateSold
    //    with get () = this.GetValue(SignExtension.DateSoldProp)
    //    and set value = this.SetValue(SignExtension.DateSoldProp, value) |> ignore

    static member defaultSign: Sign = {
        Id = None
        Line = Some "80"
        TerminusA = Some "Örs vezér tere M+H"
        TerminusB = Some "Keleti pályaudvar M"
        LineType = LineType.Trolley
        SignType = SignType.Újbalos
        DateMade = Some <| System.DateOnly.Parse "2021-01-01"
        DateGot = System.DateOnly.Parse "2022-01-01"
        Price = 5000M
        Worth = 5000M
        IsSold = true
        DateSold = System.DateOnly.Parse "2023-01-01" |> Some
    }
    static member SignProp = AvaloniaProperty.Register<SignExtension, Sign>("Sign", SignExtension.defaultSign)
    member this.Sign
        with get () = this.GetValue(SignExtension.SignProp)
        and set value = this.SetValue(SignExtension.SignProp, value) |> ignore

    static member sign<'t when 't :> SignExtension> (sign: Sign) : IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<Sign>(SignExtension.SignProp, sign, ValueNone)
        
    //static member onClick<'t when 't :> SignExtension>(func: RoutedEventArgs -> unit, ?subPatchOptions) =
    //    AttrBuilder<'t>.CreateSubscription<RoutedEventArgs>(SignExtension.On, func, ?subPatchOptions = subPatchOptions)

    static member child<'t when 't :> SignExtension> (value: IView option) : IAttr<'t> =
        AttrBuilder<'t>.CreateContentSingle(SignExtension.ChildProperty, value)

    static member child<'t when 't :> SignExtension>(value: IView) : IAttr<'t> =
        value |> Some |> SignExtension.child


module SignExtension =
    let private infoCollection sign font padding gridCol gridRow =
        let dateMade =
            match sign.DateMade with
            | Some dt ->
                let format = if dt.Year >= 2003 then "yyMM" else @"\'yy\/MM"
                dt.ToString format
            | None -> "-"

        Grid.create [
            Grid.column gridCol
            Grid.row gridRow

            Grid.columnDefinitions "*, *, *"
            Grid.rowDefinitions "*, *"

            Grid.children [
                InfoBox.create [
                    InfoBox.text <| sign.SignType.ToString()
                    InfoBox.font font

                    InfoBox.pad padding
                    InfoBox.padding(0, padding, padding, padding)
                    Grid.column 0
                    Grid.row 0
                ]
                InfoBox.create [
                    InfoBox.text <| dateMade
                    InfoBox.font font

                    InfoBox.pad padding
                    InfoBox.padding(0, 0, padding, padding)
                    Grid.column 0
                    Grid.row 1
                ]
                InfoBox.create [
                    InfoBox.text <| sign.DateGot.ToString "yy.MM.dd."
                    InfoBox.font font

                    InfoBox.pad padding
                    InfoBox.padding(0, padding, padding, padding)
                    Grid.column 1
                    Grid.row 0
                ]
                InfoBox.create [
                    let text =
                        if sign.IsSold then sign.DateSold.Value.ToString "yy.MM.dd."
                        else
                            let got: System.DateTime = sign.DateGot.ToDateTime(System.TimeOnly.MinValue)
                            let today = System.DateTime.Today
                            let months =
                                ((today.Year - got.Year) * 12) + (today.Month - got.Month)
                            if months < 12 then
                                sprintf "%i hónapja" months
                            else sprintf "%s éve" <| System.Math.Round(float months / 12., 1).ToString()
                        
                    InfoBox.text text
                    InfoBox.font font

                    InfoBox.pad padding
                    InfoBox.padding(0, 0, padding, padding)
                    Grid.column 1
                    Grid.row 1
                ]
                InfoBox.create [
                    InfoBox.text <| sprintf "-%.0f Ft" sign.Price
                    InfoBox.font font

                    InfoBox.pad padding
                    InfoBox.padding(0, padding, padding, padding)
                    Grid.column 2
                    Grid.row 0
                ]
                InfoBox.create [
                    InfoBox.text <| sprintf "+%.0f Ft" sign.Worth
                    InfoBox.font font

                    InfoBox.pad padding
                    InfoBox.padding(0, 0, padding, padding)
                    Grid.column 2
                    Grid.row 1
                ]
            ]
        ]
        
    let private create' (sign: Sign) =
        let font = 
            match sign.SignType with
            |_ -> Media.FontFamily("avares://SignTracker.MultiPlatform/Assets/Fonts#Frutiger Next")
        let padding = 15

        let parseOption str =
            match str with
            | Some str -> str
            | None -> "-"

        Border.create [
            let bgColor =
                match sign.LineType with
                | LineType.Bus -> "#FF009FE3"
                | LineType.Trolley -> "#FFE51E17"
                | LineType.Tram -> "#FFFFD500"
                | LineType.Night -> "#FFFFFFFF"
                | _ -> "#FF888888"
            Border.background bgColor
            Border.cornerRadius 25
            Border.child (
                Grid.create [
                    Grid.columnDefinitions "Auto, Auto"
                    Grid.rowDefinitions "*, *"
                    Grid.children [
                        Sign.create [
                            Sign.lineNum <| parseOption sign.Line
                            Sign.terminusA <| parseOption sign.TerminusA
                            Sign.terminusB <| parseOption sign.TerminusB
                            Sign.font font

                            Sign.padding padding
                            Grid.column 0
                            Grid.row 0
                            Grid.rowSpan 2
                        ]
                        infoCollection sign font padding 1 0
                    ]
                ]
            )
        ]

    let create (attrs: IAttr<SignExtension> list) : IView<SignExtension> =
        let sign =
            match List.tryFind (fun (attr: IAttr<SignExtension>) -> attr.UniqueName = "Sign") attrs with
            | Some attr -> attr.Property.Value.Value :?> Sign
            | None -> SignExtension.defaultSign

        [
            SignExtension.child (
                create' sign
            )
        ]
        |> List.append attrs
        |> ViewBuilder.Create<SignExtension>