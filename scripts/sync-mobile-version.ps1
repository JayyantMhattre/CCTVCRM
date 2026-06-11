# Syncs FrontEnd.Mobile/version.yaml -> pubspec.yaml (and optional native projects).
param(
    [string]$ProjectDir = "$PSScriptRoot/../FrontEnd.Mobile",
    [int]$BuildNumberOverride = 0
)

$ErrorActionPreference = "Stop"
$versionFile = Join-Path $ProjectDir "version.yaml"
$pubspec = Join-Path $ProjectDir "pubspec.yaml"

if (-not (Test-Path $versionFile)) { Write-Error "Missing $versionFile" }

$content = Get-Content $versionFile -Raw
if ($content -match 'version:\s*([0-9]+\.[0-9]+\.[0-9]+)') { $semver = $Matches[1] } else { Write-Error "version: x.y.z not found" }
if ($content -match 'build:\s*([0-9]+)') { $build = [int]$Matches[1] } else { $build = 1 }
if ($BuildNumberOverride -gt 0) { $build = $BuildNumberOverride }

$full = "$semver+$build"
Write-Host "Syncing mobile version -> $full"

$pub = Get-Content $pubspec -Raw
$pub = $pub -replace '(?m)^version:\s*.+$', "version: $full"
Set-Content -Path $pubspec -Value $pub -NoNewline

Write-Host "Updated pubspec.yaml"
