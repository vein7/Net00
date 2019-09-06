// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    let mutable num1 = 3;
    num1 <- 4;

    printfn "%s\t%d\t%f" "ss" num1 0.33

    let b1 = not true;
    printfn "%b" b1
    

    let arr = [for i in 0..10 -> (i, i*i)]
    printfn "%A" arr



    0 // return an integer exit code
