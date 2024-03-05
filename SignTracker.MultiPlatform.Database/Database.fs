namespace SignTracker.MultiPlatform

open System
open FSharp.Data.Sql

module Database =
    type LineType =
    | Bus = 0
    | Express = 4
    | Trolley = 1
    | Tram = 2
    | Night = 3

    type SignType =
    | Egyéb = 0
    | Duplaoldalas = 1
    | Balos = 2
    | Tojáslogós = 3
    | Hullámlogós = 4
    | Matricás = 5
    | Sötétkék = 6
    | Vonalcsíkos = 7
    | Újbalos = 8

    type Sign = {
        Id: int option
        Line: string option
        TerminusA: string option
        TerminusB: string option
        LineType: LineType
        SignType: SignType
        DateMade: DateOnly option
        DateGot: DateOnly
        Price: decimal
        Worth: decimal
        IsSold: bool
        DateSold: DateOnly option
    }

    type SqlConnection = SqlDataProvider<
                            Common.DatabaseProviderTypes.MYSQL,
                            "Server=localhost;Port=3306;Database=signtracker;User=dummy;Password=password",
                            UseOptionTypes = Common.NullableColumnType.OPTION>

    let ctx = SqlConnection.GetDataContext("connstr") //Connection string goes here - i'll make a file for it later
    let db = ctx.Signtracker
    
    let parseDateTime (dt: DateTime option) : DateOnly option =
        match dt with
        | Some dt -> dt |> DateOnly.FromDateTime |> Some
        | None -> None
    let parseDateOnly (dt: DateOnly option) : DateTime option =
        match dt with
        | Some dt -> dt.ToDateTime TimeOnly.MinValue |> Some
        | None -> None


    let getSigns userId =
        query {
            for sign in db.Signs do
            where (sign.UserId = userId)
            select sign
        }
        |> Seq.map (fun sign ->
            {
                Id = Some sign.Id
                Line = sign.Line
                TerminusA = sign.TerminusA
                TerminusB = sign.TerminusB
                LineType = enum sign.LineType
                SignType = enum sign.SignType
                DateMade = parseDateTime sign.DateMade
                DateGot = sign.DateGot |> DateOnly.FromDateTime
                Price = sign.Price
                Worth = sign.Worth
                IsSold = Convert.ToBoolean sign.IsSold
                DateSold = parseDateTime sign.DateSold
            }
        )

    let addSign (sign: Sign) userId =
        let newSign = db.Signs.Create()

        newSign.Line <- sign.Line
        newSign.TerminusA <- sign.TerminusA
        newSign.TerminusB <- sign.TerminusB
        newSign.LineType <- int sign.LineType
        newSign.SignType <- int sign.SignType
        newSign.DateMade <- parseDateOnly sign.DateMade
        newSign.DateGot <- sign.DateGot.ToDateTime TimeOnly.MinValue
        newSign.Price <- sign.Price
        newSign.Worth <- sign.Worth
        newSign.IsSold <- Convert.ToSByte false
        newSign.DateSold <- None
        newSign.UserId <- userId

        ctx.SubmitUpdates()

    let editSign (sign: Sign) =
        match sign.Id with
        | Some id ->
            query {
                for sign' in db.Signs do
                where (sign'.Id = id)
                select sign'
                exactlyOneOrDefault
            }
            |> fun sign' ->
                //sign'.Id <- id
                sign'.Line <- sign.Line
                sign'.TerminusA <- sign.TerminusA
                sign'.TerminusB <- sign.TerminusB
                sign'.LineType <- int sign.LineType
                sign'.SignType <- int sign.SignType
                sign'.DateMade <- parseDateOnly sign.DateMade
                sign'.DateGot <- sign.DateGot.ToDateTime TimeOnly.MinValue
                sign'.Price <- sign.Price
                sign'.Worth <- sign.Worth
                sign'.IsSold <- Convert.ToSByte false
                sign'.DateSold <- None

            ctx.SubmitUpdates()
        | None -> ()