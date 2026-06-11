# Build Ashraak mobile for a flavor (dev|qa|uat|prod).
param(
    [ValidateSet('dev', 'qa', 'uat', 'prod')]
    [string]$Flavor = 'dev',
    [ValidateSet('apk', 'appbundle', 'ipa', 'ios')]
    [string]$Target = 'apk',
    [string]$ProjectDir = "$PSScriptRoot/../FrontEnd.Mobile",
    [int]$BuildNumber = 0
)

$ErrorActionPreference = "Stop"
& "$PSScriptRoot/sync-mobile-version.ps1" -ProjectDir $ProjectDir -BuildNumberOverride $BuildNumber
Set-Location $ProjectDir

if (-not (Get-Command flutter -ErrorAction SilentlyContinue)) {
    Write-Error "Flutter SDK not found in PATH."
}

$defines = @(
    "--dart-define=ENV=$Flavor",
    "--dart-define=FLAVOR=$Flavor"
)

if ($BuildNumber -gt 0) {
    $defines += "--dart-define=APP_BUILD_NUMBER=$BuildNumber"
}

flutter pub get

switch ($Target) {
    'apk' {
        flutter build apk --release --flavor $Flavor @defines
    }
    'appbundle' {
        flutter build appbundle --release --flavor $Flavor @defines
    }
    'ipa' {
        flutter build ipa --release --flavor $Flavor @defines
    }
    'ios' {
        flutter build ios --release --flavor $Flavor @defines
    }
}

Write-Host "Build complete: $Flavor / $Target"
