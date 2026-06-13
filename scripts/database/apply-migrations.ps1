# Apply EF Core migrations for all modules with migration folders.
# Run from repository root after PostgreSQL is healthy.
#
# Prerequisites:
#   - .NET SDK 10 (see BackEnd/global.json)
#   - PostgreSQL reachable at localhost:5432 (or POSTGRES_PORT from .env)
#
# Usage:
#   .\scripts\database\apply-migrations.ps1
#   .\scripts\database\apply-migrations.ps1 -ConnectionString "Host=localhost;..."

param(
    [string]$ConnectionString = "Host=localhost;Port=5432;Database=ashraak;Username=ashraak;Password=ashraak_dev"
)

$ErrorActionPreference = "Stop"
$BackendRoot = Join-Path $PSScriptRoot "..\..\BackEnd"
$StartupProject = Join-Path $BackendRoot "src\Host\Ashraak.Api\Ashraak.Api.csproj"

$MigrationProjects = @(
    "src\Modules\Cctv\Lead\Ashraak.Cctv.Lead.Infrastructure\Ashraak.Cctv.Lead.Infrastructure.csproj",
    "src\Modules\Cctv\Customer\Ashraak.Cctv.Customer.Infrastructure\Ashraak.Cctv.Customer.Infrastructure.csproj",
    "src\Modules\Cctv\Amc\Ashraak.Cctv.Amc.Infrastructure\Ashraak.Cctv.Amc.Infrastructure.csproj",
    "src\Modules\Cctv\Service\Ashraak.Cctv.Service.Infrastructure\Ashraak.Cctv.Service.Infrastructure.csproj",
    "src\Modules\Cctv\Engineer\Ashraak.Cctv.Engineer.Infrastructure\Ashraak.Cctv.Engineer.Infrastructure.csproj",
    "src\Modules\Cctv\Ticket\Ashraak.Cctv.Ticket.Infrastructure\Ashraak.Cctv.Ticket.Infrastructure.csproj",
    "src\Modules\Cctv\Invoice\Ashraak.Cctv.Invoice.Infrastructure\Ashraak.Cctv.Invoice.Infrastructure.csproj"
)

Push-Location $BackendRoot
try {
    foreach ($project in $MigrationProjects) {
        $projectPath = Join-Path $BackendRoot $project
        Write-Host "Applying migrations: $project" -ForegroundColor Cyan
        dotnet ef database update `
            --project $projectPath `
            --startup-project $StartupProject `
            --connection $ConnectionString
        if ($LASTEXITCODE -ne 0) {
            throw "Migration failed for $project"
        }
    }
    Write-Host "All migrations applied successfully." -ForegroundColor Green
}
finally {
    Pop-Location
}
