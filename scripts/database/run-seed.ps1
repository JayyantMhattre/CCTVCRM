# Load smoke-test seed data into PostgreSQL.
# Run from repository root after migrations are applied.
#
# Usage:
#   .\scripts\database\run-seed.ps1

$ErrorActionPreference = "Stop"
$RepoRoot = Join-Path $PSScriptRoot "..\.."
$SeedFile = Join-Path $RepoRoot "scripts\test-data\seed-smoke-chain.sql"

if (-not (Test-Path $SeedFile)) {
    throw "Seed file not found: $SeedFile"
}

Write-Host "Loading smoke test seed data..." -ForegroundColor Cyan
Get-Content $SeedFile -Raw | docker compose exec -T postgres psql -U ashraak -d ashraak
Write-Host "Seed data loaded." -ForegroundColor Green
