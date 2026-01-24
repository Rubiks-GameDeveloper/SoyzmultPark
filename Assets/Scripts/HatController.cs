using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum HatType
{
    None,
    Kokoshnik,
    Boroda,
    Helmet,
    OrangeBox
}

public enum HelmetColor
{
    Default,
    Blue,
    White
}

public class HatController : MonoBehaviour
{
    [System.Serializable]
    public class CharacterHats
    {
        public GameObject characterObject;
        public GameObject kokoshnikInstance;
        public GameObject borodaInstance;
        public GameObject helmetInstance;
        public GameObject orangeBoxInstance;
        
        // Рендерер каски (один, но с разными материалами)
        public Renderer helmetRenderer;
    }
    
    [Header("Character Settings")]
    [Tooltip("Автоматически найти персонажей в сцене (если включено, массив Characters будет заполнен автоматически)")]
    [SerializeField] private bool autoFindCharacters = true;
    
    [Tooltip("Список персонажей. Если Auto Find Characters включено, можно оставить пустым")]
    [SerializeField] private CharacterHats[] characters = new CharacterHats[3];
    
    [Header("Model References (drag FBX files or prefabs here)")]
    [SerializeField] private GameObject kokoshnikPrefab;
    [SerializeField] private GameObject borodaPrefab;
    [SerializeField] private GameObject helmetPrefab;
    [SerializeField] private GameObject orangeBoxPrefab;
    
    [Header("Or use direct model paths (alternative to prefabs)")]
    [SerializeField] private string kokoshnikModelPath = "Models/kokoshnik/kokoshnik";
    [SerializeField] private string borodaModelPath = "Models/boroda/boroda";
    [SerializeField] private string helmetModelPath = "Models/helmet/hamlet";
    [SerializeField] private string orangeBoxModelPath = "Models/orange_box/SecretofOrange";
    
    [Header("Helmet Materials")]
    [SerializeField] private Material helmetDefaultMaterial;
    [SerializeField] private Material helmetBlueMaterial;
    [SerializeField] private Material helmetWhiteMaterial;
    
    [Header("Helmet Textures (optional - will be loaded from Resources)")]
    [SerializeField] private Texture2D helmetDefaultTexture;
    [SerializeField] private Texture2D helmetBlueTexture;
    [SerializeField] private Texture2D helmetWhiteTexture;
    
    private HatType currentHatType = HatType.None;
    private HelmetColor currentHelmetColor = HelmetColor.Default;
    
    private Dictionary<GameObject, List<Renderer>> characterRenderers = new Dictionary<GameObject, List<Renderer>>();
    
    // Событие для уведомления об изменении шапок (для ARController)
    public System.Action<HatType> OnHatTypeChanged;
    
    private void Start()
    {
        // Автоматически находим персонажей, если нужно
        if (autoFindCharacters)
        {
            AutoFindCharacters();
        }
        
        // Автоматически находим модели, если они не назначены
        AutoFindReferences();
        InitializeHats();
    }
    
    private void AutoFindCharacters()
    {
        // Ищем все объекты с рендерерами, которые могут быть персонажами
        // Ищем по именам или по наличию определенных компонентов
        List<GameObject> foundCharacters = new List<GameObject>();
        
        // Метод 1: Ищем по именам (если персонажи имеют характерные имена)
        string[] characterNames = { "gena", "shopoklyak", "cheburashka", "character", "персонаж" };
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            string objName = obj.name.ToLower();
            foreach (string charName in characterNames)
            {
                if (objName.Contains(charName) && !foundCharacters.Contains(obj))
                {
                    // Проверяем, что это действительно персонаж (есть рендерер или меш)
                    if (obj.GetComponentInChildren<Renderer>() != null || obj.GetComponentInChildren<MeshFilter>() != null)
                    {
                        foundCharacters.Add(obj);
                        break;
                    }
                }
            }
        }
        
