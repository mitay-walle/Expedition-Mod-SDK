using System;
using Expedition.ModApi;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Expedition.ModSdk.Editor
{
    public static class ModSdkProjectBootstrap
    {
        [MenuItem("Expedition/Mod SDK/Configure Project")]
        public static void Configure()
        {
            ModManifest manifest = ModProjectValidator.LoadManifest();
            ModProjectValidator.ValidateManifest(manifest);

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            ConfigureProfile(settings, manifest.modId);
            ConfigureCatalog(settings);
            AddressableAssetGroup group = ConfigureContentGroup(settings);
            ConfigureSampleContent(settings, group, manifest.content[0].address);
            ConfigureRenderPipeline();

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Mod SDK] Configured project modId={manifest.modId} group='{ModSdkPaths.ContentGroupName}'.");
        }

        public static void ConfigureFromCommandLine()
        {
            try
            {
                Configure();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void ConfigureProfile(AddressableAssetSettings settings, string modId)
        {
            settings.profileSettings.CreateValue(ModSdkPaths.ModIdProfileVariable, modId);
            settings.profileSettings.SetValue(settings.activeProfileId, ModSdkPaths.ModIdProfileVariable, modId);
            settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kRemoteBuildPath,
                "[UnityEngine.Application.dataPath]/../Builds/Staging/[BuildTarget]");
            settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kRemoteLoadPath,
                "expedition-mod://[Mod.Id]/[BuildTarget]");
        }

        private static void ConfigureCatalog(AddressableAssetSettings settings)
        {
            settings.BuildRemoteCatalog = true;
            settings.EnableJsonCatalog = false;
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteBuildPath);
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteLoadPath);
        }

        private static AddressableAssetGroup ConfigureContentGroup(AddressableAssetSettings settings)
        {
            AddressableAssetGroup group = settings.FindGroup(ModSdkPaths.ContentGroupName);
            if (group == null)
            {
                group = settings.CreateGroup(ModSdkPaths.ContentGroupName, false, false, false, null,
                    typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            }

            BundledAssetGroupSchema bundleSchema = group.GetSchema<BundledAssetGroupSchema>();
            bundleSchema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteBuildPath);
            bundleSchema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteLoadPath);
            bundleSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            bundleSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
            EditorUtility.SetDirty(bundleSchema);
            return group;
        }

        private static void ConfigureRenderPipeline()
        {
            if (!AssetDatabase.IsValidFolder(ModSdkPaths.RenderPipelineFolder))
                AssetDatabase.CreateFolder("Assets/Expedition Mod SDK", "Settings");

            UniversalRendererData rendererData =
                AssetDatabase.LoadAssetAtPath<UniversalRendererData>(ModSdkPaths.RendererDataAssetPath);
            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(rendererData, ModSdkPaths.RendererDataAssetPath);
            }

            UniversalRenderPipelineAsset pipelineAsset =
                AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(ModSdkPaths.RenderPipelineAssetPath);
            if (pipelineAsset == null)
            {
                pipelineAsset = UniversalRenderPipelineAsset.Create(rendererData);
                AssetDatabase.CreateAsset(pipelineAsset, ModSdkPaths.RenderPipelineAssetPath);
            }

            GraphicsSettings.defaultRenderPipeline = pipelineAsset;
            QualitySettings.renderPipeline = pipelineAsset;
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(pipelineAsset);
        }

        private static void ConfigureSampleContent(
            AddressableAssetSettings settings,
            AddressableAssetGroup group,
            string address)
        {
            string guid = AssetDatabase.AssetPathToGUID(ModSdkPaths.SampleContentAssetPath);
            if (string.IsNullOrWhiteSpace(guid))
                throw new InvalidOperationException($"Sample content is missing: {ModSdkPaths.SampleContentAssetPath}");

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.SetAddress(address, false);
            EditorUtility.SetDirty(group);
        }
    }
}