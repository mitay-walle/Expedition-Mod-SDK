using System;
using System.IO;
using System.Linq;
using Expedition.ModApi;
using UnityEditor;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    public sealed class ModWorkspaceWindow : EditorWindow
    {
        private string[] _roots = Array.Empty<string>();
        private int _selected;
        private string _status;
        private bool _failed;
        private string _modsDirectory;
        public static string LastValidatedRoot { get; private set; }
        public static string LastBuiltRoot { get; private set; }
        public static string LastInstalledRoot { get; private set; }

        [MenuItem("Expedition/Mod SDK/Mod Workspace")]
        public static void Open() => GetWindow<ModWorkspaceWindow>("Expedition Mods");

        private void OnEnable()
        {
            _roots = ModBuildCommand.DiscoverMods();
            _modsDirectory = EditorPrefs.GetString("Expedition.ModSdk.GameModsDirectory", "");
            _selected = Math.Max(0, Array.IndexOf(_roots, ModSdkPaths.SelectedModRoot));
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Тестовые моды / Test mods", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Каждый мод хранится в своей папке Assets/Mods. Выберите пакет, проверьте его и соберите.", MessageType.Info);
            if (GUILayout.Button("Обновить список / Refresh")) OnEnable();
            if (_roots.Length == 0) { EditorGUILayout.HelpBox("В Assets/Mods нет manifest mod.json.", MessageType.Warning); return; }
            _selected = EditorGUILayout.Popup("Мод / Mod", _selected, _roots.Select(Path.GetFileName).ToArray());
            ModSdkPaths.SelectedModRoot = _roots[_selected];
            if (GUILayout.Button("Открыть папку мода / Select mod folder")) Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(ModSdkPaths.SelectedModRoot);
            if (GUILayout.Button("1. Проверить / Validate")) Run(() =>
            {
                ModBuildCommand.PrepareAndValidate();
                LastValidatedRoot = ModSdkPaths.SelectedModRoot;
                return "Проверка пройдена / Validation passed";
            });
            if (GUILayout.Button("2. Собрать мод / Build selected")) Run(() =>
            {
                string output = ModBuildCommand.Build();
                LastBuiltRoot = ModSdkPaths.SelectedModRoot;
                return output;
            });
            if (GUILayout.Button("Собрать все моды / Build all")) Run(() => string.Join("\n", ModBuildCommand.BuildAll()));
            if (GUILayout.Button("Открыть готовые пакеты / Open packages")) EditorUtility.RevealInFinder(ModSdkPaths.PackagesRoot);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Установка / Installation", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Закройте игру. Выберите её папку persistentDataPath/Mods (не папку SDK и не папку с exe). После установки запустите игру заново.", MessageType.Info);
            _modsDirectory = EditorGUILayout.TextField("Папка Mods игры", _modsDirectory);
            if (GUILayout.Button("Выбрать папку / Browse"))
            {
                string selected = EditorUtility.OpenFolderPanel("Выберите persistentDataPath/Mods игры", _modsDirectory, "");
                if (!string.IsNullOrEmpty(selected)) _modsDirectory = selected;
            }
            if (GUILayout.Button("3. Установить пакет / Install selected")) Run(() =>
            {
                ModManifest manifest = ModProjectValidator.LoadManifest();
                string package = Path.Combine(ModSdkPaths.PackagesRoot, $"{manifest.modId}-{manifest.version}");
                string installed = ModPackageInstaller.Install(package, _modsDirectory);
                EditorPrefs.SetString("Expedition.ModSdk.GameModsDirectory", _modsDirectory);
                LastInstalledRoot = ModSdkPaths.SelectedModRoot;
                return installed + "\nПерезапустите игру / Restart the game";
            });
            EditorGUILayout.Space();
            if (GUILayout.Button("Обучение / Tutorial")) EditorApplication.ExecuteMenuItem("Tutorials/Show Tutorials Window");
            if (!string.IsNullOrEmpty(_status)) EditorGUILayout.HelpBox(_status, _failed ? MessageType.Error : MessageType.Info);
        }

        private void Run(Func<string> operation)
        {
            try { _status = operation(); _failed = false; }
            catch (Exception exception) { _status = exception.Message; _failed = true; Debug.LogException(exception); }
        }
    }
}
