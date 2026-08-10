# Expedition Mod SDK

An open Unity project for authoring content-only mods for Expedition through Addressables secondary catalogs.

## Status

This repository contains the authoring and build side of the mod pipeline. The matching catalog loader and public gameplay contracts still need to be integrated into the game before packages built here can be installed by players.

Pinned compatibility:

- Unity `6000.7.0a3`
- Addressables `2.11.1`
- Universal Render Pipeline `17.7.0`
- Initial target: Windows 64-bit

Use those exact versions. AssetBundles are not portable across arbitrary Unity, package, render-pipeline, or type-layout changes.

## No paid plugins or private game files

The project uses Unity Registry packages only. It does not contain Odin Inspector, other paid Asset Store plugins, private Expedition assets, or copied game DLLs.

`Expedition.ModApi.dll` is compiled from the public source in `Assets/Expedition Mod SDK/Runtime`. Do not replace it with a stripped Player assembly: managed stripping is a Player optimization and does not produce a stable authoring contract.

## Create a mod

1. Install Unity `6000.7.0a3` with Windows Build Support.
2. Clone and open this project.
3. Copy the project to a new folder for your mod, or replace the sample content in `Assets/Expedition Mod SDK/Mod/Content`.
4. Edit `Assets/Expedition Mod SDK/Mod/mod.json`.
5. Add assets to the `Expedition Mod Content` Addressables group and give each one the address declared in `mod.json`.
6. Select `Expedition > Mod SDK > Validate Mod`.
7. Select `Expedition > Mod SDK > Build Mod`.

The package is written to `Builds/Packages/<mod-id>-<version>/`. Generated packages are ignored by Git.

See [Creating a mod](Docs/CreatingAMod.md) for the manifest schema, [Architecture](Docs/Architecture.md) for the compatibility contract, and [References](Docs/References.md) for the research basis.

## Command-line build

```powershell
$unityEditor = 'C:\Path\To\Unity\6000.7.0a3\Editor\Unity.exe'
$project = 'C:\Path\To\ExpeditionModSDK'
& $unityEditor `
  -batchmode -quit `
  -projectPath $project `
  -buildTarget StandaloneWindows64 `
  -executeMethod Expedition.ModSdk.Editor.ModBuildCommand.BuildFromCommandLine `
  -logFile (Join-Path $project 'mod-build.log')
```

A non-zero Unity exit code means validation or build failed. The log uses the `[Mod SDK]` prefix.

## Repository audit

After Git is initialized, run:

```powershell
./Tools/audit_repository.ps1
```

The audit rejects tracked DLLs, Unity packages, `Assets/Plugins` content, and non-Unity package sources.

## Recreate the Addressables setup

The repository already contains its generated Addressables settings. If they are deliberately removed, run `Expedition > Mod SDK > Configure Project`. This is an explicit setup action; the build command never repairs missing settings automatically.

## License

Repository-owned source and documentation are MIT licensed. Unity and Unity Registry packages retain their own licenses; see [Third-party notices](THIRD_PARTY_NOTICES.md).
