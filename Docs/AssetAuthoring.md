# Создание и разметка assets

Эта инструкция относится к исходному проекту SDK. Готовую папку Builds/Packages устанавливают в игру; исходные `.asset`, `.cs` и `.meta` туда не копируют. Версию Unity берите из ProjectSettings/ProjectVersion.txt.

Ссылки ниже ведут на официальные руководства Unity: Addressables 2.11, Localization 1.5 и общие API Unity 6. Точные версии проекта по-прежнему задаются ProjectVersion.txt и Packages/manifest.json. Имена `Catalog.*`, игровые ID, формат `mod.json` и папка Mods — контракт Expedition SDK, а не встроенные соглашения Unity.

## Что означает каждая метка

| Значение | Где задаётся | Для чего нужно |
|---|---|---|
| modId | mod.json | Уникальное имя пакета, например `myteam.survey` |
| Игровой ID | Inspector конфигурации: Item Id, Recipe Id, Milestone Id или Id | Идентичность предмета/рецепта/вехи/погоды для игровых владельцев и сохранений |
| Address | Addressables Groups | Ключ загрузки конкретного asset из каталога |
| content.id | Запись content в mod.json | Уникальная запись контента пакета; это не автоматическая установка игрового ID в конфигурации |
| content.address | Та же запись manifest | Должен точно совпадать с Address в группе |
| [Label](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/Labels.html) | Addressables Groups | Определяет, какой игровой каталог должен зарегистрировать asset |
| [Group](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/Groups.html) | Addressables Groups | Определяет пакет при сборке; сама по себе не регистрирует asset в игре |

Пример: у Mineral.asset игровой Item Id — `myteam.survey/item/mineral`, Address и content.address — `myteam.survey/mineral.asset`, label — `Catalog.Item`, группа — `Mod_myteam.survey`. content.id можно сделать равным Address. Все ID и Address пакета начинайте с `<modId>/`; регистр символов важен. Не меняйте опубликованные игровые ID и адреса persistent prefab.

Не используйте GameObject Tag, Asset Label внизу Inspector или имя файла вместо **Addressables Label**. Labels `Item`, `Recipes`, `Weather` и произвольный `Mod.Example` не заменяют доменные `Catalog.*`.

## Создать asset в существующем моде

