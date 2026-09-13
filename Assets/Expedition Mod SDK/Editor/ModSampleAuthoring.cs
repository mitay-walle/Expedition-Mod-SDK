using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Expedition.ModApi;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Items;
using Recipes;
using Game.Audio;
using Game.Environment.Weather;
using Game.Environment.DayNights;
using Progression;
using Research;

namespace Expedition.ModSdk.Editor
{
    public static class ModSampleAuthoring
    {
        [MenuItem("Expedition/Mod SDK/Create Test Mods")]
        public static void Create()
        {
            Folder("Assets/Mods");
            string first = "Assets/Mods/DataAndPresentation";
            if (AssetDatabase.IsValidFolder("Assets/Expedition Mod SDK/Mod") && !AssetDatabase.IsValidFolder(first))
            {
                string error = AssetDatabase.MoveAsset("Assets/Expedition Mod SDK/Mod", first);
                if (!string.IsNullOrEmpty(error)) throw new InvalidOperationException(error);
            }
            Locale[] locales = new[] { GetLocale("en"), GetLocale("ru") };
            if (AddressableAssetSettingsDefaultObject.Settings.FindGroup("Mod_sample.author") == null) Presentation(first);
            if (!File.Exists("Assets/Mods/Production/mod.json")) Production("Assets/Mods/Production", locales);
            if (!File.Exists("Assets/Mods/World/mod.json")) World("Assets/Mods/World");
            if (!File.Exists("Assets/Mods/ResearchAndEvents/mod.json")) Events("Assets/Mods/ResearchAndEvents", locales);
            ModSdkPaths.SelectedModRoot = first;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ModTutorialAuthoring.Create();
        }

