namespace FRangle.Core

open System
open System.Runtime.InteropServices

module Utils =

    type OSType =
        | Linux
        | Windows
        | OSX
        | FreeBSD
        | Unknown
        static member Current =
            // A bit ugly but that this is mainly a helper.
            if RuntimeInformation.IsOSPlatform(OSPlatform.Linux) then
                OSType.Linux
            elif RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                OSType.Windows
            elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then
                OSType.OSX
            elif RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) then
                OSType.FreeBSD
            else
                OSType.Unknown
