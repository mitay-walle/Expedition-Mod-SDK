using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Tutorials.Editor;
using Unity.Tutorials.Editor.Paragraphs;

namespace Expedition.ModSdk.Editor
{
    public static class ModTutorialAuthoring
    {
        public const string TutorialPath = "Assets/Tutorials/BuildAndInstallMods.asset";
        [MenuItem("Expedition/Mod SDK/Create Tutorial Assets")]
        public static void Create()
        {
            if (AssetDatabase.LoadAssetAtPath<Tutorial>(TutorialPath) != null) return;
            if (!AssetDatabase.IsValidFolder("Assets/Tutorials")) AssetDatabase.CreateFolder("Assets", "Tutorials");
            var tutorial = ScriptableObject.CreateInstance<Tutorial>();
            tutorial.TutorialTitle = "Собрать и установить мод / Build and install a mod";
            tutorial.LessonId = "expedition-mod-build-install";
            tutorial.Version = "1";
            tutorial.SceneManagementBehavior = Tutorial.SceneManagementBehaviorType.UseActiveScene;
            tutorial.ReturnToPreviousScenes = false;
            AssetDatabase.CreateAsset(tutorial, TutorialPath);
            var pages = new List<TutorialPage>();
            AddPage(tutorial, pages, "1. Рабочее окно / Workspace",
                "Откройте Expedition > Mod SDK > Mod Workspace. Все операции сборки и установки доступны в этом окне.\nOpen Expedition > Mod SDK > Mod Workspace.", 0);
            AddPage(tutorial, pages, "2. Папка и manifest / Folder and manifest",
                "Инструкция создания и разметки: Docs/AssetAuthoring.md (в шаблоне: Assets/Expedition Mod SDK/Documentation/Docs/AssetAuthoring.md). Создавайте конфигурации через Create, назначайте Address, Catalog.* labels и запись content в mod.json. Выберите DataAndPresentation. Каждый мод живёт в своей папке Assets/Mods/<Name>: mod.json описывает стабильные ID, Content содержит исходные assets. Общий API находится отдельно и использует asmdef.\nChoose DataAndPresentation. Each mod has its own manifest and Content folder. The common source API is separate. Нажмите «Открыть папку мода» и изучите mod.json. Не меняйте опубликованные ID.", -1);
            AddPage(tutorial, pages, "3. Проверка / Validate",
                "Нажмите «1. Проверить». Исправьте ошибки, показанные в окне. Каждый адрес manifest должен соответствовать asset в группе выбранного мода. Прямые ссылки на assets соседнего мода запрещены: используйте стабильный ID и dependencies.\nClick Validate. Every manifest address must match an entry in this mod's Addressables group.", 1);
            AddPage(tutorial, pages, "4. Сборка / Build",
                "Нажмите «2. Собрать мод». Дождитесь результата. В Builds/Packages/<mod-id>-<version> появятся mod.json, каталог, hash и bundles. Для всего набора есть «Собрать все моды».\nClick Build selected and wait. Distribute the entire output folder; source assets are not an installable package.", 2);
            AddPage(tutorial, pages, "5. Установка / Install",
                "Закройте игру. Укажите её Application.persistentDataPath/Mods и нажмите «3. Установить пакет». В локальной сборке Survival_2024 это обычно %USERPROFILE%/AppData/LocalLow/DefaultCompany/Survival_2024/Mods. Путь зависит от Company/Product игры. Сначала установите зависимости; рядом не должно быть двух версий одного modId.\nClose the game, choose its persistentDataPath/Mods folder and click Install selected. Install dependencies first. Для ручной установки скопируйте всю папку пакета, сохранив вложенные пути.", 3);
            AddPage(tutorial, pages, "6. Проверка в игре / Verify in game",
                "Запустите игру заново: набор модов фиксируется при старте. Проверьте сообщение Mod catalog ready и фактическое использование контента. Для sample.author текст по адресу sample.author/additive/sample-text должен совпасть с исходным TextAsset. Проверка файлов и сборка не доказывают игровой эффект.\nRestart the game. Verify catalog loading and actual gameplay use. Other sample configurations still require their domain's runtime integration; see Docs/CreatingAMod.md. Для обновления закройте игру и удалите только старую папку конкретного мода, затем установите новую. Отсутствующий мод в сохранении должен быть явной ошибкой совместимости.", -1);
            tutorial.PagesCollection.SetItems(pages);
            EditorUtility.SetDirty(tutorial);
            var container = ScriptableObject.CreateInstance<TutorialContainer>();
            container.Title = "Expedition Mod SDK";
            container.Subtitle = "От исходных assets до установленного пакета / From assets to installed package";
            container.Sections = new[] { new TutorialContainer.Section { Heading = tutorial.TutorialTitle,
                Text = "Выбор мода, проверка, сборка и установка / Select, validate, build and install",
                Tutorial = tutorial, Type = TutorialContainer.SectionType.Tutorial } };
            AssetDatabase.CreateAsset(container, "Assets/Tutorials/ExpeditionMods.asset");
            AssetDatabase.SaveAssets();
        }

        private static void AddPage(Tutorial tutorial, List<TutorialPage> pages, string title, string text, int step)
        {
            var page = ScriptableObject.CreateInstance<TutorialPage>();
            page.name = title;
            page.Title = title;
            page.NextButton = "Далее / Next";
            page.DoneButton = "Готово / Done";
            AssetDatabase.AddObjectToAsset(page, tutorial);
            var paragraph = ScriptableObject.CreateInstance<InstructionsParagraph>();
            paragraph.name = title + " Instructions";
            paragraph.Title = title;
            paragraph.Text = text;
            AssetDatabase.AddObjectToAsset(paragraph, tutorial);
            if (step >= 0)
            {
                var criterion = ScriptableObject.CreateInstance<ModTutorialCriterion>();
                criterion.name = "Step " + step;
                criterion.Step = step;
                AssetDatabase.AddObjectToAsset(criterion, tutorial);
                paragraph.Criterias().AddItem(new TypedCriterion(new SerializedType(typeof(ModTutorialCriterion)), criterion));
            }
            page.Paragraphs.Add(paragraph);
            pages.Add(page);
            EditorUtility.SetDirty(paragraph);
            EditorUtility.SetDirty(page);
        }
    }
}
