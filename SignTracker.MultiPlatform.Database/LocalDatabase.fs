namespace SignTracker.MultiPlatform

open System
open Fumble
open FSharp.Data.Sql

module LocalDatabase =
    open Database

    let path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SignTracker\local.db"
    let connectionString = "Data source=" + path

    let init () =
        if not <| IO.File.Exists path then
            IO.Directory.CreateDirectory <| Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SignTracker" |> ignore

            connectionString
            |> Sql.connect
            |> Sql.query
                    "create table signs (
                            id          integer primary key autoincrement,
                            line        text,
                            terminusA   text,
                            terminusB   text,
                            lineType    integer,
                            signType    integer,
                            dateMade    text,
                            dateGot     text,
                            price       real,
                            worth       real,
                            isSold      boolean,
                            dateSold    text,
                            userId      integer
                        );"
            |> Sql.executeNonQuery
            |> ignore

    let parseDateTime (dt: DateTime option) : DateOnly option =
        match dt with
        | Some dt -> dt |> DateOnly.FromDateTime |> Some
        | None -> None
    let parseDateOnly (dt: DateOnly option) : DateTime option =
        match dt with
        | Some dt -> dt.ToDateTime TimeOnly.MinValue |> Some
        | None -> None

    let parseDateString str =
        match str with
        | "" -> None
        | _ -> str |> DateOnly.Parse |> Some

    let getSigns userId =
        connectionString
        |> Sql.connect
        |> Sql.query "select * from signs where userId = @userId"
        |> Sql.parameters [ "@userId", Sql.int userId ]
        |> Sql.execute (fun read ->
            {
                Id = Some <| read.int "id"
                Line = read.stringOrNone "line"
                TerminusA = read.stringOrNone "terminusA"
                TerminusB = read.stringOrNone "terminusB"
                LineType = enum <| read.int "lineType"
                SignType = enum <| read.int "signType"
                DateMade = parseDateTime <| read.dateTimeOrNone "dateMade"
                DateGot = read.string "dateGot" |> DateTime.Parse |> DateOnly.FromDateTime
                Price = read.decimal "price"
                Worth = read.decimal "worth"
                IsSold = read.bool "isSold"
                DateSold = parseDateTime <| read.dateTimeOrNone "dateSold"
            }
        )
        |> fun res ->
            match res with
            | Ok list -> List.toSeq list
            | Error e -> failwith e.Message

    let addSign (sign: Sign) userId =
        connectionString
        |> Sql.connect
        |> Sql.query
            "
                insert into signs (line, terminusA, terminusB, lineType, signType, dateMade, dateGot, price, worth, isSold, dateSold, userId)
                values(
                    @line,
                    @terminusA,
                    @terminusB,
                    @lineType,
                    @signType,
                    @dateMade,
                    @dateGot,
                    @price,
                    @worth,
                    @isSold,
                    @dateSold,
                    @userId
                );
            "
        |> Sql.parameters [
            "@line", Sql.stringOrNone sign.Line
            "@terminusA", Sql.stringOrNone sign.TerminusA
            "@terminusB", Sql.stringOrNone sign.TerminusB
            "@lineType", Sql.int <| int sign.LineType
            "@signType", Sql.int <| int sign.SignType
            "@dateMade", Sql.dateTimeOrNone <| parseDateOnly sign.DateMade
            "@dateGot", Sql.dateTime <| sign.DateGot.ToDateTime(TimeOnly.MinValue)
            "@price", Sql.decimal sign.Price
            "@worth", Sql.decimal sign.Worth
            "@isSold", Sql.bool sign.IsSold
            "@dateSold", Sql.dateTimeOrNone <| parseDateOnly sign.DateSold
            "@userId", Sql.int userId
        ]
        |> Sql.executeNonQuery
        |> fun res ->
            match res with
            | Ok _ -> ()
            | Error e -> failwith e.Message

    let saveSigns () =
        ()

    //let editSign (sign: Sign) =
    //    match sign.Id with
    //    | Some id ->
    //        query {
    //            for sign' in db.Signs do
    //            where (sign'.Id = id)
    //            select sign'
    //            exactlyOneOrDefault
    //        }
    //        |> fun sign' ->
    //            //sign'.Id <- id
    //            sign'.Line <- sign.Line
    //            sign'.TerminusA <- sign.TerminusA
    //            sign'.TerminusB <- sign.TerminusB
    //            sign'.LineType <- int sign.LineType
    //            sign'.SignType <- int sign.SignType
    //            sign'.DateMade <- parseDateOnly sign.DateMade
    //            sign'.DateGot <- sign.DateGot.ToDateTime TimeOnly.MinValue
    //            sign'.Price <- sign.Price
    //            sign'.Worth <- sign.Worth
    //            sign'.IsSold <- Convert.ToSByte false
    //            sign'.DateSold <- None

    //        ctx.SubmitUpdates()
    //    | None -> ()