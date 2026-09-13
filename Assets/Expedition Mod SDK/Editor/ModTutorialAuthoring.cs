using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.Tutorials.Editor;
using Unity.Tutorials.Editor.Paragraphs;

namespace Expedition.ModSdk.Editor
{
    public static class ModTutorialAuthoring
    {
        public const string TutorialPath = "Assets/Tutorials/BuildAndInstallMods.asset";
        public const string EnglishTutorialPath = "Assets/Tutorials/BuildAndInstallModsEnglish.asset";
        private static readonly string[] RussianTitles =
        {
            "1. Рабочее окно",
            "2. Создание и разметка",
            "3. Проверка",
            "4. Сборка",
            "5. Установка",
            "6. Проверка в игре"
        };
        private static readonly string[] EnglishTitles =
        {
            "1. Workspace",
            "2. Create and label assets",
            "3. Validate",
            "4. Build",
            "5. Install",
            "6. Verify in game"
        };
        private static readonly string[] RussianText =
        {
            "Откройте Expedition > Mod SDK > Mod Workspace. В этом окне выбирают мод, проверяют его, собирают и устанавливают. Для первого прохождения выберите DataAndPresentation. Урок использует текущую сцену и не меняет её.",
            "Каждый мод находится в Assets/Mods/<Name>: mod.json описывает пакет, Content содержит его assets. Откройте папку выбранного мода и изучите manifest. Для нового предмета используйте Create > Expedition > Items > Item Config. Задайте уникальный Item Id, например sample.production/item/copper. Добавьте asset в группу Mod_<modId>, назначьте Address и label Catalog.Item. Для рецептов нужен Catalog.Recipe, для погоды — Catalog.Weather. Добавьте запись content с таким же address в mod.json. Опубликованные ID не меняйте. Полная инструкция: Docs/AssetAuthoring.md; в шаблоне — Assets/Expedition Mod SDK/Documentation/Docs/AssetAuthoring.md. В ней есть ссылки на официальную Unity Docs.",
            "Нажмите «1. Проверить / Validate». Исправьте ошибки в рабочем окне. Каждая запись content в manifest должна соответствовать одному asset в группе мода. Проверьте Address и точные Catalog.* labels. Прямые ссылки на assets соседнего мода запрещены: связанные Object-полями конфигурации держите в одном пакете. Для поддержанных связей по игровым ID объявляйте dependencies. Проверка структуры не доказывает игровой эффект.",
            "Нажмите «2. Собрать мод / Build selected» и дождитесь результата. В Builds/Packages/<mod-id>-<version> появятся mod.json, catalog, hash и bundles. Кнопка «Собрать все моды / Build all mods» собирает весь набор. Перед передачей выполните Expedition > Mod SDK > Verify Built Bundles. Передавайте целиком папку пакета, а не исходные assets. Игра и SDK должны использовать совместимые версии Editor, Addressables, URP и API.",
            "Закройте игру или остановите её Play Mode. Укажите папку Application.persistentDataPath/Mods именно игры и нажмите «3. Установить пакет / Install selected». Для Survival_2024 на Windows это обычно %USERPROFILE%/AppData/LocalLow/DefaultCompany/Survival_2024/Mods; путь зависит от Company/Product игры. Сначала установите зависимости. Не оставляйте рядом две версии одного modId. При ручной установке копируйте всю папку пакета из Builds/Packages с сохранением вложенных путей.",
            "Запустите игру заново: набор модов фиксируется при старте. Проверьте сообщения mod catalog ready, регистрацию у игрового владельца и использование контента. У sample.author текст по адресу sample.author/additive/sample-text должен совпадать с исходным TextAsset. Для локализованного контента проверьте RU и EN, для persistent контента — сохранение и загрузку. Наличие каталога не доказывает весь игровой сценарий. При обновлении остановите игру, уберите только прежнюю папку этого пакета и установите новую. Несовместимое сохранение должно дать явную ошибку, а не подмену содержимого."
        };
        private static readonly string[] EnglishText =
        {
            "Open Expedition > Mod SDK > Mod Workspace. This window lets you select, validate, build and install a mod. Choose DataAndPresentation for your first walkthrough. The tutorial uses the current scene without replacing it.",
            "Each mod lives in Assets/Mods/<Name>: mod.json describes the package and Content contains its assets. Open the selected mod folder and inspect its manifest. To create an item, choose Create > Expedition > Items > Item Config. Set a unique Item Id, such as sample.production/item/copper. Add the asset to group Mod_<modId>, assign its Address and the Catalog.Item label. Recipes use Catalog.Recipe; weather uses Catalog.Weather. Add a content entry with the same address to mod.json. Keep published IDs stable. The detailed authoring guide is Docs/AssetAuthoring.md, or Assets/Expedition Mod SDK/Documentation/Docs/AssetAuthoring.md in the template. The guide is currently in Russian and includes links to official Unity documentation.",
            "Click 1. Validate and resolve any errors shown in the workspace. Each manifest content entry must match one asset in the mod group. Check the Address and exact Catalog.* labels. Direct Unity references to another mod folder are not supported: keep configurations linked by Object fields in the same package. Declare dependencies for supported links using stable gameplay IDs. Structural validation does not prove gameplay behavior.",
            "Click 2. Build selected and wait for completion. Builds/Packages/<mod-id>-<version> will contain mod.json, a catalog, its hash and bundles. Build all mods builds the full set. Before distribution, run Expedition > Mod SDK > Verify Built Bundles. Distribute the entire package folder, not the source assets. The game and SDK must use compatible Editor, Addressables, URP and API versions.",
            "Close the game or stop its Play Mode. Choose the game's Application.persistentDataPath/Mods directory and click 3. Install selected. For Survival_2024 on Windows this is usually %USERPROFILE%/AppData/LocalLow/DefaultCompany/Survival_2024/Mods; the path depends on the game's Company/Product settings. Install dependencies first. Do not keep two versions of the same modId installed together. To install manually, copy the entire package folder from Builds/Packages while preserving its relative paths.",
            "Restart the game: the installed mod set is fixed at startup. Check the mod catalog ready messages, registration by the gameplay owner and actual use of the content. For sample.author, the text at sample.author/additive/sample-text must match the source TextAsset. Check both RU and EN for localized content and save/load for persistent content. A loaded catalog does not prove the complete gameplay scenario. To update a mod, stop the game, remove only that package's previous folder and install the new version. An incompatible save must produce an explicit error instead of silently substituting content."
        };

