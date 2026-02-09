#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Downloads pre-built PDFium binaries from bblanchon/pdfium-binaries.
.DESCRIPTION
    Downloads PDFium native libraries for the specified platforms and places them
    in the correct runtimes/{rid}/native/ directory structure.
.PARAMETER Version
    The PDFium version to download. Defaults to latest.
.PARAMETER OutputDir
    The output directory. Defaults to ../runtimes relative to this script.
#>
param(
    [string]$Version = "latest",
    [string]$OutputDir = (Join-Path $PSScriptRoot ".." "runtimes")
)

$ErrorActionPreference = "Stop"

$platforms = @(
    @{ Rid = "win-x64";     Asset = "pdfium-win-x64.tgz" },
    @{ Rid = "win-x86";     Asset = "pdfium-win-x86.tgz" },
    @{ Rid = "linux-x64";   Asset = "pdfium-linux-x64.tgz" },
    @{ Rid = "linux-arm64";  Asset = "pdfium-linux-arm64.tgz" },
    @{ Rid = "osx-x64";     Asset = "pdfium-mac-x64.tgz" },
    @{ Rid = "osx-arm64";   Asset = "pdfium-mac-arm64.tgz" }
)

$baseUrl = if ($Version -eq "latest") {
    "https://github.com/bblanchon/pdfium-binaries/releases/latest/download"
} else {
    "https://github.com/bblanchon/pdfium-binaries/releases/download/chromium/$Version"
}

# Create output directory
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

foreach ($platform in $platforms) {
    $rid = $platform.Rid
    $asset = $platform.Asset
    $url = "$baseUrl/$asset"
    $targetDir = Join-Path $OutputDir $rid "native"
    $tempFile = Join-Path ([System.IO.Path]::GetTempPath()) $asset

    Write-Host "Downloading $asset for $rid..." -ForegroundColor Cyan

    try {
        Invoke-WebRequest -Uri $url -OutFile $tempFile -UseBasicParsing

        New-Item -ItemType Directory -Force -Path $targetDir | Out-Null

        # Extract the tgz
        $tempExtract = Join-Path ([System.IO.Path]::GetTempPath()) "pdfium-$rid"
        if (Test-Path $tempExtract) { Remove-Item -Recurse -Force $tempExtract }
        New-Item -ItemType Directory -Force -Path $tempExtract | Out-Null

        tar -xzf $tempFile -C $tempExtract

        # Find and copy the library file
        $libFiles = Get-ChildItem -Path $tempExtract -Recurse -Include "pdfium.dll", "libpdfium.so", "libpdfium.dylib"
        foreach ($lib in $libFiles) {
            Copy-Item -Path $lib.FullName -Destination $targetDir -Force
            Write-Host "  Copied $($lib.Name) to $targetDir" -ForegroundColor Green
        }

        # Cleanup
        Remove-Item -Force $tempFile -ErrorAction SilentlyContinue
        Remove-Item -Recurse -Force $tempExtract -ErrorAction SilentlyContinue
    }
    catch {
        Write-Warning "Failed to download $asset : $_"
    }
}

Write-Host "`nDone! Binaries are in $OutputDir" -ForegroundColor Green