        // Метод 2: Если не нашли по именам, ищем объекты с префабами моделей
        if (foundCharacters.Count < 3)
        {
            // Ищем объекты, которые являются экземплярами префабов персонажей
            foreach (GameObject obj in allObjects)
            {
                if (foundCharacters.Count >= 3) break;
                
                // Проверяем, что объект имеет дочерние объекты с рендерерами
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0 && !foundCharacters.Contains(obj))
                {
                    // Проверяем, что это не UI элемент
                    if (obj.GetComponent<Canvas>() == null && obj.GetComponent<RectTransform>() == null)
                    {
                        foundCharacters.Add(obj);
                    }
                }
            }
        }
        
        // Заполняем массив characters
        if (foundCharacters.Count > 0)
        {
            // Создаем массив нужного размера
            System.Array.Resize(ref characters, Mathf.Max(3, foundCharacters.Count));
            
            for (int i = 0; i < foundCharacters.Count && i < characters.Length; i++)
            {
                if (characters[i] == null)
                {
                    characters[i] = new CharacterHats();
                }
                characters[i].characterObject = foundCharacters[i];
            }
            
            Debug.Log($"Найдено персонажей: {foundCharacters.Count}");
        }
        else
        {
            Debug.LogWarning("Не удалось автоматически найти персонажей. Назначьте их вручную в Inspector.");
        }
    }
    
    private void AutoFindReferences()
    {
        // Автоматически находим модели, если они не назначены
        if (kokoshnikPrefab == null)
        {
            kokoshnikPrefab = FindModelInScene("kokoshnik") ?? LoadModelFromAssets("Models/kokoshnik/kokoshnik");
        }
        if (borodaPrefab == null)
        {
            borodaPrefab = FindModelInScene("boroda") ?? LoadModelFromAssets("Models/boroda/boroda");
        }
        if (helmetPrefab == null)
        {
            helmetPrefab = FindModelInScene("hamlet") ?? FindModelInScene("helmet") ?? LoadModelFromAssets("Models/helmet/hamlet");
        }
        if (orangeBoxPrefab == null)
        {
            orangeBoxPrefab = FindModelInScene("SecretofOrange") ?? FindModelInScene("orange") ?? LoadModelFromAssets("Models/orange_box/SecretofOrange");
        }
        
        // Автоматически находим текстуры касок
        if (helmetDefaultTexture == null)
        {
            helmetDefaultTexture = LoadTexture("Models/helmet/lambert1_Base");
        }
        if (helmetBlueTexture == null)
        {
            helmetBlueTexture = LoadTexture("Models/helmet/lambert1_blue_Base");
        }
        if (helmetWhiteTexture == null)
        {
            helmetWhiteTexture = LoadTexture("Models/helmet/lambert1_white_Base");
        }
    }
    
    private GameObject FindModelInScene(string nameContains)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains(nameContains.ToLower()))
            {
                return obj;
            }
        }
        return null;
    }
    
    private GameObject LoadModelFromAssets(string path)
    {
        // Пытаемся загрузить через Resources
        GameObject loaded = Resources.Load<GameObject>(path);
        if (loaded != null) return loaded;
        
        // Пытаемся найти через AssetDatabase (только в Editor)
        #if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:GameObject " + System.IO.Path.GetFileName(path));
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (assetPath.ToLower().Contains(path.Replace("Models/", "").ToLower()))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            }
        }
        #endif
        
        return null;
    }
    
    private Texture2D LoadTexture(string path)
    {
        // Пытаемся загрузить через Resources
        Texture2D loaded = Resources.Load<Texture2D>(path);
        if (loaded != null) return loaded;
        
        // Пытаемся найти через AssetDatabase (только в Editor)
        #if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:Texture2D " + System.IO.Path.GetFileName(path));
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (assetPath.ToLower().Contains(path.Replace("Models/", "").ToLower()))
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            }
        }
        #endif
        
        return null;
    }
    
    private void InitializeHats()
    {
        // Загружаем текстуры и создаем материалы перед инициализацией персонажей
        LoadHelmetTextures();
        
        // Инициализация шапок для всех персонажей
        foreach (var characterHats in characters)
        {
            if (characterHats != null && characterHats.characterObject != null)
            {
                InitializeCharacterHats(characterHats, characterHats.characterObject.transform);
            }
        }
        
        // Создаем материалы после инициализации рендереров
        CreateHelmetMaterials();
        
        // Скрываем все шапки по умолчанию
        SetHatType(HatType.None);
    }
    
    private void InitializeCharacterHats(CharacterHats hats, Transform parent)
    {
        // Создаем кокошник
        if (hats.kokoshnikInstance == null)
        {
            GameObject prefab = GetModelPrefab(kokoshnikPrefab, kokoshnikModelPath);
            if (prefab != null)
            {
                hats.kokoshnikInstance = Instantiate(prefab, parent);
                hats.kokoshnikInstance.SetActive(false);
            }
        }
        
        // Создаем бороду
        if (hats.borodaInstance == null)
        {
            GameObject prefab = GetModelPrefab(borodaPrefab, borodaModelPath);
            if (prefab != null)
            {
                hats.borodaInstance = Instantiate(prefab, parent);
                hats.borodaInstance.SetActive(false);
            }
        }
        
        // Создаем каску
        if (hats.helmetInstance == null)
        {
            GameObject prefab = GetModelPrefab(helmetPrefab, helmetModelPath);
            if (prefab != null)
            {
                hats.helmetInstance = Instantiate(prefab, parent);
                hats.helmetInstance.SetActive(false);
                
                // Получаем рендерер каски
                hats.helmetRenderer = hats.helmetInstance.GetComponentInChildren<Renderer>();
            }
        }
        
        // Создаем ящик
        if (hats.orangeBoxInstance == null)
        {
            GameObject prefab = GetModelPrefab(orangeBoxPrefab, orangeBoxModelPath);
            if (prefab != null)
            {
                hats.orangeBoxInstance = Instantiate(prefab, parent);
                hats.orangeBoxInstance.SetActive(false);
            }
        }
    }
    
    private GameObject GetModelPrefab(GameObject prefab, string resourcePath)
    {
        // Если префаб назначен напрямую, используем его
        if (prefab != null)
        {
            return prefab;
        }
        
        // Иначе пытаемся загрузить из Resources
        GameObject loaded = Resources.Load<GameObject>(resourcePath);
        if (loaded != null)
        {
            return loaded;
        }
        
        Debug.LogWarning($"Не удалось загрузить модель по пути: {resourcePath}. Назначьте префаб в Inspector.");
        return null;
    }
    
    private void LoadHelmetTextures()
    {
        // Загружаем текстуры напрямую из папки Assets, если они не назначены
        // В Unity это нужно делать через AssetDatabase в Editor или через Resources
        // Для runtime используем Resources, но текстуры должны быть в папке Resources
        
        // Альтернативный способ - загрузить через путь к файлу
        // Но проще назначить текстуры в Inspector
        
        // Если текстуры не назначены в Inspector, попробуем загрузить из Resources
        // (требует перемещения текстур в папку Resources)
        if (helmetDefaultTexture == null)
        {
            // Попытка загрузить из Resources (если текстуры там)
            helmetDefaultTexture = Resources.Load<Texture2D>("lambert1_Base");
        }
        if (helmetBlueTexture == null)
        {
            helmetBlueTexture = Resources.Load<Texture2D>("lambert1_blue_Base");
        }
        if (helmetWhiteTexture == null)
        {
            helmetWhiteTexture = Resources.Load<Texture2D>("lambert1_white_Base");
        }
    }
    
    private void CreateHelmetMaterials()
    {
        // Создаем материалы для разных цветов, если они не назначены
        Renderer helmetRenderer = null;
        foreach (var characterHats in characters)
        {
            if (characterHats != null && characterHats.helmetRenderer != null)
            {
                helmetRenderer = characterHats.helmetRenderer;
                break;
            }
        }
        
        if (helmetDefaultMaterial == null && helmetRenderer != null)
        {
            // Используем материал из рендерера как базовый
            Material baseMaterial = helmetRenderer.sharedMaterial;
            if (baseMaterial != null)
            {
                helmetDefaultMaterial = new Material(baseMaterial);
                if (helmetDefaultTexture != null)
                {
                    // Пробуем разные имена свойств текстуры
                    if (helmetDefaultMaterial.HasProperty("_MainTexture"))
                        helmetDefaultMaterial.SetTexture("_MainTexture", helmetDefaultTexture);
                    else if (helmetDefaultMaterial.HasProperty("_BaseMap"))
                        helmetDefaultMaterial.SetTexture("_BaseMap", helmetDefaultTexture);
                    else if (helmetDefaultMaterial.HasProperty("_MainTex"))
                        helmetDefaultMaterial.SetTexture("_MainTex", helmetDefaultTexture);
                }
            }
        }
        
        if (helmetBlueMaterial == null && helmetDefaultMaterial != null)
        {
            helmetBlueMaterial = new Material(helmetDefaultMaterial);
            if (helmetBlueTexture != null)
            {
                if (helmetBlueMaterial.HasProperty("_MainTexture"))
                    helmetBlueMaterial.SetTexture("_MainTexture", helmetBlueTexture);
                else if (helmetBlueMaterial.HasProperty("_BaseMap"))
                    helmetBlueMaterial.SetTexture("_BaseMap", helmetBlueTexture);
                else if (helmetBlueMaterial.HasProperty("_MainTex"))
                    helmetBlueMaterial.SetTexture("_MainTex", helmetBlueTexture);
            }
        }
        
        if (helmetWhiteMaterial == null && helmetDefaultMaterial != null)
        {
            helmetWhiteMaterial = new Material(helmetDefaultMaterial);
            if (helmetWhiteTexture != null)
            {
                if (helmetWhiteMaterial.HasProperty("_MainTexture"))
                    helmetWhiteMaterial.SetTexture("_MainTexture", helmetWhiteTexture);
                else if (helmetWhiteMaterial.HasProperty("_BaseMap"))
                    helmetWhiteMaterial.SetTexture("_BaseMap", helmetWhiteTexture);
                else if (helmetWhiteMaterial.HasProperty("_MainTex"))
                    helmetWhiteMaterial.SetTexture("_MainTex", helmetWhiteTexture);
            }
        }
    }
    
    public void SetHatType(HatType hatType)
    {
        currentHatType = hatType;
        
        // Скрываем все шапки у всех персонажей
        foreach (var characterHats in characters)
        {
            if (characterHats != null)
            {
                SetCharacterHatsActive(characterHats, false);
            }
        }
        
        // Показываем выбранную шапку у всех персонажей
        if (hatType != HatType.None)
        {
            foreach (var characterHats in characters)
            {
                if (characterHats != null && characterHats.characterObject != null)
                {
                    SetCharacterHatsActive(characterHats, hatType, true);
                }
            }
        }
        
        // Уведомляем об изменении
        OnHatTypeChanged?.Invoke(hatType);
    }
    
    public List<Renderer> GetActiveHatRenderers()
    {
        List<Renderer> renderers = new List<Renderer>();
        
        if (currentHatType == HatType.None) return renderers;
        
        // Получаем рендереры для всех персонажей
        foreach (var characterHats in characters)
        {
            if (characterHats != null)
            {
                AddCharacterHatRenderers(characterHats, renderers);
            }
        }
        
        return renderers;
    }
    
    private void AddCharacterHatRenderers(CharacterHats hats, List<Renderer> renderers)
    {
        switch (currentHatType)
        {
            case HatType.Kokoshnik:
                if (hats.kokoshnikInstance != null)
                {
                    renderers.AddRange(hats.kokoshnikInstance.GetComponentsInChildren<Renderer>());
                }
                break;
            case HatType.Boroda:
                if (hats.borodaInstance != null)
                {
                    renderers.AddRange(hats.borodaInstance.GetComponentsInChildren<Renderer>());
                }
                break;
            case HatType.Helmet:
                if (hats.helmetInstance != null && hats.helmetRenderer != null)
                {
                    renderers.Add(hats.helmetRenderer);
                }
                break;
            case HatType.OrangeBox:
                if (hats.orangeBoxInstance != null)
                {
                    renderers.AddRange(hats.orangeBoxInstance.GetComponentsInChildren<Renderer>());
                }
                break;
        }
    }
    
    public void SetHelmetColor(HelmetColor color)
    {
        currentHelmetColor = color;
        
        if (currentHatType == HatType.Helmet)
        {
            foreach (var characterHats in characters)
            {
                if (characterHats != null)
                {
                    UpdateHelmetColors(characterHats, color);
                }
            }
        }
    }
    
    private void UpdateHelmetColors(CharacterHats hats, HelmetColor color)
    {
        if (hats.helmetRenderer == null) return;
        
        // Меняем материал рендерера в зависимости от выбранного цвета
        Material materialToUse = null;
        switch (color)
        {
            case HelmetColor.Default:
                materialToUse = helmetDefaultMaterial;
                break;
            case HelmetColor.Blue:
                materialToUse = helmetBlueMaterial;
                break;
            case HelmetColor.White:
                materialToUse = helmetWhiteMaterial;
                break;
        }
        
        if (materialToUse != null)
        {
            hats.helmetRenderer.sharedMaterial = materialToUse;
        }
    }
    
    private void SetCharacterHatsActive(CharacterHats hats, bool active)
    {
        if (hats.kokoshnikInstance != null) hats.kokoshnikInstance.SetActive(false);
        if (hats.borodaInstance != null) hats.borodaInstance.SetActive(false);
        if (hats.helmetInstance != null) hats.helmetInstance.SetActive(false);
        if (hats.orangeBoxInstance != null) hats.orangeBoxInstance.SetActive(false);
    }
    
    private void SetCharacterHatsActive(CharacterHats hats, HatType hatType, bool active)
    {
        switch (hatType)
        {
            case HatType.Kokoshnik:
                if (hats.kokoshnikInstance != null) hats.kokoshnikInstance.SetActive(active);
                break;
            case HatType.Boroda:
                if (hats.borodaInstance != null) hats.borodaInstance.SetActive(active);
                break;
            case HatType.Helmet:
                if (hats.helmetInstance != null)
                {
                    hats.helmetInstance.SetActive(active);
                    if (active) UpdateHelmetColors(hats, currentHelmetColor);
                }
                break;
            case HatType.OrangeBox:
                if (hats.orangeBoxInstance != null) hats.orangeBoxInstance.SetActive(active);
                break;
        }
    }
    
    public HatType GetCurrentHatType()
    {
        return currentHatType;
    }
    
    public HelmetColor GetCurrentHelmetColor()
    {
        return currentHelmetColor;
    }
}
