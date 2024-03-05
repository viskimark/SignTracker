namespace SignTracker.MultiPlatform.Views

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
    open EditSign
    let view (state: State) (dispatch) =
        let maxWidth = 500.
        Viewbox.create [
            Viewbox.stretch Media.Stretch.Uniform
            Viewbox.maxWidth 1000
            Viewbox.child (
                StackPanel.create [
                    StackPanel.spacing 10
                    StackPanel.margin 10
                    StackPanel.children [
                        // Line number
                        TextBox.create [
                            TextBox.text state.lineNum
                            TextBox.maxWidth maxWidth
                            TextBox.onTextChanged (fun str ->
                                dispatch <| LineNumChanged str
                            )
                        ]

                        // Terminus A
                        TextBox.create [
                            TextBox.text state.terminusA
                            TextBox.maxWidth maxWidth
                            TextBox.onTextChanged (fun str ->
                                dispatch <| TerminusAChanged str
                            )
                        ]

                        // Terminus B
                        TextBox.create [
                            TextBox.text state.terminusB
                            TextBox.maxWidth maxWidth
                            TextBox.onTextChanged (fun str ->
                                dispatch <| TerminusBChanged str
                            )
                        ]


                        Grid.create [
                            Grid.columnDefinitions "*, *"
                            Grid.rowDefinitions "*, *, *, *"
                            Grid.maxWidth maxWidth
                            Grid.children [
                                // Line type
                                TextBlock.create [
                                    TextBlock.text "Vonal típusa:"
                                    TextBlock.row 0
                                    TextBlock.column 0
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                ComboBox.create [
                                    ComboBox.horizontalAlignment HorizontalAlignment.Right
                                    ComboBox.dataItems [
                                        Database.LineType.Bus
                                        Database.LineType.Express
                                        Database.LineType.Night
                                        Database.LineType.Tram
                                        Database.LineType.Trolley
                                    ]
                                    ComboBox.selectedItem state.lineType
                                    ComboBox.onSelectedItemChanged (tryUnbox >> Option.iter(LineTypeChanged >> dispatch))
                                    ComboBox.row 0
                                    ComboBox.column 1
                                    ComboBox.margin (0, 0, 0, 10)
                                ]

                                // Sign type
                                TextBlock.create [
                                    TextBlock.text "Tábla típusa:"
                                    TextBlock.row 1
                                    TextBlock.column 0
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                ComboBox.create [
                                    ComboBox.horizontalAlignment HorizontalAlignment.Right
                                    ComboBox.dataItems [
                                        Database.SignType.Újbalos
                                        Database.SignType.Vonalcsíkos
                                        Database.SignType.Sötétkék
                                        Database.SignType.Matricás
                                        Database.SignType.Hullámlogós
                                        Database.SignType.Tojáslogós
                                        Database.SignType.Balos
                                        Database.SignType.Duplaoldalas
                                        Database.SignType.Egyéb
                                    ]
                                    ComboBox.selectedItem state.signType
                                    ComboBox.onSelectedItemChanged (tryUnbox >> Option.iter(SignTypeChanged >> dispatch))
                                    ComboBox.row 1
                                    ComboBox.column 1
                                    ComboBox.margin (0, 0, 0, 10)
                                ]

                                //Date made
                                Grid.create [
                                    Grid.row 2
                                    Grid.column 0
                                    Grid.columnDefinitions "Auto, Auto"
                                    Grid.verticalAlignment VerticalAlignment.Center
                                    Grid.children [
                                        CheckBox.create [
                                            CheckBox.column 0
                                            CheckBox.isChecked state.hasDateMade
                                            CheckBox.onClick (fun _ -> dispatch HasDateMadeChanged)
                                        ]
                                        TextBlock.create [
                                            TextBlock.text "Gyártás dátuma:"
                                            TextBlock.column 1
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                        ]
                                    ]
                                ]
                                CalendarDatePicker.create [
                                    CalendarDatePicker.horizontalAlignment HorizontalAlignment.Right
                                    if state.hasDateMade then
                                        let date = 
                                            match state.dateMade with
                                            | Some dt -> dt.ToDateTime System.TimeOnly.MinValue
                                            | None -> System.DateTime.Today
                                        CalendarDatePicker.displayDate date
                                    else
                                        CalendarDatePicker.isEnabled false
                                    CalendarDatePicker.onSelectedDateChanged (fun dt -> dispatch <| DateMadeChanged dt)
                                    CalendarDatePicker.row 2
                                    CalendarDatePicker.column 1
                                    CalendarDatePicker.margin (0, 0, 0, 10)
                                ]

                                // Date got
                                TextBlock.create [
                                    TextBlock.text "Megszerzés dátuma:"
                                    TextBlock.row 3
                                    TextBlock.column 0
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                CalendarDatePicker.create [
                                    CalendarDatePicker.horizontalAlignment HorizontalAlignment.Right
                                    CalendarDatePicker.displayDate <| state.dateGot.ToDateTime System.TimeOnly.MinValue
                                    CalendarDatePicker.onSelectedDateChanged (fun dt -> dispatch <| DateGotChanged dt)
                                    CalendarDatePicker.row 3
                                    CalendarDatePicker.column 1
                                ]
                            ]
                        ]

                        // Price
                        TextBox.create [
                            TextBox.text <| sprintf "%.0f" state.price
                            TextBox.maxWidth maxWidth
                            TextBox.onTextChanged (fun str ->
                                let num = try decimal str with ex -> 0M
                                dispatch (PriceChanged <| decimal num)
                            )
                        ]

                        // Worth
                        TextBox.create [
                            TextBox.text <| sprintf "%.0f" state.worth
                            TextBox.maxWidth maxWidth
                            TextBox.onTextChanged (fun str ->
                                let num = try decimal str with ex -> 0M
                                dispatch (WorthChanged <| num)
                            )
                        ]

                        // Preview
                        Viewbox.create [
                            Viewbox.stretch Media.Stretch.Fill
                            Viewbox.maxWidth <| maxWidth * 1.5
                            Viewbox.child (
                                SignExtension.create [
                                    SignExtension.sign {
                                        Id = None
                                        Line = Some state.lineNum
                                        TerminusA = Some state.terminusA
                                        TerminusB = Some state.terminusB
                                        LineType = state.lineType
                                        SignType = state.signType
                                        DateMade = state.dateMade
                                        DateGot = state.dateGot
                                        Price = state.price
                                        Worth = state.worth
                                        IsSold = false
                                        DateSold = None
                                    }
                                ]
                            )
                        ]
                    ]
                ]
            )
        ]
