# Architecture

## Scope

The first version supports content-only mods. It does not execute mod assemblies or patch game code.

Each built mod contains:

- a validated `mod.json`;
- one Addressables secondary catalog and hash;
- the AssetBundles referenced by that catalog.

The SDK writes catalog and bundle internal IDs with the `expedition-mod://<mod-id>/...` scheme. The game owns one Addressables `InternalIdTransformFunc` that maps this scheme to the installed mod directory before it loads the catalog.

Catalogs are loaded once during startup. Addressables 2.11 does not expose catalog unloading, so enabling or disabling mods requires a restart.

## Compatibility boundary

Build content with the exact Unity editor, Addressables, render pipeline, and public API versions used by the game. A mod package records its minimum game version; the future game-side loader must validate it before loading the catalog.

The public API assembly is intentionally small and source-built. Private gameplay assemblies are not an SDK contract. If a later content type needs game serialization, expose the smallest stable type in `Expedition.ModApi` and publish a new SDK version.

Do not ship:

- full game DLLs;
- Player-stripped DLLs;
- .NET reference assemblies used as Unity runtime substitutes;
- paid plugin assemblies or editor tooling;
- private game art, audio, scenes, prefabs, or ScriptableObjects.

## Ownership

- `ModProjectValidator` owns manifest and Addressables mapping validation.
- `ModBuildCommand` owns the build and package output.
- `ModSdkProjectBootstrap` only creates the committed Addressables authoring setup when explicitly invoked.
- The game-side loader will own discovery, dependency ordering, ID conflict policy, path remapping, and catalog loading.

The build command fails when setup is missing; it does not add fallback configuration or silently repair state.
