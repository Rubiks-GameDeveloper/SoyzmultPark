# Инструкция по настройке системы управления шапками

## Обзор
Система позволяет добавлять различные шапки (кокошник, борода, каска, ящик) на персонажей и менять их через UI интерфейс. Каски поддерживают разные цвета (по умолчанию, синий, белый).

## Компоненты системы

### 1. HatController.cs
Основной скрипт для управления шапками на персонажах.

**Настройка в Inspector:**
- **Gena Hats**:
  - `Character Object` - ссылка на GameObject персонажа Гены
  - Остальные поля заполнятся автоматически при инициализации
  
- **Shopoklyak Hats**:
  - `Character Object` - ссылка на GameObject персонажа Шапокляк
  - Остальные поля заполнятся автоматически при инициализации

- **Prefabs**:
  - `Kokoshnik Prefab` - префаб кокошника (из Assets/Models/kokoshnik/)
  - `Boroda Prefab` - префаб бороды (из Assets/Models/boroda/)
  - `Helmet Prefab` - префаб каски (из Assets/Models/helmet/hamlet.fbx)
  - `Orange Box Prefab` - префаб ящика (из Assets/Models/orange_box/)

- **Helmet Materials** (опционально):
  - `Helmet Default Material` - материал для каски по умолчанию
  - `Helmet Blue Material` - материал для синей каски
  - `Helmet White Material` - материал для белой каски

### 2. HatUIManager.cs
Скрипт для управления UI кнопками смены шапок.

**Настройка в Inspector:**
- `Hat Controller` - ссылка на компонент HatController
- `None Button` - кнопка для снятия шапки
- `Kokoshnik Button` - кнопка выбора кокошника
- `Boroda Button` - кнопка выбора бороды
- `Helmet Button` - кнопка выбора каски
- `Orange Box Button` - кнопка выбора ящика
- `Helmet Color Panel` - панель с кнопками выбора цвета каски
- `Helmet Default Button` - кнопка выбора каски по умолчанию
- `Helmet Blue Button` - кнопка выбора синей каски
- `Helmet White Button` - кнопка выбора белой каски
- `Hat Selection Panel` - главная панель выбора шапок
- `Toggle Hat Panel Button` - кнопка для показа/скрытия панели

### 3. ARController.cs
Обновлен для интеграции с HatController.

**Дополнительная настройка:**
- `Hat Controller` - ссылка на компонент HatController

## Пошаговая настройка в Unity

### Шаг 1: Подготовка префабов
1. Откройте сцену `SampleScene.unity`
2. Найдите объекты `genaObject` и `shopoklyakObject`
3. Создайте префабы из моделей:
   - Перетащите `Assets/Models/kokoshnik/kokoshnik.fbx` в сцену
   - Перетащите `Assets/Models/boroda/boroda.fbx` в сцену
   - Перетащите `Assets/Models/helmet/hamlet.fbx` в сцену
   - Перетащите `Assets/Models/orange_box/SecretofOrange.fbx` в сцену
4. Создайте префабы из этих объектов (перетащите в папку Assets/Prefabs/)

### Шаг 2: Настройка HatController
1. Найдите объект с компонентом `ARController` в сцене
2. Добавьте компонент `HatController`
3. Настройте поля:
   - В `Gena Hats` → `Character Object` перетащите `genaObject`
   - В `Shopoklyak Hats` → `Character Object` перетащите `shopoklyakObject`
   - В `Prefabs` перетащите созданные префабы

### Шаг 3: Создание материалов для касок (опционально)
1. Создайте 3 материала в папке `Assets/Models/helmet/`:
   - `HelmetDefaultMaterial.mat`
   - `HelmetBlueMaterial.mat`
   - `HelmetWhiteMaterial.mat`
2. Настройте материалы:
   - Используйте шейдер из существующих материалов (например, из KokoshnikMaterial)
   - Для `HelmetDefaultMaterial` назначьте текстуру `lambert1_Base.png`
   - Для `HelmetBlueMaterial` назначьте текстуру `lambert1_blue_Base.png`
   - Для `HelmetWhiteMaterial` назначьте текстуру `lambert1_white_Base.png`
3. Перетащите материалы в соответствующие поля `HatController`

### Шаг 4: Создание UI панели
1. В Canvas создайте новую панель `HatSelectionPanel`
2. Добавьте кнопки:
   - `NoneButton` - "Без шапки"
   - `KokoshnikButton` - "Кокошник"
   - `BorodaButton` - "Борода"
   - `HelmetButton` - "Каска"
   - `OrangeBoxButton` - "Ящик"
3. Создайте дочернюю панель `HelmetColorPanel` с кнопками:
   - `HelmetDefaultButton` - "По умолчанию"
   - `HelmetBlueButton` - "Синий"
   - `HelmetWhiteButton` - "Белый"
4. Добавьте компонент `HatUIManager` на `HatSelectionPanel`
5. Настройте все ссылки в Inspector

### Шаг 5: Интеграция с ARController
1. В компоненте `ARController` найдите поле `Hat Controller`
2. Перетащите объект с компонентом `HatController`

## Использование

После настройки:
1. При запуске приложения появится панель выбора шапок
2. Пользователь может выбрать тип шапки через кнопки
3. При выборе каски появится панель выбора цвета
4. Шапки будут появляться на персонажах с эффектом dissolve при обнаружении маркера

## Примечания

- Материалы для касок можно создать автоматически (скрипт попытается загрузить текстуры из Resources)
- Рендереры шапок автоматически добавляются в список `rendersToDissolve` для анимации появления
- Система поддерживает расширение - можно легко добавить новые типы шапок
