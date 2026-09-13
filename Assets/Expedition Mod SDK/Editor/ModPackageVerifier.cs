using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Expedition.ModApi;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Expedition.ModSdk.Editor
{
    public static class ModPackageVerifier
    {
        [MenuItem("Expedition/Mod SDK/Verify Built Bundles")]
        public static void VerifyFromMenu() => Debug.Log(Verify());

        public static string Verify()
        {
            var packages = Directory.GetDirectories(ModSdkPaths.PackagesRoot).Select(path =>
                new ModPackage(path, JsonUtility.FromJson<ModManifest>(File.ReadAllText(Path.Combine(path, "mod.json"))))).ToArray();
            ModPackageValidation.ValidateAndOrder(packages, "0.1.0", Application.unityVersion);
            var byId = packages.ToDictionary(package => package.Manifest.modId);
            var previousTransform = Addressables.InternalIdTransformFunc;
            var lines = new List<string>();
            Addressables.InternalIdTransformFunc = location =>
            {
                string id = location.InternalId;
                const string prefix = "expedition-mod://";
                if (!id.StartsWith(prefix, StringComparison.Ordinal)) return previousTransform != null ? previousTransform(location) : id;
                string relative = id.Substring(prefix.Length); int slash = relative.IndexOf('/');
                if (slash < 0 || !byId.TryGetValue(relative.Substring(0, slash), out var package)) throw new InvalidOperationException("Unknown package path: " + id);
                return package.ResolvePath(relative.Substring(slash + 1));
            };
            try
            {
                foreach (ModPackage package in packages)
                {
                    var catalog = Addressables.LoadContentCatalogAsync(package.ResolvePath(package.Manifest.catalog), false);
                    try
                    {
                        var locator = catalog.WaitForCompletion();
                        if (catalog.Status != AsyncOperationStatus.Succeeded) throw new InvalidOperationException("Catalog failed: " + package.Manifest.modId);
                        try
                        {
                            int count = 0;
                            foreach (ModContentEntry entry in package.Manifest.content)
                            {
                                if (!locator.Locate(entry.address, typeof(UnityEngine.Object), out var locations) || locations.Count != 1)
                                    throw new InvalidOperationException("Expected one location for " + entry.address);
                                var asset = Addressables.LoadAssetAsync<UnityEngine.Object>(locations[0]);
                                try
                                {
                                    var value = asset.WaitForCompletion();
                                    if (asset.Status != AsyncOperationStatus.Succeeded || value == null) throw new InvalidOperationException("Bundle asset failed: " + entry.address);
                                    if (entry.address == "sample.author/additive/sample-text" &&
                                        (!(value is TextAsset text) || !text.text.Contains("public SDK"))) throw new InvalidOperationException("Unexpected sample text.");
                                    count++;
                                }
                                finally { if (asset.IsValid()) Addressables.Release(asset); }
                            }
                            lines.Add(package.Manifest.modId + ": PASS, " + count + " assets loaded from bundle locations");
                        }
                        finally { Addressables.RemoveResourceLocator(locator); }
                    }
                    finally { if (catalog.IsValid()) Addressables.Release(catalog); }
                }
            }
            finally { Addressables.InternalIdTransformFunc = previousTransform; }
            string report = string.Join("\n", lines);
            Directory.CreateDirectory(ModSdkPaths.BuildsRoot);
            File.WriteAllText(Path.Combine(ModSdkPaths.BuildsRoot, "BundleVerification.txt"), DateTime.UtcNow.ToString("O") + "\n" + report);
            return report;
        }
    }
}
