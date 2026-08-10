# Expedition Mod SDK instructions

These instructions apply to the entire repository.

- Keep the SDK content-only. Do not add code-mod loading, arbitrary assembly execution, patching, or injection.
- Pin Unity and package versions to the versions documented in `README.md`.
- Only Unity Registry packages and repository-owned source/content may be committed.
- Do not add paid, Asset Store-only, closed-source, or private game packages and assets.
- Do not commit DLLs copied from the game. `Expedition.ModApi` is built from the public source in this repository.
- Treat `Assets/AddressableAssetsData/` as Unity-owned state. Change it through Unity or the SDK configuration command, not by hand.
- Do not hand-create `.meta` files or GUIDs.
- Keep one build owner: `ModBuildCommand` validates and builds a mod package.
- Keep generated output under `Builds/`; it is intentionally ignored by Git.
- Before publishing, run the repository audit and a real sample-mod build described in `README.md`.
