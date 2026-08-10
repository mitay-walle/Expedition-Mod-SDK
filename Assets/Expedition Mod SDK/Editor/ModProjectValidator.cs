using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Expedition.ModApi;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    internal static class ModProjectValidator
    {
        public static ModManifest LoadAndValidate()
        {
            ModManifest manifest = LoadManifest();
            ValidateManifest(manifest);
            ValidateAddressables(manifest);
            return manifest;
        }

        public static ModManifest LoadManifest()
        {
            if (!File.Exists(ModSdkPaths.ManifestFullPath))
                throw new InvalidOperationException($"Manifest is missing: {ModSdkPaths.ManifestAssetPath}");

            string json = File.ReadAllText(ModSdkPaths.ManifestFullPath);
            ModManifest manifest = JsonUtility.FromJson<ModManifest>(json);
            return manifest ?? throw new InvalidOperationException("mod.json could not be parsed.");
        }

        public static void ValidateManifest(ModManifest manifest)
        {
            Require(manifest.schemaVersion == ModContract.SchemaVersion,
                $"schemaVersion must be {ModContract.SchemaVersion}.");
            Require(IsValidModId(manifest.modId), "modId must be a lowercase reverse-domain-style identifier.");
            Require(IsValidVersion(manifest.version), "version must use major.minor.patch format.");
            Require(IsValidVersion(manifest.minimumGameVersion), "minimumGameVersion must use major.minor.patch format.");
            Require(!string.IsNullOrWhiteSpace(manifest.displayName), "displayName is required.");
            Require(string.IsNullOrWhiteSpace(manifest.catalog), "catalog is build output and must be empty in the source manifest.");
            Require(manifest.dependencies != null, "dependencies must be a JSON array.");
            Require(manifest.content != null && manifest.content.Count > 0, "content must declare at least one asset.");

            var dependencyIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (ModDependency dependency in manifest.dependencies)
            {
                Require(dependency != null, "dependencies cannot contain null entries.");
                Require(IsValidModId(dependency.modId), $"Dependency modId '{dependency.modId}' is invalid.");
                Require(dependency.modId != manifest.modId, "A mod cannot depend on itself.");
                Require(dependencyIds.Add(dependency.modId), $"Dependency '{dependency.modId}' is declared more than once.");
                Require(IsValidVersion(dependency.minimumVersion),
                    $"Dependency '{dependency.modId}' has an invalid minimumVersion.");
            }

            var contentIds = new HashSet<string>(StringComparer.Ordinal);
            var addresses = new HashSet<string>(StringComparer.Ordinal);
            var replacementTargets = new HashSet<string>(StringComparer.Ordinal);
            foreach (ModContentEntry entry in manifest.content)
            {
                Require(entry != null, "content cannot contain null entries.");
                Require(IsNamespaced(entry.id, manifest.modId), $"Content id '{entry.id}' must start with '{manifest.modId}/'.");
                Require(contentIds.Add(entry.id), $"Content id '{entry.id}' is declared more than once.");
                Require(IsNamespaced(entry.address, manifest.modId),
                    $"Address '{entry.address}' must start with '{manifest.modId}/'.");
                Require(addresses.Add(entry.address), $"Address '{entry.address}' is declared more than once.");
                Require(IsSupportedKind(entry.kind), $"Content '{entry.id}' has unsupported kind '{entry.kind}'.");
                Require(IsSupportedMode(entry.mode), $"Content '{entry.id}' has unsupported mode '{entry.mode}'.");
                ValidateTarget(entry, replacementTargets);
            }
        }

        private static void ValidateAddressables(ModManifest manifest)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            Require(settings != null, "Addressables settings are missing. Run Expedition > Mod SDK > Configure Project.");

            AddressableAssetGroup group = settings.FindGroup(ModSdkPaths.ContentGroupName);
            Require(group != null,
                $"Addressables group '{ModSdkPaths.ContentGroupName}' is missing. Run Configure Project.");
            Require(group.entries.Count == manifest.content.Count,
                $"Group '{ModSdkPaths.ContentGroupName}' contains {group.entries.Count} entries, but mod.json declares {manifest.content.Count}.");

            var groupAddresses = new HashSet<string>(StringComparer.Ordinal);
            foreach (AddressableAssetEntry groupEntry in group.entries)
            {
                Require(groupEntry != null, $"Group '{ModSdkPaths.ContentGroupName}' contains a missing entry.");
                Require(groupAddresses.Add(groupEntry.address),
                    $"Addressables address '{groupEntry.address}' occurs more than once in the content group.");
            }

            foreach (ModContentEntry contentEntry in manifest.content)
                Require(groupAddresses.Contains(contentEntry.address),
                    $"Address '{contentEntry.address}' is declared in mod.json but not in '{ModSdkPaths.ContentGroupName}'.");
        }

        private static void ValidateTarget(ModContentEntry entry, HashSet<string> replacementTargets)
        {
            if (entry.mode == ModContract.ModeAdditive)
            {
                Require(string.IsNullOrWhiteSpace(entry.targetId),
                    $"Additive content '{entry.id}' must not declare targetId.");
                return;
            }

            Require(!string.IsNullOrWhiteSpace(entry.targetId),
                $"Replacement content '{entry.id}' must declare a game-owned targetId.");
            Require(replacementTargets.Add(entry.targetId),
                $"Replacement targetId '{entry.targetId}' is declared more than once.");
        }

        private static bool IsValidModId(string value) =>
            !string.IsNullOrWhiteSpace(value) &&
            Regex.IsMatch(value, "^[a-z0-9]+(?:[.-][a-z0-9-]+)+$", RegexOptions.CultureInvariant);

        private static bool IsValidVersion(string value) =>
            !string.IsNullOrWhiteSpace(value) &&
            Regex.IsMatch(value, "^[0-9]+\\.[0-9]+\\.[0-9]+(?:[-+][0-9A-Za-z.-]+)?$", RegexOptions.CultureInvariant);

        private static bool IsNamespaced(string value, string modId) =>
            !string.IsNullOrWhiteSpace(value) && value.StartsWith(modId + "/", StringComparison.Ordinal);

        private static bool IsSupportedKind(string value) => value is
            ModContract.KindPrefab or
            ModContract.KindData or
            ModContract.KindTexture or
            ModContract.KindMaterial or
            ModContract.KindAudio;

        private static bool IsSupportedMode(string value) => value is
            ModContract.ModeAdditive or
            ModContract.ModeReplacement;

        private static void Require(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}