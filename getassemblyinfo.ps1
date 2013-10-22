# Copyright 2013 Peter Rekdal Sunde <peter.sunde@gmail.com> 
#
# MIT License
#
# References:
# ==========
# http://apichange.codeplex.com/SourceControl/changeset/view/76c98b8c7311#ApiChange.Api/src/Introspection/CorFlagsReader.cs
# https://github.com/jbevain/cecil
# http://msdn.microsoft.com/library/windows/hardware/gg463125

function Write-Diagnostic {
    param(
        [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
        [string]$message
    )

    Write-Host
    Write-Host $message -ForegroundColor Green
    Write-Host
}

function Get-AssemblyInfo {
  param(
        [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
        [string] $filename
    )

    if(-not (Test-Path -Path $filename)) {
        Write-Error "Could not find file: $filename"
        exit 1
    }

    function AssemblyInfo {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [uint32] $peFormat = $null,
            [parameter(Position = 1, Mandatory = $false, ValueFromPipeline = $true)]
            [uint32] $attributes = $null,
            [parameter(Position = 2, Mandatory = $false, ValueFromPipeline = $true)]
            [uint32] $machine = $null,
            [parameter(Position = 3, Mandatory = $false, ValueFromPipeline = $true)]
            [uint16] $characteristics = $null,
            [parameter(Position = 4, Mandatory = $false, ValueFromPipeline = $true)]
            [object] $optionalHeaders = $null,
            [parameter(Position = 5, Mandatory = $false, ValueFromPipeline = $true)]
            [uint32] $majorRuntimeVersion = $null,
            [parameter(Position = 6, Mandatory = $false, ValueFromPipeline = $true)]
            [uint32] $minorRuntimeVersion = $null,
            [parameter(Position = 7, Mandatory = $false, ValueFromPipeline = $true)]
            [string] $targetFramework = $null
        )

        $assemblyInfo = @{}

        # Major/minor
        $assemblyInfo.Filename = $filename
        $assemblyInfo.MajorRuntimeVersion = $majorRuntimeVersion
        $assemblyInfo.MinorRuntimeVersion = $minorRuntimeVersion
        $assemblyInfo.TargetFramework = $targetFramework
        $assemblyInfo.ModuleKind = GetModuleKind -characteristics $characteristics -subSystem $optionalHeaders.SubSystem
        $assemblyInfo.ModuleCharacteristics = GetModuleCharacteristics -characteristics $characteristics
        $assemblyInfo.ModuleAttributes = GetModuleAttributes -attributes $attributes

        ## PeFormat
        if($peFormat -eq 0x20b) { 
            $assemblyInfo.PEFormat = "PE32Plus" 
        } elseif($peFormat -eq 0x10b) { 
            $assemblyInfo.PEFormat = "PE32" 
        }

        ## ProcessorArchitecture
        $assemblyInfo.ProcessorArchitecture = "Unknown"

        switch -Exact ($machine) {
            0x014c {
                $assemblyInfo.ProcessorArchitecture = "x86"
                if($assemblyInfo.ModuleAttributes -contains "ILOnly") {
                    $assemblyInfo.ProcessorArchitecture = "AnyCpu"
                }
            }
            0x8664 {
                $assemblyInfo.ProcessorArchitecture = "x64" 
            }
            0x0200 {
                $assemblyInfo.ProcessorArchitecture = "IA64"
            }
            0x01c4 {
                $assemblyInfo.ProcessorArchitecture = "ARMv7" 
            }
            default {
                if($assemblyInfo.PEFormat -eq "PE32PLUS") {
                    $assemblyInfo.ProcessorArchitecture = "x64"
                } elseif($assemblyInfo.PEFormat -eq "PE32") {
                    $assemblyInfo.ProcessorArchitecture = "x86"
                }
            }
        }

        return New-Object -TypeName PSObject -Property $assemblyInfo
    }

    function GetModuleKind {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [uint32] $characteristics,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [uint32] $subSystem
        )

        # ImageCharacteristics.Dll
        if($characteristics -eq ($characteristics -bor 0x2000)) {
            return "Dll"
        }

        # SubSystem.WindowsGui || SubSystem.WindowsCeGui
        if($subSystem -eq 0x2 -or $subSystem -eq 0x9) {
            return "WinExe"
        }

        return "Console"
    }

    function GetModuleCharacteristics {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [uint16] $characteristics
        )

        $moduleCharacteristics = @()

        if($characteristics -eq ($characteristics -bor 0x0020)) {
            $moduleCharacteristics += "HighEntropyVA"
        }

        if($characteristics -eq ($characteristics -bor 0x0040)) {
           $moduleCharacteristics += "DynamicBase"
        }

        if($characteristics -eq ($characteristics -bor 0x0400)) {
            $moduleCharacteristics += "NoSEH"
        }

        if($characteristics -eq ($characteristics -bor 0x0100)) {
           $moduleCharacteristics += "NXCompat"
        }

        if($characteristics -eq ($characteristics -bor 0x1000)) {
            $moduleCharacteristics += "AppContainer"
        }

        if($characteristics -eq ($characteristics -bor 0x8000)) {
            $moduleCharacteristics += "TerminalServerAware"
        }

        return $moduleCharacteristics
    }

    function GetModuleAttributes {
        param(
            [parameter(Position = 0, Mandatory = $false, ValueFromPipeline = $true)]
            [uint32] $attributes = $null
        )

        $moduleAttributes = @()

        if($attributes -eq ($attributes -bor 0x1)) {
            $moduleAttributes += "ILOnly"
        }

        if($attributes -eq ($attributes -bor 0x2)) {
            $moduleAttributes += "Required32Bit"
        }

        if($attributes -eq ($attributes -bor 0x8)) {
            $moduleAttributes += "StrongNameSigned"
        }

        if($attributes -eq ($attributes -bor 0x00020000)) {
            $moduleAttributes += "Preferred32Bit"
        }

        return $moduleAttributes

    }

    function Advance {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.Stream] $stream,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [int] $count
        )

        $stream.Seek($count, [System.IO.SeekOrigin]::Current) | Out-Null
    }

    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L238
    function ReadZeroTerminatedString {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.BinaryReader] $binaryReader,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [int] $length
        )

        $read = 0
        $buffer = New-Object char[] $length
        $bytes = $binaryReader.ReadBytes($length)
        while($read -lt $length) {
            $current = $bytes[$read]
            if($current -eq 0) {
                break
            }        
            
            $buffer[$read++] = $current
        }

        return New-Object string ($buffer, 0, $read)
    }
    
    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/Image.cs#L98
    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/Image.cs#L107
    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/Image.cs#L124
    function ResolveVirtualAddress {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [uint32] $rva,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [object[]] $sections
        )

        $section = $null

        $sections | ForEach-Object {
            if($rva -ge $_.VirtualAddress -and $rva -lt $_.VirtualAddress + $_.SizeOfRawData) {
                $section = $_
                return
            }
        }

        if($section -eq $null) {
            Write-Error "Unable to resolve virtual address for rva address: " $rva
            exit 1
        }

        return [System.Convert]::ToUInt32($rva + $section.PointerToRawData - $section.VirtualAddress)
    }

    # # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/Image.cs#L53
    function MoveTo {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.Stream] $stream,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [object] $dataDirectory,
            [parameter(Position = 2, Mandatory = $true, ValueFromPipeline = $true)]
            [object] $sections
        )

        $stream.Position = ResolveVirtualAddress -rva ([uint32] $dataDirectory.VirtualAddress) -sections $sections
     }

    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/BinaryStreamReader.cs#L46
    function ReadDataDirectory {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.BinaryReader] $binaryReader
        )

        $dataDirectory = @{}
        $dataDirectory.VirtualAddress = $binaryReader.ReadUInt32()
        $dataDirectory.VirtualSize = $binaryReader.ReadUInt32() 
        $dataDirectory.IsZero = $dataDirectory.VirtualAddress -eq 0 -and $dataDirectory.VirtualSize -eq 0

        return New-Object -TypeName PSObject -Property $dataDirectory
    }

    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L140
    function ReadOptionalHeaders {
        param(
           [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
           [System.IO.BinaryReader] $binaryReader,
           [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
           [System.IO.Stream] $stream,
           [parameter(Position = 2, Mandatory = $true, ValueFromPipeline = $true)]
           [uint16] $peFormat
        )

        $optionalHeaders = @{}
        $optionalHeaders.PEFormat = $peFormat
        $optionalHeaders.SubSystem = $null
        $optionalHeaders.SubSystemMajor = $null
        $optionalHeaders.SubSystemMinor = $null
        $optionalHeaders.Characteristics = $null
        $optionalHeaders.CLIHeader = $null
        $optionalHeaders.Debug = $null

        # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L136
        Advance -stream $stream -count 44

        # SubSysMajor 2
        # SubSystemMinor 2
        $optionalHeaders.SubSystemMajor = $binaryReader.ReadUInt16()
        $optionalHeaders.SubSystemMinor = $binaryReader.ReadUInt16()

        Advance -stream $stream -count 18

        # SubSystem 2
        $optionalHeaders.SubSystem = $binaryReader.ReadUInt16()

        # DLLFlags
        $optionalHeaders.Characteristics = $binaryReader.ReadUInt16()

        # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L197
        if($peFormat -eq 0x20b) { 
            Advance -stream $stream -count 88  
        } else { 
            Advance -stream $stream -count 72 
        }
        # Debug 8
        $optionalHeaders.Debug = ReadDataDirectory -binaryReader $binaryReader

        # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L209
        Advance -stream $stream -count 56 

        # CLIHeader
        $optionalHeaders.CLIHeader = ReadDataDirectory -binaryReader $binaryReader

        # Reserved 8
        Advance -stream $stream -count 8 

        return New-Object -TypeName PSObject -Property $optionalHeaders
    }

    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/BinaryStreamReader.cs#L48
    function ReadSections {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.BinaryReader] $binaryReader,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.Stream] $stream,
            [parameter(Position = 2, Mandatory = $true, ValueFromPipeline = $true)]
            [uint16] $count
        )

        # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L289
        function ReadSection {
            param(
                [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
                [System.IO.Stream] $stream,
                [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
                [object] $section
            )
            
            # Save current position
            $position = $stream.Position

            # Move to pointer
            $stream.Position = $section.PointerToRawData

            # Reader pointer value
            $length = [System.Convert]::ToInt32($section.SizeOfRawData)
            $data = New-Object byte[] $length
            $offset = 0
            $read = 0

            while (($read = $stream.Read($data, $offset, $length - $offset)) -gt 0) {
                $offset += $read
            }

            $section.Data = $data

            # Restore old position
            $stream.Position = $position

            return $section

        }

        $sections = New-Object object[] $count

        for($i = 0; $i -lt $count; $i++) {

            $section = @{}

            # Name
            $section.Name = ReadZeroTerminatedString -binaryReader $reader -length 8

            # Data
            $section.Data = $null

            # VirtualSize 4
            Advance -stream $stream -count 4

            # VirtualAddress	4
            $section.VirtualAddress = $binaryReader.ReadUInt32()

            # SizeOfRawData	4
            $section.SizeOfRawData = $binaryReader.ReadUInt32()

            # PointerToRawData	4
            $section.PointerToRawData = $binaryReader.ReadUInt32()

            # PointerToRelocations		4
            # PointerToLineNumbers		4
            # NumberOfRelocations		2
            # NumberOfLineNumbers		2
            # Characteristics			4
            Advance -stream $stream -count 16 

            # Read section data
            $section = (ReadSection -stream $stream -section $section)

            # Add section
            $sections[$i] = New-Object -TypeName PSObject -Property $section

        }

        return $sections

    }

    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L307
    function ReadCLIHeader {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.BinaryReader] $binaryReader,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.Stream] $stream,
            [parameter(Position = 2, Mandatory = $true, ValueFromPipeline = $true)]
            [object] $dataDirectory,
            [parameter(Position = 3, Mandatory = $true, ValueFromPipeline = $true)]
            [object] $sections
        )

        MoveTo -stream $stream -dataDirectory $dataDirectory -sections $sections

        # 4 because of major/minor
        Advance -stream $stream -count 4

        $cliHeader = @{}
        $cliHeader.MajorRuntimeVersion = $binaryReader.ReadUInt16()
        $cliHeader.MinorRuntimeVersion = $binaryReader.ReadUInt16()
        $cliHeader.Metadata = ReadDataDirectory -binaryReader $binaryReader 
        $cliHeader.Attributes = $binaryReader.ReadUInt32()
        $cliHeader.EntryPointToken = $binaryReader.ReadUInt32()
        $cliHeader.Resources = ReadDataDirectory -binaryReader $binaryReader 
        $cliHeader.StrongName = ReadDataDirectory -binaryReader $binaryReader 
    
        return New-Object -TypeName PSObject -Property $cliHeader
    }

    # https://github.com/jbevain/cecil/blob/master/Mono.Cecil.PE/ImageReader.cs#L334
    function GetTargetFrameworkVersion {
        param(
            [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.BinaryReader] $binaryReader,
            [parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
            [System.IO.Stream] $stream,
            [parameter(Position = 2, Mandatory = $true, ValueFromPipeline = $true)]
            [object] $dataDirectory,
            [parameter(Position = 3, Mandatory = $true, ValueFromPipeline = $true)]
            [object] $sections,
            [parameter(Position = 4, Mandatory = $true, ValueFromPipeline = $true)]
            [object] $optionalHeaders   
       )


       $targetFramework = ""

       MoveTo -stream $stream -dataDirectory $dataDirectory -sections $sections

       if($binaryReader.ReadUInt32() -ne 0x424a5342) {
           Write-Error "BadImageFormat"
           exit 1
       }

       # 4 because of major/minor
       Advance -stream $stream -count 8

       # Read framework version
       $frameworkVersion = ReadZeroTerminatedString -binaryReader $binaryReader -length $binaryReader.ReadInt32()

       switch -Exact ($frameworkVersion[1]) {
            1 {
                if($frameworkVersion[3] -eq 0) { 
                    $targetFramework = "NET10" 
                } else { 
                    $targetFramework = "NET11"
                }
            }
            2 {
                $targetFramework = "NET20"
            }
            4 {
                if($optionalHeaders.SubSystemMinor -eq 0x6) {
                    $targetFramework = "NET45"
                } else {
                    $targetFramework = "NET40"
                }
            }
       }
        
       return $targetFramework
    }

    # Read assembly
    $stream = New-Object System.IO.FileStream($filename, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::Read)
    $reader = New-Object System.IO.BinaryReader($stream)
    $length = $stream.Length

    # Read PE format
    # ==============
    # The initial part here reads the PE format (not specific to .NET like cecil does)
    # because we are interested in determining generic PE metadata

    # Read pointer to PE header.
    $stream.Position = 0x3c
    $peHeaderPtr = $reader.ReadUInt32()
    if($peHeaderPtr -eq 0) {
        $peHeaderPtr = 0x80
    }

    # Ensure there is at least enough room for the following structures:
    #     24 byte PE Signature & Header
    #     28 byte Standard Fields         (24 bytes for PE32+)
    #    68 byte NT Fields               (88 bytes for PE32+)
    # >= 128 byte Data Dictionary Table
    if($peHeaderPtr > ($length - 256)) {
        Write-Error "Invalid PE header"
        exit 1
    }

    # Check the PE signature.  Should equal 'PE\0\0'.
    $stream.Position = $peHeaderPtr
    $peSignature = $reader.ReadUInt32()
    if ($peSignature -ne 0x00004550) {
        Write-Error "Invalid PE signature"
        exit 1
    }

    # Read PE header fields.
    $machine = $reader.ReadUInt16()
    $numberOfSections = $reader.ReadUInt16()

    Advance -stream $stream -count 14

    $characteristics = $reader.ReadUInt16()
    $peFormat = $reader.ReadUInt16()

    # Must be PE32 or PE32plus
    if ($peFormat -ne 0x10b -and $peFormat -ne 0x20b) {
        Write-Error "Invalid PE format. Must be either PE32 or PE32PLUS"
        exit 1
    }

    $optionalHeaders = ReadOptionalHeaders -binaryReader $reader -stream $stream -peFormat $peFormat
    if($optionalHeaders.CLIHeader.IsZero) {
        return AssemblyInfo -peFormat $peFormat -characteristics $characteristics -machine $machine
    }

    $sections = ReadSections -binaryReader $reader -stream $stream -count $numberOfSections

    $cliHeader = ReadCLIHeader -binaryReader $reader -stream $stream `
        -dataDirectory $optionalHeaders.CLIHeader -sections $sections

    $targetFramework = GetTargetFrameworkVersion -binaryReader $reader -stream $stream `
        -dataDirectory $cliHeader.Metadata -sections $sections -optionalHeaders $optionalHeaders

    $assemblyInfo = AssemblyInfo -peFormat $peFormat -attributes $cliHeader.Attributes -machine $machine -optionalHeaders $optionalHeaders `
            -characteristics $optionalHeaders.Characteristics -majorRuntimeVersion $cliHeader.MajorRuntimeVersion `
            -minorRuntimeVersion $cliHeader.MinorRuntimeVersion -targetFramework $targetFramework

    $reader.Dispose()
    $stream.Dispose()

    return $assemblyInfo
  
}

