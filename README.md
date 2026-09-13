# Expedition Mod SDK

Пошагово: [создание assets, разметка и ссылки на официальную документацию Unity](Docs/AssetAuthoring.md).

Открытый Unity SDK для content-only модов Expedition. Общий API — исходники `.cs` и `Expedition.ModApi.asmdef`; игровых DLL и Odin здесь нет.

Открывайте проект версией Editor из `ProjectSettings/ProjectVersion.txt`. Игра и SDK используют одинаковые версии URP и Addressables, закреплённые в `Packages/manifest.json`; фактически разрешённые версии проверяются перед сборкой. Pipeline закреплён на `0.5.0-exp.1`, совместимой с этим Editor.

## Быстрый старт

1. Откройте **Expedition > Mod SDK > Mod Workspace**.
2. Выберите один из четырёх модов в `Assets/Mods`.
3. Нажмите **Проверить**, затем **Собрать мод** или **Собрать все моды**.
4. Готовые пакеты находятся в `Builds/Packages/<mod-id>-<version>`.
5. Закройте игру, выберите её `Application.persistentDataPath/Mods` и нажмите **Установить пакет**. Перезапустите игру.

Обучение на Unity Tutorial API: **Tutorials > Show Tutorials Window > Expedition Mod SDK**. Выберите **Русский** или **English**. Каждая версия содержит шесть страниц: выбор, создание и разметка ассетов, проверка, сборка, установка и проверка в игре. Ключевые шаги проверяют реальное выполнение действий в Mod Workspace.

| Папка | Пакет | Содержимое |
|---|---|---|
| `Assets/Mods/DataAndPresentation` | `sample.author` | TextAsset, собственная иконка, синтезированный звук и профиль шагов |
| `Assets/Mods/Production` | `sample.production` | ресурс, расходник с существующим ValueChangeConfig, рецепт, RU/EN |
| `Assets/Mods/World` | `sample.world` | погодная конфигурация с VolumeProfile, небесное тело с собственной текстурой |
| `Assets/Mods/ResearchAndEvents` | `sample.events` | образец, веха исследования, диалог с RU/EN субтитрами и синтетическим звуком для проверки времени |

Это тесты авторства и реальных bundles. Сборка и загрузка всех 22 объявленных assets проверены. Полный сценарий из 15 игровых действий, размещаемые/подбираемые prefab, замены стабильных слотов и AI-моб ещё не завершены; эти пакеты не являются доказательством их runtime-поддержки. Подробности: [создание и установка](Docs/CreatingAMod.md), [архитектура](Docs/Architecture.md).

**Expedition > Mod SDK > Verify Built Bundles** загружает объявленный контент из готовых внешних каталогов и пишет результат в `Builds/BundleVerification.txt`.

## Шаблон проекта

Готовый архив шаблона: `Builds/Templates/expedition-mod-sdk-1.0.0.tgz`. Он содержит исходники SDK, четыре примера модов и обучение на русском и английском языках.

Это переносимый Unity project template, не `.unitypackage` и не сборка игры. В `.tgz` есть `package/package.json` с точной версией Editor и `package/ProjectData~` с исходным проектом. Для ручного развёртывания извлеките ProjectData~ в новую папку и откройте её через Hub подходящей версией Unity. Сам ProjectVersion.txt штатный упаковщик удаляет; его значение сохранено в package.json. Не выбирайте произвольную другую версию Editor.

В созданном проекте инструкция также лежит в `Assets/Expedition Mod SDK/Documentation`. В исходном Git-репозитории канонические документы — README и Docs. Исходный snapshot упаковки остаётся в Builds, исключённом из Git.

## Публичная публикация

Разрешены только исходники и контент репозитория и пакеты Unity Registry. Запрещены платные/приватные ассеты игры, Odin и скопированные DLL. Изображения и звук примеров созданы специально для SDK; игровые assets не копировались.

Перед публикацией: `Tools/audit_repository.ps1`, реальная сборка всех модов и **Verify Built Bundles**. `Builds/` исключён из Git; `.tgz` шаблона и готовые пакеты публикуются как release artifacts. Код и собственный примерный контент — MIT; лицензии Unity packages сохраняются: [THIRD_PARTY_NOTICES](THIRD_PARTY_NOTICES.md).

Проверка в Survival: четыре каталога подключены, три предмета, рецепт и погодный эпизод зарегистрированы игровыми владельцами. Разрешённые внешние ссылки примеров описаны в [инструкции](Docs/CreatingAMod.md).
