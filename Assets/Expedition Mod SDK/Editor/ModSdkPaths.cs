using System.IO;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    internal static class ModSdkPaths
    {
        public const string ModsRoot = "Assets/Mods";
        public static string SelectedModRoot = ModsRoot + "/DataAndPresentation";
        public static string ManifestAssetPath => SelectedModRoot + "/mod.json";
        public static string SampleContentAssetPath => SelectedModRoot + "/Content/sample-additive.txt";
        public const string AddressablesSettingsFolder = "Assets/AddressableAssetsData";
        public const string AddressablesSettingsName = "AddressableAssetSettings";
        public static string ContentGroupName => "Mod_" + ModProjectValidator.LoadManifest().modId;
        public const string ModIdProfileVariable = "Mod.Id";
        public const string RenderPipelineFolder = "Assets/Expedition Mod SDK/Settings";
        public const string RendererDataAssetPath = RenderPipelineFolder + "/ExpeditionModRenderer.asset";
        public const string RenderPipelineAssetPath = RenderPipelineFolder + "/ExpeditionModRenderPipeline.asset";

        public static string ProjectRoot => Directory.GetParent(Application.dataPath)!.FullName;
        public static string ManifestFullPath => Path.Combine(ProjectRoot, ManifestAssetPath);
        public static string BuildsRoot => Path.Combine(ProjectRoot, "Builds");
        public static string StagingRoot => Path.Combine(BuildsRoot, "Staging");
        public static string PackagesRoot => Path.Combine(BuildsRoot, "Packages");
    }
}