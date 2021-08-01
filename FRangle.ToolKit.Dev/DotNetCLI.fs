namespace FRangle.ToolKit.Dev

open System
open System.Text.Json
open System.Text.Json.Serialization
open FRangle.Core
open FRangle.Core.Pipelines

module DotNetCLI =

    type Config =
        { [<JsonPropertyName("dotNetPath")>]
          DotNetPath: string }

    type BuildConfig =
        { Configuration: string option
          Framework: string option
          Force: bool
          Interactive: bool
          NoDependencies: bool
          NoIncremental: bool
          NoRestore: bool
          NoLogo: bool
          Output: string option
          Runtime: string option
          Source: string option
          Verbosity: string option
          VersionSuffix: string option }
        member config.GetCommand() =

            let ifNone defaultValue (input: string option) =
                input
                |> Option.defaultWith (fun _ -> defaultValue)

            let makeArg (template: string) (input: string option) =
                match input with
                | Some text -> $"{template} {text} "
                | None -> ""

            let ifTrue (output: string) (value: bool) =
                match value with
                | true -> $"{output} "
                | false -> ""

            let blank = ifNone ""

            String.Join(
                "",
                [ "build "
                  config.Configuration |> makeArg "--configuration"
                  config.Framework |> makeArg "--framework"
                  config.Force |> ifTrue "--force"
                  config.Interactive |> ifTrue "--interactive"
                  config.NoDependencies
                  |> ifTrue "--no-dependencies"
                  config.NoIncremental |> ifTrue "--no-incremental"
                  config.NoRestore |> ifTrue "--no-restore"
                  config.NoLogo |> ifTrue "--nologo"
                  config.Output |> makeArg "--output"
                  config.Runtime |> makeArg "--runtime"
                  config.Source |> makeArg "--source"
                  config.Verbosity |> makeArg "--verbosity"
                  config.VersionSuffix |> makeArg "--version-suffix" ]
            )

    let createConfig dotNetPath =
        create (fun _ -> { DotNetPath = dotNetPath })

    /// Build a .net project with the CLI.
    /// This effectively run `[dotNetPath] build -o outputDir` in the `startDir` directory.
    let build config (outputDir: string) startDir _ =
        Processes.run (
            { Name = config.DotNetPath
              Args = $"build -o {outputDir}"
              StartDirectory = Some startDir }: Processes.ProcessParameters
        )

    let buildWithConfig config (buildConfig: BuildConfig) startDir _ =
        Processes.run (
            { Name = config.DotNetPath
              Args = buildConfig.GetCommand()
              StartDirectory = Some startDir }: Processes.ProcessParameters
        )
