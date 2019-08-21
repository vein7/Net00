// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    let mutable num1 = 3;
    num1 <- 4;


    printfn "Hello World from F#!"
    0 // return an integer exit code
