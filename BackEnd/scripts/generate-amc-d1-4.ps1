# D1-4 AMC module source generator — run once, then delete if desired.
$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot '..'
$amc = Join-Path $root 'src/Modules/Cctv/Amc'
$contracts = Join-Path $root 'src/Shared/Ashraak.SharedKernel.Contracts/CctvCrm'

function Write-File($path, $content) {
    $dir = Split-Path $path -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    Set-Content -Path $path -Value $content -Encoding UTF8
    Write-Host "Wrote $path"
}

# Script continues in separate invocations via Write tool for maintainability.
Write-Host "Use Write tool batches for AMC D1-4 implementation."
