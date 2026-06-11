# Applies M5 release config (Android flavors) after flutter create bootstrap.
param(
    [string]$ProjectDir = "$PSScriptRoot/../FrontEnd.Mobile"
)

$ErrorActionPreference = "Stop"
$template = Join-Path $ProjectDir "tooling/android/app-build.gradle.kts"
$target = Join-Path $ProjectDir "android/app/build.gradle.kts"

if (-not (Test-Path $template)) { Write-Error "Missing template: $template" }
if (-not (Test-Path (Split-Path $target -Parent))) {
    Write-Host "android/ not found — run bootstrap-mobile-platform.ps1 first"
    exit 0
}

Copy-Item $template $target -Force
Write-Host "Applied flavor-enabled build.gradle.kts"

$keyExample = Join-Path $ProjectDir "android/key.properties.example"
$keyTarget = Join-Path $ProjectDir "android/key.properties"
if ((Test-Path $keyExample) -and -not (Test-Path $keyTarget)) {
    Write-Host "Copy android/key.properties.example -> key.properties for local signing"
}
