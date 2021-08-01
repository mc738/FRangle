# FRangle

`FRangle` is a library for creating pipelines.
It comes with a set of operations for common tasks like calling Http endpoints, IO and ETL functions.

There are also tool kits included for specific work flows.

The aim is to make it easy to chain these operations (and user defined ones) together.
This following the `unix` philosophy and is heavily influenced by `railway orientated programming`.
 
## Infixes

* `>>=` - Bind.
* `>=>` - Pipe.
* `>->` - Pass through.
* `>?>` - Recover.

## Why so many `unit`/`_`/`()`/`'a`'s?

The use of `_` as the last parameter in operations is to help with making the operation reliant on a previous one completing.

It will show it's self as `'a` types and will be handled when the pipeline is executed.

This is also way to start a pipeline with a `unit`. `start`, `startWith<'T>` and `create<'T>` all follow this pattern for example. 

## Core

The core library contains the base pipeline functions and specific modules for common tasks involving:

* `Xml`
* `Json`
* `File IO`
* `Http`
* `Processes`  
* ... and other (hopefully) useful stuff.

## Tool kits

Tool kits offer more operations for specific workflows, but can easily be left out if not need.

They are mainly a way of grouping related operations and their dependencies together into on namespace.

As such the should try and minimize the reliance on external libraries (out side of `System` ones) to consumers.

### Data tool kit

[To come]

### Developer tool kit

The developer tool kit (`FRangle.ToolKit.Dev`) contains operations specifically useful for developer work flows.

[FRangle.ToolKit.Dev README.md](./FRangle.Toolkit.Dev/README.md)

[More to come]

## App

[To come]

## Examples

### Build FRangle with .net CLI

The following example shows how to create 2 directories and build `FRangle.Core` and `FRangle.App` to them.

```f#
let printResult result =
    match result with
    | Ok v -> printfn $"%A{v}" 
    | Error e -> printfn $"%A{e}"

let runAndDiscard =
    Processes.run
    >-> printResult
    >=> discard

/// Callable with build "path" ()
let build (fRanglePath: string) =
    create (fun _ -> [ "C:\\Test\\App"; "C:\\Test\\Core" ]) 
    >=> Tools.mkdir
    >=> create (fun _ -> { Name = "C:\\Program Files\\dotnet\\dotnet"; Args = "build -o C:\\Test\\Core"; StartDirectory = Some $"{fRanglePath}\\FRangle.Core" } : Processes.ProcessParameters)
    >=> runAndDiscard
    >=> create (fun _ -> { Name = "C:\\Program Files\\dotnet\\dotnet"; Args = "build -o C:\\Test\\App"; StartDirectory = Some $"{fRanglePath}\\FRangle.App" } : Processes.ProcessParameters)
    >=> runAndDiscard
    >=> create (fun _ -> "Pipeline complete!")
```