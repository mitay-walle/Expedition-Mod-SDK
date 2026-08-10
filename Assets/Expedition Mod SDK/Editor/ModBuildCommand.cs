using System;
using System.IO;
using System.Linq;
using System.Text;
using Expedition.ModApi;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    public static class ModBuildCommand
    {
        [MenuItem("Expedition/Mod SDK/Validate Mod")]
        public static void ValidateFromMenu()
        {
            ModManifest manifest = ModProjectValidator.LoadAndValidate();
            Debug.Log($"[Mod SDK] Validation passed modId={manifest.modId} version={manifest.version} content={manifest.content.Count}.");
            EditorUtility.DisplayDialog("Expedition Mod SDK", "Mod validation passed.", "OK");
        }

        [MenuItem("Expedition/Mod SDK/Build Mod")]
        public static void BuildFromMenu()
        {
            string packagePath = Build();
            EditorUtility.RevealInFinder(packagePath);
            EditorUtility.DisplayDialog("Expedition Mod SDK", $"Mod built successfully:\n{packagePath}", "OK");
        }

        public static void BuildFromCommandLine()
        {
            try
            {
                Build();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static string Build()
        {
            RequireWindowsBuildTarget();
            ModManifest manifest = ModProjectValidator.LoadAndValidate();
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.profileSettings.SetValue(settings.activeProfileId, ModSdkPaths.ModIdProfileVariable, manifest.modId);

            RecreateOwnedDirectory(ModSdkPaths.StagingRoot);
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
            if (!string.IsNullOrWhiteSpace(result.Error))
                throw new InvalidOperationException($"Addressables build failed: {result.Error}");

            string catalogPath = FindCatalog(ModSdkPaths.StagingRoot);
            string packagePath = Path.Combine(ModSdkPaths.PackagesRoot, $"{manifest.modId}-{manifest.version}");
            RecreateOwnedDirectory(packagePath);
            CopyDirectory(ModSdkPaths.StagingRoot, packagePath);

            manifest.catalog = Path.GetRelativePath(ModSdkPaths.StagingRoot, catalogPath).Replace('\\', '/');
            File.WriteAllText(Path.Combine(packagePath, "mod.json"), JsonUtility.ToJson(manifest, true) + "\n",
                new UTF8Encoding(false));

            Debug.Log($"[Mod SDK] Built modId={manifest.modId} version={manifest.version} catalog={manifest.catalog} output='{packagePath}'.");
            return packagePath;
        }

        private static void RequireWindowsBuildTarget()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
            {
                throw new InvalidOperationException(
                    "Active build target must be StandaloneWindows64. Switch the Unity project target, then build again.");
            }
        }

        private static string FindCatalog(string stagingRoot)
        {
            string[] catalogs = Directory.GetFiles(stagingRoot, "catalog_*.*", SearchOption.AllDirectories)
                .Where(path => path.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (catalogs.Length != 1)
                throw new InvalidOperationException($"Expected one remote catalog in '{stagingRoot}', found {catalogs.Length}.");

            return catalogs[0];
        }

        private static void RecreateOwnedDirectory(string path)
        {
            string buildsRoot = Path.GetFullPath(ModSdkPaths.BuildsRoot) + Path.DirectorySeparatorChar;
            string fullPath = Path.GetFullPath(path);
            if (!fullPath.StartsWith(buildsRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Refusing to clean a directory outside '{ModSdkPaths.BuildsRoot}'.");

            if (Directory.Exists(fullPath))
                Directory.Delete(fullPath, true);

            Directory.CreateDirectory(fullPath);
        }

        private static void CopyDirectory(string sourcePath, string destinationPath)
        {
            foreach (string directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(Path.Combine(destinationPath, Path.GetRelativePath(sourcePath, directory)));

            foreach (string file in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
            {
                string destinationFile = Path.Combine(destinationPath, Path.GetRelativePath(sourcePath, file));
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
                File.Copy(file, destinationFile, true);
            }
        }
    }
}