        private static void Presentation(string root)
        {
            Folder(root + "/Content");
            Texture2D icon = Icon(root + "/Content/SurveyIcon.png", new Color(0.2f, 0.85f, 0.7f));
            string audioPath = root + "/Content/Footstep.wav";
            WriteSound(audioPath);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);
            var layered = Asset<LayeredAudioClip>(root + "/Content/FootstepLayers.asset");
            layered.Layers.Clear(); var layer = new LayeredAudioClipLayer(); layer.Clips.Add(clip); layered.Layers.Add(layer);
            var profile = Asset<FootstepAudioProfile>(root + "/Content/FootstepProfile.asset");
            Set(profile, "<DefaultSound>k__BackingField", layered);
            Configure(root, "sample.author", "Данные и оформление / Data and presentation");
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            var textEntry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(root + "/Content/sample-additive.txt"));
            textEntry.SetAddress("sample.author/additive/sample-text");
            WriteManifest(root, "sample.author", "Данные и оформление / Data and presentation");
            EditorUtility.SetDirty(layered);
        }

        private static void Production(string root, Locale[] locales)
        {
            Folder(root + "/Content");
            var strings = Strings(root, "sample.production", locales);
            var resource = Item(root, "sample.production", "Mineral", strings, "Минерал", "Mineral");
            var consumable = Item(root, "sample.production", "Restorative", strings, "Восстановитель", "Restorative");
            var serialized = new SerializedObject(consumable);
            var features = serialized.FindProperty("_componentConfigs"); features.arraySize = 1;
            features.GetArrayElementAtIndex(0).managedReferenceValue = new ValueChangeConfig("Health", new ParticleSystem.MinMaxCurve(15));
            serialized.ApplyModifiedPropertiesWithoutUndo();
            var recipe = Asset<ItemRecipeConfig>(root + "/Content/RestorativeRecipe.asset");
            Set(recipe, "_recipeId", "sample.production/recipe/restorative"); Set(recipe, "_result", consumable);
            var recipeData = new SerializedObject(recipe);
            var ingredients = recipeData.FindProperty("_ingredients"); ingredients.arraySize = 1;
            ingredients.GetArrayElementAtIndex(0).FindPropertyRelative("_item").objectReferenceValue = resource;
            ingredients.GetArrayElementAtIndex(0).FindPropertyRelative("_count").intValue = 2;
            var stations = recipeData.FindProperty("_craftingStationIds"); stations.arraySize = 1; stations.GetArrayElementAtIndex(0).stringValue = "Crafting/FieldPrinter";
            recipeData.ApplyModifiedPropertiesWithoutUndo(); recipe.Validate(); resource.Validate(); consumable.Validate();
            Configure(root, "sample.production", "Предметы и производство / Items and production");
        }

        private static void World(string root)
        {
            Folder(root + "/Content");
            var volume = Asset<VolumeProfile>(root + "/Content/AmberAtmosphere.asset");
            var grading = volume.Add<ColorAdjustments>(true); grading.colorFilter.Override(new Color(1f, 0.78f, 0.5f));
            AssetDatabase.AddObjectToAsset(grading, volume);
            var weather = Asset<WeatherDefinition>(root + "/Content/AmberWeather.asset");
            Set(weather, "<Id>k__BackingField", "sample.world/weather/amber"); Set(weather, "<Profile>k__BackingField", volume);
            var body = Asset<CelestialBodyDefinition>(root + "/Content/AmberMoon.asset");
            Set(body, "<Texture>k__BackingField", Icon(root + "/Content/AmberMoon.png", new Color(1f, 0.65f, 0.3f)));
            Set(body, "<Sphere>k__BackingField", false);
            EditorUtility.SetDirty(volume);
            Configure(root, "sample.world", "Окружение / Environment");
        }

        private static void Events(string root, Locale[] locales)
        {
            Folder(root + "/Content");
            var strings = Strings(root, "sample.events", locales);
            var source = Item(root, "sample.events", "Sample", strings, "Образец минерала", "Mineral sample");
            var milestone = Asset<MilestoneConfig>(root + "/Content/MineralResearch.asset");
            Set(milestone, "_milestoneId", "sample.events/milestone/mineral");
            SetLocalized(milestone, "_title", Text(strings, "Milestone", "Исследование минерала", "Mineral research"));
            var sample = Asset<ResearchSampleConfig>(root + "/Content/MineralSample.asset");
            Set(sample, "<Milestone>k__BackingField", milestone); Set(sample, "<SourceItem>k__BackingField", source);
            SetLocalized(sample, "<Description>k__BackingField", Text(strings, "Research", "Передайте образец учёному для анализа.", "Submit the sample to the scientist for analysis."));
            sample.Validate();
            var dialogue = Asset<Dialogue.DialogueConfig>(root + "/Content/ResearchDialogue.asset");
            string audioPath = root + "/Content/DialogueTiming.wav";
            WriteSound(audioPath, 176400);
            dialogue.SetImportedData(AssetDatabase.AssetPathToGUID(audioPath), AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath), new[] {
                new Dialogue.DialogueCaption(0, 4, Text(strings, "Line1", "Ты принёс образец? Начнём исследование.", "You brought the sample? Let's begin the analysis.")),
                new Dialogue.DialogueCaption(4, 8, Text(strings, "Line2", "Результаты помогут подготовить следующий опыт.", "The results will help prepare the next experiment.")) });
            EditorUtility.SetDirty(dialogue);
            Configure(root, "sample.events", "Исследование и диалог / Research and dialogue");
        }

        private static ItemConfig Item(string root, string modId, string name, StringTableCollection strings, string ru, string en)
        {
            var item = Asset<ItemConfig>(root + "/Content/" + name + ".asset");
            Set(item, "_itemId", modId + "/item/" + name.ToLowerInvariant()); Set(item, "_category", "Resources");
            SetLocalized(item, "_name", Text(strings, name, ru, en));
            SetLocalized(item, "_description", Text(strings, name + ".Description", "Тестовый контент SDK.", "SDK test content."));
            return item;
        }

        private static StringTableCollection Strings(string root, string id, Locale[] locales)
        {
            Folder(root + "/Content/Localization");
            return LocalizationEditorSettings.GetStringTableCollection(id) ?? LocalizationEditorSettings.CreateStringTableCollection(id, root + "/Content/Localization", locales);
        }

        private static LocalizedString Text(StringTableCollection collection, string key, string ru, string en)
        {
            foreach (StringTable table in collection.StringTables)
            { table.AddEntry(key, table.LocaleIdentifier.Code == "ru" ? ru : en); EditorUtility.SetDirty(table); }
            EditorUtility.SetDirty(collection.SharedData);
            return new LocalizedString(collection.SharedData.TableCollectionNameGuid, key);
        }

        private static void SetLocalized(UnityEngine.Object target, string property, LocalizedString value)
        {
            Type type = target.GetType();
            System.Reflection.FieldInfo field = null;
            while (type != null && field == null) { field = type.GetField(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public); type = type.BaseType; }
            if (field == null) throw new InvalidOperationException(property);
            field.SetValue(target, value); EditorUtility.SetDirty(target);
        }

        private static Locale GetLocale(string code)
        {
            Locale existing = LocalizationEditorSettings.GetLocales().FirstOrDefault(locale => locale.Identifier.Code == code);
            if (existing != null) return existing;
            Folder("Assets/Expedition Mod SDK/Settings/Locales");
            Locale locale = Locale.CreateLocale(code);
            AssetDatabase.CreateAsset(locale, "Assets/Expedition Mod SDK/Settings/Locales/" + code + ".asset");
            LocalizationEditorSettings.AddLocale(locale); return locale;
        }

        private static void Configure(string root, string id, string title)
        {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            settings.profileSettings.CreateValue(ModSdkPaths.ModIdProfileVariable, id);
            settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kRemoteBuildPath, "[UnityEngine.Application.dataPath]/../Builds/Staging/[BuildTarget]");
            settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kRemoteLoadPath, "expedition-mod://[Mod.Id]/[BuildTarget]");
            settings.BuildRemoteCatalog = true;
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteBuildPath);
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteLoadPath);
            var group = settings.FindGroup("Mod_" + id) ?? settings.CreateGroup("Mod_" + id, false, false, false, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            var schema = group.GetSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteBuildPath);
            schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteLoadPath);
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            foreach (string guid in AssetDatabase.FindAssets("", new[] { root + "/Content" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(path)) continue;
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset == null || asset is StringTableCollection) continue;
                if (asset is VolumeProfile) { settings.RemoveAssetEntry(guid); continue; }
                var entry = settings.CreateOrMoveEntry(guid, group);
                entry.SetAddress(id + "/" + Path.GetRelativePath(root + "/Content", path).Replace('\\', '/').ToLowerInvariant());
                string label = asset switch { ItemConfig => "Item", ItemRecipeConfig => "Recipes", WeatherDefinition => "Weather", MilestoneConfig => "Milestone", ResearchSampleConfig => "ResearchSample", _ => "Mod.Example" };
                settings.AddLabel(label); entry.SetLabel(label, true);
            }
            EditorUtility.SetDirty(group); EditorUtility.SetDirty(settings); EditorUtility.SetDirty(schema);
            WriteManifest(root, id, title);
        }

        private static void WriteManifest(string root, string id, string title)
        {
            var group = AddressableAssetSettingsDefaultObject.Settings.FindGroup("Mod_" + id);
            var manifest = new ModManifest { schemaVersion = ModContract.SchemaVersion, modId = id, version = "0.1.0", displayName = title,
                description = "SDK authoring and bundle transport example. Gameplay verification is tracked separately.", minimumGameVersion = "0.1.0", catalog = "" };
            foreach (var entry in group.entries.OrderBy(e => e.address, StringComparer.Ordinal))
            {
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(entry.guid));
                string kind = asset is Texture ? ModContract.KindTexture : asset is AudioClip ? ModContract.KindAudio : ModContract.KindData;
                manifest.content.Add(new ModContentEntry { id = entry.address, address = entry.address, kind = kind, mode = ModContract.ModeAdditive, targetId = "" });
            }
            File.WriteAllText(root + "/mod.json", JsonUtility.ToJson(manifest, true) + "\n");
            AssetDatabase.ImportAsset(root + "/mod.json");
        }

        private static T Asset<T>(string path) where T : ScriptableObject
        { var existing = AssetDatabase.LoadAssetAtPath<T>(path); if (existing != null) return existing; var asset = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset, path); return asset; }
        private static void Folder(string path)
        { if (AssetDatabase.IsValidFolder(path)) return; string parent = Path.GetDirectoryName(path).Replace('\\', '/'); Folder(parent); AssetDatabase.CreateFolder(parent, Path.GetFileName(path)); }
        private static void Set(UnityEngine.Object asset, string name, object value)
        {
            var data = new SerializedObject(asset); var property = data.FindProperty(name);
            if (property == null) throw new InvalidOperationException(asset.name + ": " + name);
            if (value is string text) property.stringValue = text;
            else if (value is bool flag) property.boolValue = flag;
            else property.objectReferenceValue = (UnityEngine.Object)value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
        private static Texture2D Icon(string path, Color color)
        {
            var texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
            { float radius = Vector2.Distance(new Vector2(x, y), new Vector2(63.5f, 63.5f)); texture.SetPixel(x, y, radius < 55 ? color * (0.45f + 0.55f * (1 - radius / 55)) : Color.clear); }
            texture.Apply(); File.WriteAllBytes(path, texture.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path); return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        private static void WriteSound(string path, int samples = 6615)
        {
            const int rate = 22050;
            using (var output = new BinaryWriter(File.Create(path)))
            {
                output.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); output.Write(36 + samples * 2);
                output.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt ")); output.Write(16); output.Write((short)1); output.Write((short)1);
                output.Write(rate); output.Write(rate * 2); output.Write((short)2); output.Write((short)16);
                output.Write(System.Text.Encoding.ASCII.GetBytes("data")); output.Write(samples * 2);
                var random = new System.Random(713);
                for (int i = 0; i < samples; i++) { double t = (double)i / rate; output.Write((short)(10000 * Math.Exp(-t * 24) * (0.6 * Math.Sin(t * 2 * Math.PI * 110) + 0.4 * (random.NextDouble() * 2 - 1)))); }
            }
            AssetDatabase.ImportAsset(path);
        }
    }
}
