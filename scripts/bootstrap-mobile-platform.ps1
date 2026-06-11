# Bootstraps FrontEnd.Mobile when platform folders are missing (no Flutter code in M0).
param(
    [string]$ProjectDir = "$PSScriptRoot/../FrontEnd.Mobile"
)

$ErrorActionPreference = "Stop"
Set-Location $ProjectDir

if (-not (Get-Command flutter -ErrorAction SilentlyContinue)) {
    Write-Error "Flutter SDK not found in PATH. Install Flutter 3.24+ and retry."
}

if (-not (Test-Path "android") -or -not (Test-Path "ios")) {
    Write-Host "Creating android/ios platform folders..."
    flutter create . --org com.ashraak --project-name ashraak_mobile --platforms=android,ios
}

flutter pub get

& "$PSScriptRoot/apply-mobile-release-config.ps1" -ProjectDir $ProjectDir
& "$PSScriptRoot/sync-mobile-version.ps1" -ProjectDir $ProjectDir

Write-Host "Mobile platform bootstrap complete."
