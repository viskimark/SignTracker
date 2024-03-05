open SignTracker.MultiPlatform.Database

let testSign: Sign = {
    Id = None
    Line = Some "3"
    TerminusA = Some "Gubacsi út / Határ út"
    TerminusB = Some "Mexikói út M"
    LineType = LineType.Tram
    SignType = SignType.Újbalos
    DateMade = Some <| System.DateOnly.Parse "2020-01-01"
    DateGot = System.DateOnly.Parse "2022-01-01"
    Price = 4000M
    Worth = 5000M
    IsSold = false
    DateSold = None
}
addSign testSign 1
