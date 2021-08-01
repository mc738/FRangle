# FRangle

`FRangle` is a library for creating pipelines.
It comes with a set of operations for common tasks like calling Http endpoints, IO and ETL functions.

There are also tool kits included for specific work flows.

The aim is to make it easy to chain these operations (and user defined ones) together.
This following the `unix` philosophy and is heavily influenced by `railway orientated programming`.
 
## What are pipelines (in this context)?

Pipelines are a series of operations that can that are dependent on the result of the previous operation
and can potential fail.

If an operation fails, following operations will (generally) not be run.

This means they are suited to tasks that required multiple independent steps to be performed in series.

They are not designed to be be a long running task, rather to simplify such work flows and error handling.

Also, they are reasonable stateless. An operation depends on the previous result, it is generally upto the user to handle state (see examples).

Due to the nature of operations they could be a few side effects (such as persisting data), the aim of the library is to wrap this up an present a semi FP way to handle it.

The benefit of this approach is that operations can be independently build, tested and chained together to offer a lot of flexibility. 

## Infixes

* `>>=` - Bind.
* `>=>` - Pipe.
* `>->` - Pass through.
* `>?>` - Recover.
* `>+>` - Combine.

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

## Comms tool kit

[To come]

### Data tool kit

[To come]

### Developer tool kit

The developer tool kit (`FRangle.ToolKit.Dev`) contains operations specifically useful for developer work flows.

[FRangle.ToolKit.Dev README.md](./FRangle.ToolKit.Dev/README.md)

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