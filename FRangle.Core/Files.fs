namespace FRangle.Core

open System
open System.IO
open FRangle.Core.Domain.Errors

[<RequireQualifiedAccess>]
module Files =
    
    let readAsStream path =
        try
            Ok (File.OpenRead(path) :> Stream)
        with
        | :? ArgumentException as ex -> Error (FRangleError.FileError (FileError.Argument ex.Message))
        | :? ArgumentNullException as ex -> Error (FRangleError.FileError (FileError.ArgumentNull ex.Message))
        | :? PathTooLongException as ex -> Error (FRangleError.FileError (FileError.PathTooLong ex.Message))
        | :? DirectoryNotFoundException as ex -> Error (FRangleError.FileError (FileError.DirectoryNotFound ex.Message))
        | :? UnauthorizedAccessException as ex -> Error (FRangleError.FileError (FileError.UnauthorizedAccess ex.Message))
        | :? FileNotFoundException as ex -> Error (FRangleError.FileError (FileError.FileNotFound ex.Message))
        | :? NotSupportedException as ex -> Error (FRangleError.FileError (FileError.NotSupported ex.Message))
        | :? IOException as ex -> Error (FRangleError.FileError (FileError.IO ex.Message))
        | e -> Error (FRangleError.UnhandledException e.Message)
    
    let readAllBytes path =
        try
            Ok (File.ReadAllBytes(path))
        with
        | :? ArgumentException as ex -> Error (FRangleError.FileError (FileError.Argument ex.Message))
        | :? ArgumentNullException as ex -> Error (FRangleError.FileError (FileError.ArgumentNull ex.Message))
        | :? PathTooLongException as ex -> Error (FRangleError.FileError (FileError.PathTooLong ex.Message))
        | :? DirectoryNotFoundException as ex -> Error (FRangleError.FileError (FileError.DirectoryNotFound ex.Message))
        | :? UnauthorizedAccessException as ex -> Error (FRangleError.FileError (FileError.UnauthorizedAccess ex.Message))
        | :? FileNotFoundException as ex -> Error (FRangleError.FileError (FileError.FileNotFound ex.Message))
        | :? NotSupportedException as ex -> Error (FRangleError.FileError (FileError.NotSupported ex.Message))
        | :? IOException as ex -> Error (FRangleError.FileError (FileError.IO ex.Message))
        | e -> Error (FRangleError.UnhandledException e.Message)
        
    let readAllLines path =
        try
            Ok (File.ReadAllLines(path))
        with
        | :? ArgumentException as ex -> Error (FRangleError.FileError (FileError.Argument ex.Message))
        | :? ArgumentNullException as ex -> Error (FRangleError.FileError (FileError.ArgumentNull ex.Message))
        | :? PathTooLongException as ex -> Error (FRangleError.FileError (FileError.PathTooLong ex.Message))
        | :? DirectoryNotFoundException as ex -> Error (FRangleError.FileError (FileError.DirectoryNotFound ex.Message))
        | :? UnauthorizedAccessException as ex -> Error (FRangleError.FileError (FileError.UnauthorizedAccess ex.Message))
        | :? FileNotFoundException as ex -> Error (FRangleError.FileError (FileError.FileNotFound ex.Message))
        | :? NotSupportedException as ex -> Error (FRangleError.FileError (FileError.NotSupported ex.Message))
        | :? IOException as ex -> Error (FRangleError.FileError (FileError.IO ex.Message))
        | e -> Error (FRangleError.UnhandledException e.Message)
          
    let read path =
        try
            Ok (File.ReadAllText(path))
        with
        | :? ArgumentException as ex -> Error (FRangleError.FileError (FileError.Argument ex.Message))
        | :? ArgumentNullException as ex -> Error (FRangleError.FileError (FileError.ArgumentNull ex.Message))
        | :? PathTooLongException as ex -> Error (FRangleError.FileError (FileError.PathTooLong ex.Message))
        | :? DirectoryNotFoundException as ex -> Error (FRangleError.FileError (FileError.DirectoryNotFound ex.Message))
        | :? UnauthorizedAccessException as ex -> Error (FRangleError.FileError (FileError.UnauthorizedAccess ex.Message))
        | :? FileNotFoundException as ex -> Error (FRangleError.FileError (FileError.FileNotFound ex.Message))
        | :? NotSupportedException as ex -> Error (FRangleError.FileError (FileError.NotSupported ex.Message))
        | :? IOException as ex -> Error (FRangleError.FileError (FileError.IO ex.Message))
        | e -> Error (FRangleError.UnhandledException e.Message)
        
    /// Write text to a file using `System.File.WriteAllText()`. 
    let write path content =
        try
            Ok(File.WriteAllText(path, content))
        with //TODO check error handling.
        | :? ArgumentException as ex -> Error (FRangleError.FileError (FileError.Argument ex.Message))
        | :? ArgumentNullException as ex -> Error (FRangleError.FileError (FileError.ArgumentNull ex.Message))
        | :? PathTooLongException as ex -> Error (FRangleError.FileError (FileError.PathTooLong ex.Message))
        | :? DirectoryNotFoundException as ex -> Error (FRangleError.FileError (FileError.DirectoryNotFound ex.Message))
        | :? UnauthorizedAccessException as ex -> Error (FRangleError.FileError (FileError.UnauthorizedAccess ex.Message))
        | :? FileNotFoundException as ex -> Error (FRangleError.FileError (FileError.FileNotFound ex.Message))
        | :? NotSupportedException as ex -> Error (FRangleError.FileError (FileError.NotSupported ex.Message))
        | :? IOException as ex -> Error (FRangleError.FileError (FileError.IO ex.Message))
        | e -> Error (FRangleError.UnhandledException e.Message)
        