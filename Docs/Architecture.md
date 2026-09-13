# Architecture

Expedition.ModApi is a source-built assembly shared by the game and the SDK. Public configuration sources and necessary authoring data live in `Assets/Expedition Mod SDK/Runtime`; no game DLLs, Odin, private gameplay assets or paid plugins are distributed. Gameplay services, DI, object management and action execution remain in the game.

Each source mod lives under `Assets/Mods/<Name>` with its own manifest and Content folder. Each output package has its own schema-2 manifest, catalog, hash and bundles. Unity, Addressables, URP, public API and Windows x64 must match the game. ProjectVersion.txt owns the Editor version; package manifests pin dependencies, and builds check resolved Addressables/URP versions against ModContract.

ModProjectValidator validates authoring inputs, IDs, versions, entry membership and references between source mod folders. ModBuildCommand alone owns compilation and packaging. Before validation/build it restores explicitly declared localization tables to the selected mod group and manifest addresses, because Unity Localization reassigns them on a fresh collection import; localization labels remain intact. During each build it includes only the selected content group, makes it the default for shared script/shader bundles, then restores the authoring settings. Paths remain `expedition-mod://<mod-id>/...`; no SDK-local Library paths should be required by the distributed package.

ModPackageValidation validates package contracts and dependency ordering. ModPackageInstaller copies a validated package into an explicitly selected game Mods folder and verifies identical bytes; it refuses conflicting installations. ModPackageVerifier tests actual external catalog locations and releases its assets/catalog handles.

The game's startup loader owns discovery under persistentDataPath/Mods, dependency checks, path remapping and external catalogs. Domain services own registration, stable replacement slots, localization, save compatibility and asset-handle lifetimes. Membership comes from labels across all loaded locators. The mod set is fixed until restart.

The test packages demonstrate real authoring and bundle transport. Their availability is not a claim that every domain has completed runtime registration or that the planned 15-action scenario has passed. Current verification limits are recorded in CreatingAMod.md.

TutorialContainer, Tutorial, TutorialPage, paragraphs and criteria are authored through Unity Editor APIs. The container offers separate Russian and English tutorials, each with six pages and the same action criteria. Both use the existing scene and never restore/reload it. It opens explicitly from the Tutorials menu; there is no custom auto-opening callback.
