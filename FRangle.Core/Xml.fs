namespace FRangle.Core

open System.Xml.Linq
open FRangle.Core.Domain
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines

[<RequireQualifiedAccess>]
module Xml =
   
    /// Convert and xml string and an XDocument
    let toXDocument (xml: string) =
        try
            Ok (XDocument.Parse(xml))
        with 
        | ex -> Error (FRangleError.XmlParsingError { Message = ex.Message })
    
    /// Convert and xml string and an XElement
    let toXElement (xml: string) = toXDocument xml |> map (fun d -> d.Root)
    
    /// Deserialize a XElement to a 'T object via a handler function. 
    let deserialize<'T> (handler: XElement -> 'T) = switch handler 

    /// Parse and xml string into type 'T object via a handler function.
    /// 'T is normally a root object or collection.
    let parse<'T> handler =
        toXElement >=> deserialize<'T> handler
        
        
    