Механизм конфигураций описан в [ScriptableObject](https://docs.unity3d.com/6000.0/Documentation/Manual/class-ScriptableObject.html); пункты меню Create задаются атрибутом [CreateAssetMenu](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CreateAssetMenuAttribute.html). В SDK уже есть нужные типы, поэтому для создания их экземпляров новый C#-скрипт не требуется.

1. В Project откройте `Assets/Mods/<Name>/Content`. Для первого опыта используйте Production.
2. Создайте конфигурацию через контекстное меню **Create** из таблицы ниже. Либо продублируйте подходящий пример **внутри того же мода** через Duplicate: Unity выдаст новый GUID и сохранит авторские значения.
3. Задайте новый игровой ID и заполните поля конфигурации. Простое переименование файла не меняет Item Id/Recipe Id. У копии обязательно замените этот ID.
4. Откройте **Window > Asset Management > Addressables > Groups**. Перетащите asset в `Mod_<modId>`. Не делайте Addressable всю папку Content.
5. Назначьте Address и точные labels из таблицы. Для стандартного Configure Project используйте адрес `<modId>/<путь относительно Content>`, с расширением и `/`: `myteam.survey/mineral.asset`. Такой же формат обязателен для таблиц локализации, которые сборщик возвращает в группу мода.
6. Добавьте запись в массив `content` своего mod.json. У каждой явно Addressable-записи группы должна быть ровно одна запись manifest, и наоборот.
7. В **Expedition > Mod SDK > Mod Workspace** выберите мод, нажмите **Проверить**, затем **Собрать мод**. После сборки выполните **Expedition > Mod SDK > Verify Built Bundles**.
8. Установите пакет и проверьте регистрацию у игрового владельца, затем применение в игре. Validate проверяет структуру manifest/группы, но не доказывает правильность каждого label, заполнение всех доменных полей или игровой эффект.

`Create Test Mods` — инструмент создания поставляемых примеров. Он не регистрирует автоматически каждый новый пользовательский asset и не исправляет произвольную авторскую конфигурацию.

## Какие конфигурации создавать и как помечать

Пути в колонке Create указаны относительно контекстного меню Project **Create**. `Data` — значение content.kind для конфигураций ScriptableObject.

| Конфигурация | Create | Addressables labels | Текущая граница |
|---|---|---|---|
| ItemConfig: ресурс/расходник/предмет | Expedition > Items > Item Config | `Catalog.Item` | Регистрация трёх примеров подтверждена в Survival; поведение задают существующие Feature Configs |
| ItemRecipeConfig | Expedition > Recipes > Item Recipe | `Catalog.Recipe` | Регистрация примера подтверждена |
| Рецепт, известный с начала игры | То же меню рецепта | `Catalog.Recipe` и `Catalog.Recipe.Initial` | Добавляйте второй label только для начального открытия; обычная регистрация не открывает рецепт |
| MilestoneConfig | Game > Progression > Milestone | `Catalog.Milestone` | Загружается владельцем прогрессии; заполните условия либо используйте ручное завершение исследованием |
| MilestoneRecipeUnlockConfig | Game > Recipes > Milestone Unlock | `Catalog.Recipe.Unlock` | Связь Milestone → Recipes; сам рецепт также должен иметь `Catalog.Recipe` |
| RecipeFragmentSetConfig | Game > Recipes > Fragment Set | `Catalog.Recipe.FragmentSet` | Нужен полный набор связей фрагментов и рецептов; игровая проверка примера не выполнена |
| ResearchRecipeConfig | Expedition > Recipes > Research Recipe | `Catalog.Recipe.Research` | Загружается текущим исследовательским процессом; это отдельная семья, не ItemRecipe |
| ResearchSampleConfig | Expedition > Research > Sample | `Catalog.Research.Sample` | Укажите ручную Milestone и Source Item; предмет и веха размечаются отдельно |
| ResearchSurveyConfig | Expedition > Research > Survey | `Catalog.Research.Sample` | Производный исследовательский тип; конкретный игровой процесс требует проверки |
| WeatherDefinition | Game > Environment > Weather | `Catalog.Weather` | Регистрация эпизода подтверждена; визуальный эффект/сохранение проверяются отдельно |
| PelengCatalog | Expedition > Peleng > Catalog | `Catalog.Peleng` | Загрузка предусмотрена startup; новый пример не проверен |
| BuildRecipeConfig | Game > Building > Recipe | `Catalog.Recipe`, при необходимости `Catalog.Recipe.Initial` | Нужен совместимый строительный prefab; SDK-пример размещения пока не завершён |
| Persistent prefab | Создание prefab из поддерживаемых игровых компонентов | `Persistence.Prefab` | Стабильный Address используется сохранениями; SDK не содержит игровых компонентов для произвольного нового prefab |

Для последних строк наличие label означает существующий путь загрузки, а не подтверждение всех механик в Player. Не добавляйте произвольным prefab label Persistence.Prefab: необходим контракт PersistentObject и соответствующих компонентов базовой игры.

### Типы без автоматического подключения в текущей интеграции

| Тип | Как создать | Как использовать сейчас |
|---|---|---|
| CelestialBodyDefinition | Create > Environment > Celestial Body | Авторство и загрузка из bundle; автоматический выбор существующей системой суток ещё не подключён |
| FootstepAudioProfile | Create > Audio > Footstep Audio Profile | Авторство; замена профиля шагов требует поддержанного слота у игрового владельца |
| LayeredAudioClip | Create > Audio > Layered Audio Clip | Зависимость аудиопрофиля либо явно загружаемый Data asset |
| ADSRClipGenerator | Create > Audio > Generators > ADSR Clip Generator | Настройка генератора через Inspector; назначайте существующему потребителю |
| InteractActionConfig, InteractGatherSourceConfig, InteractGatherResultConfig, InteractPresentationCatalog | Create > Expedition > Interact > Action / Gather Source / Gather Result / Presentation Catalog | Ссылки конфигураций существующих взаимодействий; свободная запись manifest не создаёт объект в мире |
| AnalyzerZoneConfig | Create > Expedition > Research > Analyzer Zone | Конфигурация для существующей зоны; не создаёт зону автоматически |
| InputPromptCatalog | Create > Game > UI > Input Prompt Catalog | Авторство доступно; внешняя подмена UI-каталога не подключена |
| DialogueConfig | Дублировать ResearchAndEvents/Content/ResearchDialogue.asset внутри своего пакета | Отдельного CreateAssetMenu и полноценного редактора реплик в SDK пока нет; пример создан авторским инструментом, запуск диалога не автоматический |
| VolumeProfile | Создать/дублировать профиль Volume в своём Content | Назначить в WeatherDefinition.Profile; оставить обычной зависимостью |

Такие assets можно включить как `Data`/`Mod.Example` для явной загрузки и проверки транспорта. Не выдумывайте `Catalog.Dialogue`, `Catalog.Sky` и другие labels: без игрового потребителя они ничего не подключают. Авторство AI-мобов и весь первоначальный сценарий из 15 единиц пока не завершены.

## Заполнить предмет

Для первого нового предмета продублируйте Production/Content/Mineral.asset в той же папке, измените Item Id, Name и Description, затем разметьте копию как новый asset.

| Поле Inspector | Что задавать |
|---|---|
| Item Id | Новый стабильный ID, например `sample.production/item/copper` |
| Name / Description | Localized String: собственная String Table и ключ с переводами RU/EN |
| Category | Непустая поддерживаемая категория; в примерах `Resources` |
| Pickup | Совместимый persistent pickup. Для текущих примеров разрешена внешняя ссылка на базовый IronMineral, указанная в CreatingAMod.md |
| Sprite | Игровая иконка через [AssetReferenceSprite / AssetReference](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/AssetReferences.html); Для собственной PNG выберите Texture Type = Sprite (2D and UI), Sprite Mode = Single, Apply; добавьте её в группу/manifest как Texture |
| Shape | Размер и занятые клетки инвентаря; проверьте, что предмет помещается в целевой инвентарь |
| Component Configs | Данные уже существующих игровых компонентов и применения; не добавляйте null-элементы |
| Pickup Audio / Use Audio | Поддерживаемые аудиоданные; необязательные ссылки оставляйте в состоянии рабочего примера |

У Restorative уже сохранён полиморфный ValueChangeConfig с изменением Health. Чтобы получить другой расходник, дублируйте его внутри Production, измените ID, локализацию и значения существующего элемента. Обычный Inspector SDK пока не обеспечивает полноценный выбор всех новых [SerializeReference](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SerializeReference.html)-типов: увеличение Size с null-элементом не создаёт поведение. Экипировка и инструменты используют ItemConfig с соответствующими вложенными конфигурациями; создание готового нового инструмента в SDK ещё требует завершения авторских инструментов.

Ссылки на базовый pickup и sprite могут выглядеть пустыми в чистом SDK, потому что их assets находятся только в игре. Это явно разрешённые внешние GUID из инструкции, а не повод копировать игровые prefab/платные картинки в SDK. Произвольные GUID из приватного проекта не являются поддерживаемым API.

## Заполнить рецепт

Создайте **Expedition > Recipes > Item Recipe** в том же моде, где находятся результат и ингредиенты.

- Recipe Id: уникальный стабильный ID.
- Result: ваш ItemConfig; Result Count больше нуля.
- Ingredients: минимум один элемент с Item и положительным Count.
- Crafting Station Ids: хотя бы один поддерживаемый ID, без дублей. В примере используется `Crafting/FieldPrinter`.
- Production Duration: больше нуля.

Назначьте `Catalog.Recipe`. Для немедленной доступности добавьте `Catalog.Recipe.Initial`. Для открытия по вехе создайте **Game > Recipes > Milestone Unlock**, задайте Milestone и список Recipes, назначьте ему `Catalog.Recipe.Unlock`. Не ставьте Initial рецепту, который должен открываться только исследованием.

## Исследование и погода

Для исследовательского образца создайте ItemConfig и MilestoneConfig. У ручной вехи оставьте автоматические условия Crafted Or Picked Up Items, Discovered Recipes, Built Recipes и Progression Flags пустыми. Затем создайте Research Sample, укажите Milestone, Source Item, положительную Duration и локализованное Description. Для ограничения доступа можно задать Prerequisite. Пометьте все три конфигурации их labels из таблицы; они не регистрируются за счёт одной ссылки друг на друга.

У WeatherDefinition задайте уникальный Id, неотрицательные Weight и Cooldown, положительные Entry Duration / Exit Duration и диапазон Hold Duration с минимумом больше нуля и максимумом не меньше минимума. Назначьте собственный VolumeProfile. Поле Color меняет цвет эпизода в редакторе таймлайна, а не цвет атмосферы. Для простого эпизода без игровых VFX оставьте Effects и Entry/Hold/Exit Timeline пустыми. Timeline требует совместимого Effects prefab; такой prefab нельзя заменить случайным GameObject.

## Локализация и зависимости

Официальные руководства: [String Tables — коллекции, ключи и таблицы локалей](https://docs.unity3d.com/Packages/com.unity.localization@1.5/manual/StringTables.html), [LocalizedString — ссылка на таблицу и запись](https://docs.unity3d.com/Packages/com.unity.localization@1.5/api/UnityEngine.Localization.LocalizedString.html).

1. Откройте **Window > Asset Management > Localization Tables**. Создайте String Table Collection с уникальным именем для своего мода; используйте имеющиеся локали ru и en.
2. Сохраните коллекцию и таблицы в `Content/Localization` своего пакета. Добавьте ключи и оба перевода. В Localized String выбирайте таблицу и ключ, не вставляйте отображаемый текст вместо ссылки.
3. В группу мода включите RU-таблицу, EN-таблицу и Shared Table Data. Каждую внесите в content manifest как Data с собственным Address. Сам редакторский StringTableCollection.asset в bundle не включайте.
4. Сохраните автоматически назначенные Localization labels. Не переименовывайте их в Catalog.Item и не удаляйте при переносе таблиц в группу мода. Подготовка перед сборкой возвращает объявленные таблицы из автоматически созданных Unity групп к владельцу-моду.
5. Проверьте отображение через текущий игровой UI на обоих языках. Загрузка StringTable из bundle сама по себе не доказывает работу всех LocalizedString.

Обычная прямая ссылка на материал, AudioClip или VolumeProfile внутри того же мода включит dependency в bundle; правила включения и дублирования описаны в [Asset dependencies](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/AssetDependencies.html). Такую зависимость не обязательно объявлять отдельной Addressable-записью. AssetReference, напротив, должен разрешаться через каталог: свой целевой asset добавьте явно, для базового используйте только разрешённую ссылку. Не включайте VolumeProfile примера отдельным Addressable: его sub-assets могут дать несколько Object locations на одном ключе и нарушить проверку «один address — один asset».

Не перетаскивайте ItemConfig из другого мод-пакета в Result/Ingredients. Прямые межпакетные Unity-ссылки запрещены валидатором. Запись dependencies задаёт порядок и минимальную версию пакета, но не превращает Object-поле в ссылку по стабильному ID. Если соответствующая семья не поддерживает внешние ID, держите связанные assets в одном моде.

## Создать собственный пакет

Unity описывает сборку контента в отдельном проекте в [Load content from multiple projects](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/MultiProject.html). Приведённый ниже manifest и команда Configure Project относятся к Expedition SDK.

В Project создайте `Assets/Mods/Survey/Content`. Для минимального первого пакета положите туда собственный `readme.txt`, а рядом с Content создайте текстовый `mod.json`:

```json
{
  "schemaVersion": "2",
  "modId": "myteam.survey",
  "version": "0.1.0",
  "displayName": "Survey",
  "description": "My content pack",
  "minimumGameVersion": "0.1.0",
  "catalog": "",
  "dependencies": [],
  "content": [
    {
      "id": "myteam.survey/readme.txt",
      "address": "myteam.survey/readme.txt",
      "kind": "Data",
      "mode": "Additive",
      "targetId": ""
    }
  ]
}
```

Выполните **Expedition > Mod SDK > Configure Project**: для существующего manifest появится группа `Mod_myteam.survey` и запись файла с нужным Address. Команда не создаёт пользовательские assets и не назначает автоматически все доменные labels: назначьте их сами по таблице. Для TextAsset доменный label не требуется; это пример явной загрузки. Затем выберите новый мод в Mod Workspace и соберите его.

При расширении пакета добавляйте записи content и настраивайте группу вместе. Допустимые kind: Data, Prefab, Texture, Material, Audio. Для Additive targetId пуст. Replacement требует объявленного игрового слота и реализованного владельца замены; в текущих примерах таких слотов нет.

## Частые ошибки

| Симптом | Что проверить |
|---|---|
| Asset есть в bundle, но предмет/погода не появились у владельца | Точный Catalog.* label, тип asset и игровую регистрацию; само наличие каталога недостаточно |
| Group contains N entries, manifest declares M | Добавьте недостающую запись manifest либо уберите лишнюю явную Addressable-запись; обычные dependencies не считаются entries |
| Address declared but not in group | Сравните content.address и Address, включая регистр/расширение; проверьте группу выбранного modId |
| Duplicate ItemConfig/recipe ID | Измените ID у новой копии, не только имя файла |
| Pickup prefab required / missing sprite | Нужна совместимая базовая ссылка либо поддерживаемый собственный prefab/Addressable sprite |
| No translation found | Таблица, ключ, RU/EN-записи, Shared Data и Localization labels должны быть в каталоге |
| Another AssetBundle with the same files is already loaded | Пересоберите все пакеты актуальным ModBuildCommand с отдельными MonoScripts/built-in именами по modId |
| Изменение установленного мода не видно | Остановите игру/Play Mode, замените целиком папку пакета и запустите снова |

Сборка, установка, совместимость сохранений и границы текущей игровой проверки описаны в [CreatingAMod.md](CreatingAMod.md).
