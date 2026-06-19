[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string] $OutputDirectory,

    [Parameter(Mandatory = $false)]
    [string] $Version,

    [Parameter(Mandatory = $false)]
    [switch] $NoGitInfo,

    [Parameter(Mandatory = $false)]
    [ValidateRange(0, 20)]
    [int] $RestrictFramework
)

#Requires -Version 7.5
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version Latest

function InjectArguments() {
    param(
        [Parameter(Mandatory)]
        [string[]] $Arguments
    )
    
    if ($NoGitInfo) {
        $Arguments += @(
            '-p:EnableGitInfo=false'
        )
    }
 
    if ($RestrictFramework -gt 0) {
        $Arguments += @(
            '-f',
            'net' + $RestrictFramework + '.0'
        )
    }

    return $Arguments
}

function Main() {
    $directory = $PSScriptRoot

    $DestinationDirectory = (Resolve-Path -Path $OutputDirectory).Path

    $null = New-item -ItemType Directory -Path $DestinationDirectory -Force -ErrorAction Ignore

    Push-Location -Path $directory
    try {
        $arguments = @(
            'build',
            '--configuration',
            'Release'
        )

        if ($NoGitInfo) {
            $Arguments += @(
                '-p:EnableGitInfo=false'
            )
        }

        if ($RestrictFramework -gt 0) {
            $Arguments += @(
                '-f',
                ('net' + $RestrictFramework + '.0')
            )
        }

        & dotnet $arguments

        $arguments = @(
            'pack',
            '--configuration',
            'Release',
            '--no-restore',
            '--no-build',
            "/p:PackageOutputPath=$DestinationDirectory"
        )

        if ($NoGitInfo) {
            $Arguments += @(
                '-p:EnableGitInfo=false'
            )
        }

        # if ($RestrictFramework -gt 0) {
        #     $Arguments += @(
        #         "/p:TargetFramework=net$RestrictFramework.0"
        #     )
        # }

        if ($Version) {
            $arguments += @(
                "/p:Version=$Version"
            )
        }

        & dotnet $arguments
    }
    finally {
        Pop-Location
    }
}

Main