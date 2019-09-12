// Learn more about F# at http://fsharp.org

open System
type ContactCard =  { 
    Name: string
    Phone: string
    Verified: bool
}
type ContactCardAlternate = { 
    Name     : string
    Phone    : string
    Address  : string
    Verified : bool 
}

type Address = Address of string
type Name = Name of string
type SSN2 = SSN of int
/// First, define a zip code defined via Single-case Discriminated Union.
type ZipCode = ZipCode of string

/// Next, define a type where the ZipCode is optional.
type Customer = { ZipCode: ZipCode option }

[<EntryPoint>]
let main argv =

    let mutable num1 = 3;
    num1 <- 4;

    printfn "%s\t%d\t%f" "ss" num1 0.33

    let b1 = not true;
    printfn "%b" b1
    

    let arr = [for i in 0..10 -> (i, i*i)]
    printfn "%A" arr
    
    let isOdd x = x % 2 <> 0
    let a = List.filter isOdd

    let s = Array.init 88 (fun i ->  "aa")

    let contact1 = { 
        Name = "Alf" 
        Phone = "(206) 555-0157" 
        Verified = false }
    let contact2 = { Name = "Alf"; Phone = "(206) 555-0157"; Verified = false; Address = "33"; }
    let showContactCard c = 
          c.Name + " Phone: " + c.Phone + (if not c.Verified then " (unverified)" else "")
    let c2 = showContactCard contact2

    
    // You can easily instantiate a single-case DU as follows.
    let address = Address "111 Alf Way"
    let name = Name "Alf"
    let ssn = SSN2.SSN 1234567890
    
    /// When you need the value, you can unwrap the underlying value with a simple function.
    let unwrapAddress (Address a) = a
    let unwrapName (Name n) = n
    let unwrapSSN (SSN s) = s

    0 // return an integer exit code