// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.Net.Http
open System.Reflection.Emit
open FRangle.Core
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines
open FRangle.ToolKit.Dev

// Define a function to construct a message to print
let from whom = sprintf "from %s" whom

let printResult result =
    match result with
    | Ok v -> printfn $"%A{v}" 
    | Error e -> printfn $"%A{e}"

let runAndDiscard =
    Processes.run
    >-> printResult
    >=> discard

let buildFRangleTest =
    create (fun _ -> [ "C:\\Test\\App"; "C:\\Test\\Core" ]) 
    >=> Tools.mkdir
    >=> create (fun _ -> { Name = "C:\\Program Files\\dotnet\\dotnet"; Args = "build -o C:\\Test\\Core"; StartDirectory = Some "C:\\Users\\44748\\Projects\\FRangle\\FRangle.Core" } : Processes.ProcessParameters)
    >=> runAndDiscard
    >=> create (fun _ -> { Name = "C:\\Program Files\\dotnet\\dotnet"; Args = "build -o C:\\Test\\App"; StartDirectory = Some "C:\\Users\\44748\\Projects\\FRangle\\FRangle.App" } : Processes.ProcessParameters)
    >=> runAndDiscard
    >=> create (fun _ -> "Pipeline complete!")
    
let getDotNetGitIgnore _ =    
    let timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
    use client = new HttpClient()
    
    // inner function used to stop client from disposing of itself    
    let pipeline =
        create (fun _ -> ()) // A bit of a hack, start was being a pain.
        >=> Http.getString "https://raw.githubusercontent.com/dotnet/core/main/.gitignore" client
        >-> printResult
        >=> Files.write $"C:\\Users\\44748\\Projects\\__Resources\\dot-net-git-ignore_{timestamp}.txt"
        >=> create (fun _ -> ".net git ignore saved!")
        //>=> Http.getString "https://raw.githubusercontent.com/dotnet/core/main/.gitignore" client
        //>=> Tools.create (fun _ -> "Pipeline complete!")
    
    pipeline ()
    
let buildLinuxApp: unit -> Result<string, FRangleError> =
    let config = ({
        DotNetPath = "C:\\Program Files\\dotnet\\dotnet"
    } : DotNetCLI.Config)
    
    let buildConfig = ({
        Configuration = Some "Release"
        Framework = None
        Force = false
        Interactive = false
        NoDependencies = false
        NoIncremental = false
        NoRestore = false
        NoLogo = false
        Output = Some "\\\\wsl$\\Ubuntu-20.04\\home\\max\\apps\\dummy"
        Runtime = Some "linux-x64"
        Source = None
        Verbosity = None
        VersionSuffix = None
        
    } : DotNetCLI.BuildConfig)
    
    DotNetCLI.buildWithConfig config buildConfig "C:\\Users\\44748\\Projects\\DummyApp\\DummyApp"
    >=> Strings.concat Environment.NewLine
    
let getOutstandingDevopsItems _ =
    AzureDevOps.getOpenWorkItems "" "" ""
    //>=> (fun i -> pr Ok i)

let grepTest =
    create (fun _ -> [
        "* Get this."
        "Not this"
        "* and this"
    ])
    >=> Tools.grep "^\*."
    >=> Tools.concat Environment.NewLine
    
let stub =
    create (fun _ -> "Stub pipeline.")
    >-> printResult
    
let mergeTest =
    
    let createInt (value: int) = create (fun _ -> value)
    
    //createInt
    //>+> createInt 2
    let sum (a: int) (b: int) = a + b
    
    Ints.create 1
    >+> Ints.create 2
    >=> merge2 sum
    >=> (fun v -> Ok (v.ToString()))
    
let guidParseTest : unit -> Result<string, FRangleError> =
    Guids.create
    >=> (fun v -> Ok (v.ToString()))
    >-> printResult
    >=> Guids.parse
    >=> (fun v -> Ok (v.ToString()))
    
/// An example pipeline collection.
let pipelines = PipelineCollection<unit, string>.Create([
    "get-git-ignore", getDotNetGitIgnore
    "build-FRangle", buildFRangleTest
    "grep-test", grepTest
    "sum", mergeTest
    "guid", guidParseTest
    "linux", buildLinuxApp
    "stub", stub
]) 
   
[<EntryPoint>]
let main argv =
    //match buildFRangleTest () with
    //| Ok msg -> printfn $"Success. {msg}"
    //| Error e -> printfn $"Error! %A{e}"
    
    match pipelines.Run("linux", ()) with
    | Ok msg -> printfn $"Success. {msg}"
    | Error e -> printfn $"Error! %A{e}"
    
    (*
    let test =
        //Pipelines.start
        Tools.create (fun _ -> "")
        >=> Tools.transform (fun s -> 1)
        >=> Tools.check
                (fun v ->
                    if v = 2 then
                        Ok()
                    else
                        Error $"Value is not {2}")
        >-> (fun r ->
                match r with
                | Ok _ -> printfn "All is good!"
                | Error e -> printfn "Error: %A" e)
        >?> (fun _ -> printfn "Fixing error..."; Ok 2)
        >=> Tools.check
                (fun v ->
                    if v = 2 then
                        Ok()
                    else
                        Error $"Value is not {2}")
        >-> (fun r ->
                match r with
                | Ok _ -> printfn "All is good!"
                | Error e -> printfn "Error: %A" e)
    *)  
        
    //match test () with
    //| Ok r -> printfn "Success! %A" r
    //| Error e -> printfn "Error: %A" e

    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // return an integer exit code