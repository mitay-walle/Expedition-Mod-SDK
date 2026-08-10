# Creating a mod

## Manifest

Edit `Assets/Expedition Mod SDK/Mod/mod.json`:

```json
{
  "schemaVersion": "1",
  "modId": "sample.author",
  "version": "0.1.0",
  "displayName": "Sample Author Mod",
  "description": "A minimal additive-content example.",
  "minimumGameVersion": "0.1.0",
  "catalog": "",
  "dependencies": [],
  "content": [
    {
      "id": "sample.author/additive/sample-text",
      "address": "sample.author/additive/sample-text",
      "kind": "Data",
      "mode": "Additive",
      "targetId": ""
    }
  ]
}
```

Rules:

- `schemaVersion` is currently `1`.
- `modId` is a globally unique reverse-domain-style ID made from lowercase letters, digits, dots, and hyphens.
- `version` and `minimumGameVersion` use `major.minor.patch`.
- `catalog` is generated during the build; leave it empty in the source manifest.
- Every `content.id` and `content.address` must be unique and namespaced by `modId`.
- Supported `kind` values are `Prefab`, `Data`, `Texture`, `Material`, and `Audio`.
- Supported `mode` values are `Additive` and `Replacement`.
- `targetId` must be empty for additive content.
- Replacement content must use `targetId` to name one unique, stable, game-owned extension point; it is not an arbitrary asset path.

## Addressables content

Put mod assets under `Assets/Expedition Mod SDK/Mod/Content`. In `Window > Asset Management > Addressables > Groups`, move them into `Expedition Mod Content` and set their addresses to match the manifest.

Do not mark folders Addressable. Declare concrete assets so validation can prove exactly what ships.

## Validate and build

`Validate Mod` checks the manifest, IDs, versions, supported values, Addressables settings, and the one-to-one mapping between declared content and the owned Addressables group.

`Build Mod` runs the same validation first, builds the remote catalog and bundles, copies them into one installable folder, and writes the final catalog path into the packaged `mod.json`.

## Dependencies

Dependencies describe other mods, not Unity packages:

```json
{
  "modId": "another.author",
  "minimumVersion": "1.2.0"
}
```

The game-side loader must reject missing or incompatible dependencies before loading catalogs.
