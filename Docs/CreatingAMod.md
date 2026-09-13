# Создание, сборка и установка мода

Пошагово: [создание assets, поля Inspector, Addressables labels и manifest](AssetAuthoring.md).

## Исходники

Каждый мод находится в своей папке `Assets/Mods/<Name>`. В корне лежит `mod.json`, в `Content` — его собственные assets. Общие авторские типы находятся в `Assets/Expedition Mod SDK/Runtime`, под `Expedition.ModApi.asmdef`; моды не содержат исполняемый код.

`mod.json` использует schemaVersion `2`, уникальный `modId`, версию `major.minor.patch`, minimumGameVersion, dependencies и content. Исходный `catalog` пуст; сборщик записывает итоговые версии API/Editor/Addressables/URP, target и путь каталога. `id` и `address` каждой записи начинаются с `<modId>/`. После публикации не меняйте стабильные ID контента, который участвует в сохранениях.

Поддерживаемые kind: Data, Prefab, Texture, Material, Audio. Additive имеет пустой targetId. Replacement требует объявленного игрового слота; сама запись manifest не реализует замену в игре. Тестовые пакеты используют Additive.

Runtime labels сохраняются в каталоге. Доменные владельцы игры собирают membership по labels всех загруженных locators; имя группы не определяет runtime membership. Доменные labels: Catalog.Item, Catalog.Recipe, Catalog.Weather, Catalog.Milestone, Catalog.Research.Sample. Profile небесного тела и диалог пока демонстрируют авторство/десериализацию, а не автоматическое подключение их в игровой сценарий.

## Авторство

Редактируйте assets обычным Inspector. У расходника уже сохранён ValueChangeConfig, меняющий Health. Рецепт использует подтверждённый базовый ID станции `Crafting/FieldPrinter`. Профили Volume — зависимости погоды, а не отдельные записи Addressables. В мод-пакете не должно быть ссылок на приватные игровые assets или прямых Unity-ссылок в соседний мод: такой asset иначе незаметно попадёт в bundle. Межпакетные связи требуют поддерживаемого доменом стабильного ID и записи dependencies.

Добавьте публичный asset в группу `Mod_<modId>` и назначьте адрес из manifest. Сохраните доменные и локализационные labels. Пример sample.author сохраняет исходный проверочный адрес `sample.author/additive/sample-text`.

Для новой семьи конфигураций одного доступного типа в Inspector недостаточно: отдельно нужны регистрация у игрового владельца, локализация, сохранение, жизненный цикл handle и игровая проверка. В SDK ещё не завершён весь набор инструментов для создания новых полиморфных элементов; имеющиеся элементы можно редактировать.

## Сборка

**Expedition > Mod SDK > Mod Workspace** → выберите мод → **Проверить** → **Собрать мод**. **Собрать все моды** выпускает четыре отдельных пакета. Build owner один: ModBuildCommand. Он включает только группу выбранного мода, временно делает её основной для служебных bundles, использует `expedition-mod://<modId>/...` и восстанавливает настройки групп после сборки.

Механизм внешних каталогов: [Unity — Load content catalogs](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/LoadContentCatalogAsync.html). Сборку контента отдельно от основного проекта описывает [Unity — Load content from multiple projects](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/MultiProject.html); установку в Mods и совместимость manifest проверяет сам Expedition SDK.

Выход: `Builds/Packages/<modId>-<version>/mod.json` и вложенная папка платформы с catalog, hash и bundles. Перед передачей вызовите **Verify Built Bundles**.

## Установка

1. Закройте игру. Моды фиксируются при запуске; горячего переключения нет.
2. Найдите `Application.persistentDataPath/Mods` игры. Для текущего Survival_2024 на Windows обычно `%USERPROFILE%/AppData/LocalLow/DefaultCompany/Survival_2024/Mods`; это зависит от Company/Product игры.
3. Выберите эту папку в Mod Workspace и нажмите **Установить пакет**. Либо вручную скопируйте целиком папку `<modId>-<version>` из Builds/Packages.
4. Установите зависимости до зависящего мода. Не оставляйте одновременно две версии одного modId. Для обновления закройте игру, удалите только папку старой версии и установите новую.
5. Перезапустите игру. Проверьте загрузку каталога и реальный игровой эффект. Успешная проверка копирования не означает прохождение сценария в игре.

Установщик не перезаписывает другую установленную версию. Идентичная повторная установка ничего не меняет. Он проверяет версии, зависимости, конфликты, catalog/hash/bundles и сверяет содержимое после копирования. Совместимость проверяется с текущим контрактом SDK и версией игры 0.1.0; это не автоматическое определение версии произвольного exe.

## Статус проверки

- Все 4 пакета собираются настоящим Addressables BuildPlayerContent для Windows x64.
- Загрузка каждого из 22 объявленных assets проверена по locations внешнего каталога из готовых bundles. Ожидаемый TextAsset прочитан.
- Установка всех пакетов и повторная идентичная установка проверены в отдельной временной папке.
- Tutorial: отдельные версии Русский / English по 6 страниц, выбор языка в едином container, одинаковые критерии действий, корректные sub-assets, сохранение текущей сцены.
- Игровая проверка всех 15 единиц первоначального сценария в Editor/standalone, pickup/build prefab, стабильные замены и AI остаются отдельной незавершённой интеграцией.

## Проверенная интеграция с Survival

Все четыре каталога подключены штатным startup из persistentDataPath/Mods. В игровых владельцах подтверждены три sample-предмета, рецепт sample.production/recipe/restorative и погода sample.world/weather/amber. Это проверка регистрации, не завершение полного игрового сценария.

Три тестовых предмета используют разрешённые ссылки на представление базового IronMineral: pickup GUID `50657e48d704d3a4eafff44f314eec59`, sprite GUID `2bd969b03cd9b244e8769b0ece3ed32a`, subobject `IronMineral`. Эти assets не входят в SDK или bundles модов; они разрешаются каталогом совместимой базовой игры. Предметы сохраняют собственные стабильные ID. В чистом SDK внешние ссылки не имеют локального объекта.

Вспомогательные MonoScripts/built-in bundles получают отдельный префикс modId, чтобы пакеты могли загружаться одновременно.
