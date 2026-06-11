# Validates all PackageReference entries have a central PackageVersion (NU1010 prevention).
param(
    [string]$BackendRoot = (Join-Path (Join-Path $PSScriptRoot "..") "BackEnd")
)

$ErrorActionPreference = "Stop"
$propsFile = Join-Path $BackendRoot "Directory.Packages.props"

if (-not (Test-Path $propsFile)) {
    Write-Error "Directory.Packages.props not found at $propsFile"
    exit 1
}

$central = Select-String -Path $propsFile -Pattern 'PackageVersion Include="([^"]+)"' |
    ForEach-Object { $_.Matches.Groups[1].Value } |
    Sort-Object -Unique

$refs = Get-ChildItem -Path $BackendRoot -Recurse -Filter *.csproj |
    ForEach-Object {
        Select-String -Path $_.FullName -Pattern 'PackageReference Include="([^"]+)"' |
            ForEach-Object { $_.Matches.Groups[1].Value }
    } |
    Sort-Object -Unique

$missing = @($refs | Where-Object { $_ -notin $central })

Write-Host "package-audit: central versions=$($central.Count) references=$($refs.Count)"

if ($missing.Count -gt 0) {
    Write-Host "package-audit: FAILED - missing PackageVersion entries:" -ForegroundColor Red
    $missing | ForEach-Object { Write-Host "  - $_" }
    exit 1
}

Write-Host "package-audit: OK - all PackageReference items are centrally versioned." -ForegroundColor Green
exit 0
