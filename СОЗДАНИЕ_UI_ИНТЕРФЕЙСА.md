# 🎨 Создание UI интерфейса для смены шапок

## Пошаговая инструкция

### ШАГ 1: Найдите Canvas в сцене
1. Откройте Unity
2. Откройте сцену `SampleScene.unity`
3. В Hierarchy найдите объект `Canvas` (обычно он есть в сцене)

### ШАГ 2: Создайте главную панель выбора шапок

1. **ПКМ на Canvas** → **UI** → **Panel**
2. Назовите панель: `HatSelectionPanel`
3. Настройте позицию (например, внизу экрана):
   - В Inspector найдите компонент **Rect Transform**
   - Установите **Anchor Presets**: Bottom-Center (удерживайте Shift+Alt)
   - Установите **Pos Y**: 100 (чтобы панель была немного выше низа экрана)
   - Установите **Width**: 600, **Height**: 200

### ШАГ 3: Создайте кнопки выбора типа шапки

Создайте 5 кнопок внутри `HatSelectionPanel`:

1. **ПКМ на HatSelectionPanel** → **UI** → **Button - TextMeshPro** (или обычный Button)
2. Назовите: `NoneButton`
3. В компоненте **Text (TMP)** или **Text** напишите: "Без шапки"
4. Настройте размер: **Width**: 100, **Height**: 40

Повторите для остальных кнопок:
- `KokoshnikButton` - текст: "Кокошник"
- `BorodaButton` - текст: "Борода"
- `HelmetButton` - текст: "Каска"
- `OrangeBoxButton` - текст: "Ящик"

**Совет:** Используйте **Horizontal Layout Group** на `HatSelectionPanel` для автоматического размещения кнопок:
1. Выделите `HatSelectionPanel`
2. **Add Component** → **Layout** → **Horizontal Layout Group**
3. Настройте:
   - **Spacing**: 10
   - **Child Alignment**: Middle Center
   - **Child Force Expand**: Width ✓, Height ✓

### ШАГ 4: Создайте панель выбора цвета каски

1. **ПКМ на HatSelectionPanel** → **UI** → **Panel**
2. Назовите: `HelmetColorPanel`
3. Настройте позицию (под кнопками):
   - **Anchor Presets**: Top-Center
   - **Pos Y**: -60
   - **Width**: 400, **Height**: 60
4. Добавьте **Horizontal Layout Group** (как в шаге 3)

### ШАГ 5: Создайте кнопки выбора цвета каски

Создайте 3 кнопки внутри `HelmetColorPanel`:

1. **ПКМ на HelmetColorPanel** → **UI** → **Button - TextMeshPro**
2. Назовите: `HelmetDefaultButton`
3. Текст: "Обычная"
4. Размер: **Width**: 100, **Height**: 40

Повторите для:
- `HelmetBlueButton` - текст: "Синяя"
- `HelmetWhiteButton` - текст: "Белая"

### ШАГ 6: Настройте HatUIManager

1. Найдите объект с компонентом `HatUIManager` в Hierarchy (или создайте новый GameObject)
2. Если компонента нет:
   - **Add Component** → `HatUIManager`
3. В Inspector заполните все поля:
   - **Hat Controller**: перетащите объект с компонентом `HatController`
   - **None Button**: перетащите `NoneButton`
   - **Kokoshnik Button**: перетащите `KokoshnikButton`
   - **Boroda Button**: перетащите `BorodaButton`
   - **Helmet Button**: перетащите `HelmetButton`
   - **Orange Box Button**: перетащите `OrangeBoxButton`
   - **Helmet Color Panel**: перетащите `HelmetColorPanel`
   - **Helmet Default Button**: перетащите `HelmetDefaultButton`
   - **Helmet Blue Button**: перетащите `HelmetBlueButton`
   - **Helmet White Button**: перетащите `HelmetWhiteButton`
   - **Hat Selection Panel**: перетащите `HatSelectionPanel`
   - **Toggle Hat Panel Button**: (опционально) можно оставить пустым

### ШАГ 7: Проверьте настройки

Убедитесь что:
- ✅ Все кнопки созданы и имеют текст
- ✅ Все поля в `HatUIManager` заполнены
- ✅ `HatController` назначен в `HatUIManager`
- ✅ Панель `HelmetColorPanel` скрыта по умолчанию (будет показываться при выборе каски)

## Альтернативный способ (быстрый)

Если у вас уже есть UI элементы в сцене, вы можете:
1. Использовать существующие кнопки
2. Просто перетащить их в соответствующие поля `HatUIManager`
3. Назначить правильные тексты на кнопках

## Готово! 🎉

Теперь при запуске сцены:
- Появится панель с кнопками выбора шапок
- При выборе "Каска" появится панель выбора цвета
- При выборе шапки она появится на всех персонажах
