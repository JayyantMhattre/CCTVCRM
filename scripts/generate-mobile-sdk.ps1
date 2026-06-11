# Generates Dart API client from backend OpenAPI document.
# See docs/mobile/foundation/README.md and docs/mobile/sdk-generation.md
param(
    [string]$OpenApiUrl = "http://localhost:5000/openapi/v1.json",
    [string]$SpecFile = "",
    [string]$OutputDir = "$PSScriptRoot/../FrontEnd.Mobile/packages/api_client",
    [string]$PackageName = "ashraak_api_client"
)

$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path "$PSScriptRoot/.."
$tempSpec = Join-Path $env:TEMP "ashraak-openapi-v1.json"

if ($SpecFile -ne "") {
    if (-not (Test-Path $SpecFile)) { Write-Error "Spec file not found: $SpecFile" }
    Copy-Item $SpecFile $tempSpec -Force
} else {
    Write-Host "Fetching OpenAPI from $OpenApiUrl"
    Invoke-WebRequest -Uri $OpenApiUrl -OutFile $tempSpec -UseBasicParsing
}

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error @"
Docker is required for openapi-generator-cli.
Alternative: install @openapitools/openapi-generator-cli via npm and run:
  openapi-generator-cli generate -i $tempSpec -g dart-dio -o $OutputDir --additional-properties=pubName=$PackageName
"@
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

Write-Host "Generating Dart SDK to $OutputDir"
docker run --rm `
    -v "${tempSpec}:/local/openapi.json:ro" `
    -v "${OutputDir}:/local/out" `
    openapitools/openapi-generator-cli:v7.10.0 generate `
    -i /local/openapi.json `
    -g dart-dio `
    -o /local/out `
    --additional-properties=pubName=$PackageName,nullableFields=true

Write-Host "SDK generation complete. Add path dependency in FrontEnd.Mobile/pubspec.yaml if adopting."
Write-Host "Manual edits inside generated output are forbidden — wrap in core/api adapters."
