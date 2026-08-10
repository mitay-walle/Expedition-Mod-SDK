$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$manifestPath = Join-Path $root 'Packages\manifest.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$errors = [System.Collections.Generic.List[string]]::new()

foreach ($dependency in $manifest.dependencies.PSObject.Properties) {
    if (-not $dependency.Name.StartsWith('com.unity.', [System.StringComparison]::Ordinal)) {
        $errors.Add("Non-Unity Registry dependency: $($dependency.Name)")
    }

    $version = [string] $dependency.Value
    if ($version.StartsWith('file:', [System.StringComparison]::OrdinalIgnoreCase) -or
        $version.StartsWith('git', [System.StringComparison]::OrdinalIgnoreCase) -or
        $version.Contains('://')) {
        $errors.Add("Non-registry dependency source: $($dependency.Name) = $version")
    }
}

$forbiddenTrackedFiles = @(
    & git -C $root ls-files '*.dll' '*.unitypackage' '*.tgz' 'Assets/Plugins/**'
)

foreach ($file in $forbiddenTrackedFiles) {
    if (-not [string]::IsNullOrWhiteSpace($file)) {
        $errors.Add("Forbidden tracked binary or plugin path: $file")
    }
}

if ($LASTEXITCODE -ne 0) {
    $errors.Add('git ls-files failed.')
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host '[Mod SDK Audit] Passed: Unity Registry dependencies only; no tracked DLL, unitypackage, tgz, or Assets/Plugins content.'
