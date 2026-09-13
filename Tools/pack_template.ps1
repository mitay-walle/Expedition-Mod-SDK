param([string]$Version = '1.0.0')
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$builds = Join-Path $root 'Builds'
$stage = Join-Path $builds ('TemplateSource-' + [Guid]::NewGuid().ToString('N'))
$outputDirectory = Join-Path $builds 'Templates'
$output = Join-Path $outputDirectory "expedition-mod-sdk-$Version.tgz"
if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw 'Version must be major.minor.patch.' }
New-Item -ItemType Directory -Path $stage,$outputDirectory -Force | Out-Null
foreach ($directory in @('Assets', 'Packages', 'ProjectSettings')) {
    Copy-Item -LiteralPath (Join-Path $root $directory) -Destination (Join-Path $stage $directory) -Recurse
}
$documentation = Join-Path $stage 'Assets/Expedition Mod SDK/Documentation'
New-Item -ItemType Directory -Path $documentation -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Docs') -Destination (Join-Path $documentation 'Docs') -Recurse
foreach ($file in @('README.md', 'LICENSE', 'THIRD_PARTY_NOTICES.md')) {
    Copy-Item -LiteralPath (Join-Path $root $file) -Destination (Join-Path $documentation $file)
}
& unity templates pack $stage --output $output --name com.expedition.template.mod-sdk --display-name 'Expedition Mod SDK' --description 'Public content-only mod SDK with four Addressables test mods and a build/install tutorial.' --template-version $Version --keep-project-settings --overwrite --format json
if ($LASTEXITCODE -ne 0) { throw 'Unity template packaging failed.' }
Write-Host "[Mod SDK] Template: $output"
Write-Host "[Mod SDK] Source snapshot retained under Builds: $stage"
