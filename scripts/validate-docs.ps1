# Validates that module/feature code changes include documentation updates.
# Usage: .\scripts\validate-docs.ps1 [-BaseRef origin/main] [-Mode warn|fail]

param(
    [string]$BaseRef = "",
    [ValidateSet("warn", "fail")]
    [string]$Mode = ""
)

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $Root

$configPath = Join-Path $Root "docs\doc-validation.json"
$config = Get-Content $configPath -Raw | ConvertFrom-Json

if (-not $Mode) {
    if ($env:DOC_VALIDATE_MODE -in @("warn", "fail")) { $Mode = $env:DOC_VALIDATE_MODE }
    elseif ($config.mode -in @("warn", "fail")) { $Mode = $config.mode }
    else { $Mode = "warn" }
}

if (-not $BaseRef) {
    foreach ($candidate in @("origin/main", "origin/master", "main", "HEAD~1")) {
        git rev-parse --verify $candidate 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) { $BaseRef = $candidate; break }
    }
}

if (-not $BaseRef) {
    Write-Host "doc-validate: no base ref; skipping."
    exit 0
}

Write-Host "doc-validate: comparing $BaseRef..HEAD (mode=$Mode)"

$changed = @(git diff --name-only "${BaseRef}...HEAD" 2>$null)
if (-not $changed -or $changed.Count -eq 0) {
    $changed = @(git diff --name-only $BaseRef HEAD 2>$null)
}

if (-not $changed -or $changed.Count -eq 0) {
    Write-Host "doc-validate: no changed files."
    exit 0
}

$docOnlyPatterns = @(
    "^docs/", "\.md$", "^README\.md$", "^DEVELOPER_GUIDE\.md$",
    "^FrontEnd/FRONTEND_STARTER\.md$", "^FrontEnd/COREUI_INTEGRATION\.md$",
    "^BackEnd/DOCKER_ENVIRONMENT\.md$"
)
$onlyDocs = $true
foreach ($f in $changed) {
    $isDoc = $false
    foreach ($p in $docOnlyPatterns) {
        if ($f -match $p) { $isDoc = $true; break }
    }
    if (-not $isDoc) { $onlyDocs = $false; break }
}
if ($onlyDocs) {
    Write-Host "doc-validate: documentation-only changes - OK."
    exit 0
}

function Test-PathChanged([string]$Prefix) {
    foreach ($f in $changed) {
        if ($f.StartsWith($Prefix)) { return $true }
    }
    return $false
}

function Test-AnyDocChanged([string[]]$Prefixes) {
    foreach ($p in $Prefixes) {
        foreach ($f in $changed) {
            if ($f.StartsWith($p)) { return $true }
        }
    }
    return $false
}

$violations = @()
foreach ($mapping in $config.mappings) {
    $codeHit = $false
    foreach ($cp in $mapping.codePaths) {
        if (Test-PathChanged $cp) { $codeHit = $true; break }
    }
    if (-not $codeHit) { continue }

    $docHit = Test-AnyDocChanged @($mapping.docPaths)
    if (-not $docHit) {
        $violations += "$($mapping.name): code changed; expected doc update under: $($mapping.docPaths -join ', ')"
    }
}

if ($violations.Count -gt 0) {
    Write-Host ""
    Write-Host "=== Documentation validation ($Mode) ==="
    foreach ($v in $violations) { Write-Host "  - $v" }
    Write-Host ""
    Write-Host "See docs/documentation-pr-checklist.md and docs/developer-workflow.md"
    if ($Mode -eq "fail") { exit 1 }
    exit 0
}

Write-Host "doc-validate: all mapped code changes have corresponding doc updates."
exit 0
