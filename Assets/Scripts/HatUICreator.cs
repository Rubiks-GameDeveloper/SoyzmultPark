#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Вспомогательный скрипт для автоматического создания UI интерфейса смены шапок
/// Использование: В меню Unity выберите Tools → Create Hat Selection UI
/// </summary>
public class HatUICreator : EditorWindow
{
    [MenuItem("Tools/Create Hat Selection UI")]
    public static void CreateHatUI()
    {
        // Находим Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            // Создаем Canvas если его нет
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Создаем EventSystem если его нет
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        
        // Создаем главную панель
        GameObject hatPanel = CreatePanel(canvas.transform, "HatSelectionPanel");
        RectTransform hatPanelRect = hatPanel.GetComponent<RectTransform>();
        hatPanelRect.anchorMin = new Vector2(0.5f, 0f);
        hatPanelRect.anchorMax = new Vector2(0.5f, 0f);
        hatPanelRect.pivot = new Vector2(0.5f, 0f);
        hatPanelRect.anchoredPosition = new Vector2(0, 100);
        hatPanelRect.sizeDelta = new Vector2(600, 200);
        
        // Добавляем Horizontal Layout Group
        HorizontalLayoutGroup hatLayout = hatPanel.AddComponent<HorizontalLayoutGroup>();
        hatLayout.spacing = 10;
        hatLayout.childAlignment = TextAnchor.MiddleCenter;
        hatLayout.childControlWidth = true;
        hatLayout.childControlHeight = true;
        hatLayout.childForceExpandWidth = true;
        hatLayout.childForceExpandHeight = true;
        
        // Создаем кнопки выбора типа шапки
        Button noneButton = CreateButton(hatPanel.transform, "NoneButton", "Без шапки");
        Button kokoshnikButton = CreateButton(hatPanel.transform, "KokoshnikButton", "Кокошник");
        Button borodaButton = CreateButton(hatPanel.transform, "BorodaButton", "Борода");
        Button helmetButton = CreateButton(hatPanel.transform, "HelmetButton", "Каска");
        Button orangeBoxButton = CreateButton(hatPanel.transform, "OrangeBoxButton", "Ящик");
        
        // Создаем панель выбора цвета каски
        GameObject colorPanel = CreatePanel(hatPanel.transform, "HelmetColorPanel");
        RectTransform colorPanelRect = colorPanel.GetComponent<RectTransform>();
        colorPanelRect.anchorMin = new Vector2(0.5f, 1f);
        colorPanelRect.anchorMax = new Vector2(0.5f, 1f);
        colorPanelRect.pivot = new Vector2(0.5f, 1f);
        colorPanelRect.anchoredPosition = new Vector2(0, -60);
        colorPanelRect.sizeDelta = new Vector2(400, 60);
        colorPanel.SetActive(false); // Скрываем по умолчанию
        
        // Добавляем Horizontal Layout Group для панели цветов
        HorizontalLayoutGroup colorLayout = colorPanel.AddComponent<HorizontalLayoutGroup>();
        colorLayout.spacing = 10;
        colorLayout.childAlignment = TextAnchor.MiddleCenter;
        colorLayout.childControlWidth = true;
        colorLayout.childControlHeight = true;
        colorLayout.childForceExpandWidth = true;
        colorLayout.childForceExpandHeight = true;
        
        // Создаем кнопки выбора цвета каски
        Button defaultButton = CreateButton(colorPanel.transform, "HelmetDefaultButton", "Обычная");
        Button blueButton = CreateButton(colorPanel.transform, "HelmetBlueButton", "Синяя");
        Button whiteButton = CreateButton(colorPanel.transform, "HelmetWhiteButton", "Белая");
        
        // Находим или создаем HatUIManager
        HatUIManager uiManager = FindObjectOfType<HatUIManager>();
        if (uiManager == null)
        {
            GameObject managerObj = new GameObject("HatUIManager");
            uiManager = managerObj.AddComponent<HatUIManager>();
        }
        
        // Настраиваем HatUIManager через SerializedObject
        SerializedObject serializedManager = new SerializedObject(uiManager);
        serializedManager.FindProperty("hatController").objectReferenceValue = FindObjectOfType<HatController>();
        serializedManager.FindProperty("noneButton").objectReferenceValue = noneButton;
        serializedManager.FindProperty("kokoshnikButton").objectReferenceValue = kokoshnikButton;
        serializedManager.FindProperty("borodaButton").objectReferenceValue = borodaButton;
        serializedManager.FindProperty("helmetButton").objectReferenceValue = helmetButton;
        serializedManager.FindProperty("orangeBoxButton").objectReferenceValue = orangeBoxButton;
        serializedManager.FindProperty("helmetColorPanel").objectReferenceValue = colorPanel;
        serializedManager.FindProperty("helmetDefaultButton").objectReferenceValue = defaultButton;
        serializedManager.FindProperty("helmetBlueButton").objectReferenceValue = blueButton;
        serializedManager.FindProperty("helmetWhiteButton").objectReferenceValue = whiteButton;
        serializedManager.FindProperty("hatSelectionPanel").objectReferenceValue = hatPanel;
        serializedManager.ApplyModifiedProperties();
        
        // Выделяем созданные объекты
        Selection.activeGameObject = hatPanel;
        EditorGUIUtility.PingObject(hatPanel);
        
        Debug.Log("UI интерфейс для смены шапок успешно создан! Проверьте настройки HatUIManager.");
    }
    
    private static GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        return panel;
    }
    
    private static Button CreateButton(Transform parent, string name, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(100, 40);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.6f, 0.9f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Создаем текст
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        // Используем обычный Text (можно заменить на TextMeshPro вручную)
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 18;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.white;
        
        return button;
    }
}
#endif
