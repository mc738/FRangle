namespace FRangle.ToolKit.Dev

open System
open FRangle.Core

module Git =
        
    [<RequireQualifiedAccess>]
    module Git =
        
        /// Get the latest commit hash for a path via `git rev-parse HEAD` 
        let latestCommitHash path =
            let output, errors = Processes.execute "git" "rev-parse HEAD" path
            match errors.Length = 0 with
            | true -> Ok output.Head
            | false -> Error (String.Join(Environment.NewLine, errors))