$rootFolder = "D:\Bruker_Data\anycpu-mixedplatforms"
$binFolder = Join-Path $rootFolder "bin"

Write-Diagnostic "MSIL - AnyCpu"

Get-AssemblyInfo -filename "$binFolder\sample.solution.anycpu\1.0.0\AnyCpu\Release\v2.0\sample.solution.anycpu.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.anycpu\1.0.0\AnyCpu\Release\v4.0\sample.solution.anycpu.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.anycpu\1.0.0\AnyCpu\Release\v4.5\sample.solution.anycpu.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.anycpu\1.0.0\AnyCpu\Release\v4.5.1\sample.solution.anycpu.exe"

Write-Diagnostic "MSIL - x86"

Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x86\Release\v2.0\sample.solution.mixedplatforms.x86.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x86\Release\v4.0\sample.solution.mixedplatforms.x86.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x86\Release\v4.5\sample.solution.mixedplatforms.x86.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x86\Release\v4.5.1\sample.solution.mixedplatforms.x86.exe"

Write-Diagnostic "MSIL - x64"

Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x64\Release\v2.0\sample.solution.mixedplatforms.x64.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x64\Release\v4.0\sample.solution.mixedplatforms.x64.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x64\Release\v4.5\sample.solution.mixedplatforms.x64.exe"
Get-AssemblyInfo -filename "$binFolder\sample.solution.mixedplatforms\1.0.0\x64\Release\v4.5.1\sample.solution.mixedplatforms.x64.exe"

# Also works with native binaries:
# Get-AssemblyInfo -filename "C:\Users\b543915\AppData\Roaming\Spotify\spotify.exe"
# Get-AssemblyInfo -filename "C:\Program Files\ConEmu\ConEmu64.exe"