        [MenuItem("Expedition/Mod SDK/Create Tutorial Assets")]
        public static void Create()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Tutorials")) AssetDatabase.CreateFolder("Assets", "Tutorials");
            Tutorial russian = UpdateTutorial(TutorialPath, "expedition-mod-build-install", "Собрать и установить мод", "Далее", "Готово", RussianTitles, RussianText);
            Tutorial english = UpdateTutorial(EnglishTutorialPath, "expedition-mod-build-install-en", "Build and install a mod", "Next", "Done", EnglishTitles, EnglishText);
            const string containerPath = "Assets/Tutorials/ExpeditionMods.asset";
            TutorialContainer container = AssetDatabase.LoadAssetAtPath<TutorialContainer>(containerPath);
            if (container == null)
            {
                container = ScriptableObject.CreateInstance<TutorialContainer>();
                AssetDatabase.CreateAsset(container, containerPath);
            }
            Undo.RecordObject(container, "Update tutorial language selection");
            container.Title = "Expedition Mod SDK";
            container.Subtitle = "Выберите язык / Choose a language";
            container.Sections = new[]
            {
                new TutorialContainer.Section { Heading = "Русский", Text = "Создание, проверка, сборка и установка мода", Tutorial = russian, Type = TutorialContainer.SectionType.Tutorial },
                new TutorialContainer.Section { Heading = "English", Text = "Create, validate, build and install a mod", Tutorial = english, Type = TutorialContainer.SectionType.Tutorial }
            };
            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
            Debug.Log("[Mod SDK] Updated Russian and English tutorials: six pages per language.");
        }

        private static Tutorial UpdateTutorial(string path, string lessonId, string title, string next, string done, string[] titles, string[] texts)
        {
            Tutorial tutorial = AssetDatabase.LoadAssetAtPath<Tutorial>(path);
            if (tutorial == null)
            {
                tutorial = ScriptableObject.CreateInstance<Tutorial>();
                AssetDatabase.CreateAsset(tutorial, path);
            }
            if (tutorial.PageCount != 0 && tutorial.PageCount != titles.Length)
                throw new InvalidOperationException("Expected six SDK tutorial pages: " + path);
            Undo.RecordObject(tutorial, "Translate SDK tutorial");
            tutorial.TutorialTitle = title;
            tutorial.LessonId = lessonId;
            tutorial.Version = "1";
            tutorial.SceneManagementBehavior = Tutorial.SceneManagementBehaviorType.UseActiveScene;
            tutorial.ReturnToPreviousScenes = false;
            int[] steps = { 0, -1, 1, 2, 3, -1 };
            for (int index = 0; index < titles.Length; index++)
            {
                bool creating = tutorial.PageCount <= index;
                TutorialPage page = creating ? ScriptableObject.CreateInstance<TutorialPage>() : tutorial.PagesCollection[index];
                if (creating)
                {
                    AssetDatabase.AddObjectToAsset(page, tutorial);
                    tutorial.PagesCollection.AddItem(page);
                }
                Undo.RecordObject(page, "Translate tutorial page");
                page.name = titles[index];
                page.Title = titles[index];
                page.NextButton = next;
                page.DoneButton = done;
                InstructionsParagraph paragraph;
                if (creating)
                {
                    paragraph = ScriptableObject.CreateInstance<InstructionsParagraph>();
                    AssetDatabase.AddObjectToAsset(paragraph, tutorial);
                    page.Paragraphs.Add(paragraph);
                    if (steps[index] >= 0)
                    {
                        var criterion = ScriptableObject.CreateInstance<ModTutorialCriterion>();
                        criterion.name = "Step " + steps[index];
                        criterion.Step = steps[index];
                        AssetDatabase.AddObjectToAsset(criterion, tutorial);
                        paragraph.Criterias().AddItem(new TypedCriterion(new SerializedType(typeof(ModTutorialCriterion)), criterion));
                    }
                }
                else paragraph = page.Paragraphs.OfType<InstructionsParagraph>().Single();
                Undo.RecordObject(paragraph, "Translate tutorial instructions");
                paragraph.name = titles[index] + " Instructions";
                paragraph.Title = titles[index];
                paragraph.Text = texts[index];
                EditorUtility.SetDirty(paragraph);
                EditorUtility.SetDirty(page);
            }
            EditorUtility.SetDirty(tutorial);
            return tutorial;
        }
    }
}
