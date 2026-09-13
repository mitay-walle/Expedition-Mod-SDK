using System;
using System.IO;
using System.Linq;
using System.Text;
using Expedition.ModApi;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.Collections.Generic;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    public static class ModBuildCommand
    {
        [MenuItem("Expedition/Mod SDK/Validate Mod")]
        public static void ValidateFromMenu()
        {
            ModManifest manifest = PrepareAndValidate();
            Debug.Log($"[Mod SDK] Validation passed modId={manifest.modId} version={manifest.version} content={manifest.content.Count}.");

        }

        [MenuItem("Expedition/Mod SDK/Build Mod")]
        public static void BuildFromMenu()
        {
            string packagePath = Build();
            EditorUtility.RevealInFinder(packagePath);

        }

        public static void BuildFromCommandLine()
        {
            try
            {
                string[] arguments = Environment.GetCommandLineArgs();
                int index = Array.IndexOf(arguments, "-mod");
                if (index >= 0 && index + 1 < arguments.Length && arguments[index + 1] != "all")
                {
                    string id = arguments[index + 1];
                    string root = DiscoverMods().Single(path =>
                        JsonUtility.FromJson<ModManifest>(File.ReadAllText(Path.Combine(path, "mod.json"))).modId == id);
                    ModSdkPaths.SelectedModRoot = root;
                    Build();
                }
                else BuildAll();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        [MenuItem("Expedition/Mod SDK/Export API")]
        public static void ExportApi()
        {
            string output = Path.Combine(ModSdkPaths.BuildsRoot, "Expedition.ModApi.unitypackage");
            Directory.CreateDirectory(ModSdkPaths.BuildsRoot);
            AssetDatabase.ExportPackage("Assets/Expedition Mod SDK/Runtime", output, ExportPackageOptions.Recurse);
            Debug.Log($"[Mod SDK] Exported API {ModContract.ApiVersion} to '{output}'.");
        }

        public static string[] DiscoverMods()
        {
            if (!Directory.Exists(ModSdkPaths.ModsRoot)) return Array.Empty<string>();
            return Directory.GetDirectories(ModSdkPaths.ModsRoot)
                .Where(path => File.Exists(Path.Combine(path, "mod.json")))
                .Select(path => path.Replace('\\', '/')).OrderBy(path => path, StringComparer.Ordinal).ToArray();
        }

        [MenuItem("Expedition/Mod SDK/Build All Mods")]
        public static void BuildAllFromMenu() => Debug.Log(string.Join("\n", BuildAll()));

        public static string[] BuildAll()
        {
            string previous = ModSdkPaths.SelectedModRoot;
            var outputs = new List<string>();
            try
            {
                foreach (string root in DiscoverMods())
                {
                    ModSdkPaths.SelectedModRoot = root;
                    outputs.Add(Build());
                }
                if (outputs.Count == 0) throw new InvalidOperationException("No mods found under Assets/Mods.");
                ModPackageValidation.ValidateAndOrder(outputs.Select(path => new ModPackage(path,
                    JsonUtility.FromJson<ModManifest>(File.ReadAllText(Path.Combine(path, "mod.json"))))), "0.1.0", Application.unityVersion);
                return outputs.ToArray();
            }
            finally { ModSdkPaths.SelectedModRoot = previous; }
        }

        public static ModManifest PrepareAndValidate()
        {
            ModManifest manifest = ModProjectValidator.LoadManifest();
            ModProjectValidator.ValidateManifest(manifest);
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(ModSdkPaths.ContentGroupName);
            if (group == null) throw new InvalidOperationException("Configure the mod Addressables group first.");
            // Localization reassigns groups and addresses when importing a collection into a clean project.
            foreach (ModContentEntry content in manifest.content)
            {
                string prefix = manifest.modId + "/";
                if (!content.address.StartsWith(prefix, StringComparison.Ordinal)) continue;
                string path = ModSdkPaths.SelectedModRoot + "/Content/" + content.address.Substring(prefix.Length);
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (!(asset is UnityEngine.Localization.Tables.LocalizationTable) &&
                    !(asset is UnityEngine.Localization.Tables.SharedTableData)) continue;
                string guid = AssetDatabase.AssetPathToGUID(path);
                var entry = settings.CreateOrMoveEntry(guid, group);
                entry.SetAddress(content.address);
            }
            EditorUtility.SetDirty(group);
            AssetDatabase.SaveAssets();
            return ModProjectValidator.LoadAndValidate();
        }

        public static string Build()
        {
            RequireWindowsBuildTarget();
            ModManifest manifest = PrepareAndValidate();
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            RecreateOwnedDirectory(ModSdkPaths.StagingRoot);
            var included = new Dictionary<BundledAssetGroupSchema, bool>();
            AddressablesPlayerBuildResult result;
            bool previousBuildLayout = ProjectConfigData.GenerateBuildLayout;
            var previousScriptsNaming = settings.MonoScriptBundleNaming;
            string previousScriptsPrefix = settings.MonoScriptBundleCustomNaming;
            var previousBuiltInNaming = settings.BuiltInBundleNaming;
            string previousBuiltInPrefix = settings.BuiltInBundleCustomNaming;
            AddressableAssetGroup previousDefaultGroup = settings.DefaultGroup;
            string previousId = settings.profileSettings.GetValueByName(settings.activeProfileId, ModSdkPaths.ModIdProfileVariable);
            try
            {
                foreach (AddressableAssetGroup group in settings.groups.Where(group => group != null))
                {
                    BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
                    if (schema == null) continue;
                    included.Add(schema, schema.IncludeInBuild);
                    schema.IncludeInBuild = group.Name == ModSdkPaths.ContentGroupName;
                }
                settings.profileSettings.SetValue(settings.activeProfileId, ModSdkPaths.ModIdProfileVariable, manifest.modId);
                settings.DefaultGroup = settings.FindGroup(ModSdkPaths.ContentGroupName);
                settings.MonoScriptBundleNaming = MonoScriptBundleNaming.Custom;
                settings.MonoScriptBundleCustomNaming = manifest.modId;
                settings.BuiltInBundleNaming = BuiltInBundleNaming.Custom;
                settings.BuiltInBundleCustomNaming = manifest.modId;
                ProjectConfigData.GenerateBuildLayout = true;
                AddressableAssetSettings.BuildPlayerContent(out result);
            }
            finally
            {
                settings.MonoScriptBundleNaming = previousScriptsNaming;
                settings.MonoScriptBundleCustomNaming = previousScriptsPrefix;
                settings.BuiltInBundleNaming = previousBuiltInNaming;
                settings.BuiltInBundleCustomNaming = previousBuiltInPrefix;
                settings.DefaultGroup = previousDefaultGroup;
                ProjectConfigData.GenerateBuildLayout = previousBuildLayout;
                foreach (var entry in included) entry.Key.IncludeInBuild = entry.Value;
                settings.profileSettings.SetValue(settings.activeProfileId, ModSdkPaths.ModIdProfileVariable, previousId);
                AssetDatabase.SaveAssets();
            }
            if (!string.IsNullOrWhiteSpace(result.Error))
                throw new InvalidOperationException($"Addressables build failed: {result.Error}");

            string catalogPath = FindCatalog(ModSdkPaths.StagingRoot);
            string packagePath = Path.Combine(ModSdkPaths.PackagesRoot, $"{manifest.modId}-{manifest.version}");
            RecreateOwnedDirectory(packagePath);
            CopyDirectory(ModSdkPaths.StagingRoot, packagePath);

            manifest.apiVersion = ModContract.ApiVersion;
            manifest.unityVersion = Application.unityVersion;
            manifest.addressablesVersion = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(UnityEngine.AddressableAssets.Addressables).Assembly).version;
            manifest.renderPipelineVersion = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(UnityEngine.Rendering.Universal.UniversalRenderPipeline).Assembly).version;
            manifest.buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
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