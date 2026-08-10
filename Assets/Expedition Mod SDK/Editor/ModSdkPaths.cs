using System.IO;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    internal static class ModSdkPaths
    {
        public const string ManifestAssetPath = "Assets/Expedition Mod SDK/Mod/mod.json";
        public const string SampleContentAssetPath = "Assets/Expedition Mod SDK/Mod/Content/sample-additive.txt";
        public const string AddressablesSettingsFolder = "Assets/AddressableAssetsData";
        public const string AddressablesSettingsName = "AddressableAssetSettings";
        public const string ContentGroupName = "Expedition Mod Content";
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