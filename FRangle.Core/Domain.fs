namespace FRangle.Core.Domain

module Errors =


    /// A general http error, this indicates the request could not be completed.
    type HttpError = { Message: string; Exception: string }

    /// A http error response, this indicates the request was completed by returned an error status code.
    type HttpResponseError = { Code: int; Message: string }

    type XmlParsingError = { Message: string }

    type JsonParsingError = { Message: string; Exception: string }

    type FailedCheck = { Message: string }

    type ProcessError = { Errors: string list }

    type FileError =
        | Argument of string
        | ArgumentNull of string
        | PathTooLong of string
        | DirectoryNotFound of string
        | UnauthorizedAccess of string
        | FileNotFound of string
        | NotSupported of string
        | IO of string

    type FRangleError =
        | FileError of FileError
        | HttpError of HttpError
        | HttpResponseError of HttpResponseError
        | JsonParsingError of JsonParsingError
        | XmlParsingError of XmlParsingError
        | FailedCheck of FailedCheck
        | UnhandledException of string
        | ProcessError of ProcessError
        | PipelineNotFound of string
        | BespokeError of string