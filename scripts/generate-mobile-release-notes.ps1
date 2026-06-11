# Generates release notes markdown from git tags / recent commits.
param(
    [string]$FromTag = "",
    [string]$OutputFile = ""
)

$ErrorActionPreference = "Stop"
$lines = @("## Mobile release notes", "")

if ($FromTag -ne "") {
    $commits = git log "$FromTag..HEAD" --pretty=format:"- %s (%h)" -- FrontEnd.Mobile/
} else {
    $commits = git log -15 --pretty=format:"- %s (%h)" -- FrontEnd.Mobile/
}

if ($commits) {
    $lines += $commits
} else {
    $lines += "- No commits in range"
}

$text = $lines -join "`n"
if ($OutputFile -ne "") {
    Set-Content -Path $OutputFile -Value $text
    Write-Host "Wrote $OutputFile"
} else {
    Write-Output $text
}
