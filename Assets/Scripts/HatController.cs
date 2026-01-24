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
        public GameObject helmetDefaultInstance;  // Каска по умолчанию
        public GameObject helmetBlueInstance;     // Синяя каска
        public GameObject helmetWhiteInstance;    // Белая каска
        public GameObject orangeBoxInstance;
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
        
        // Автоматически находим текстуры касок из папки Assets/Models/helmet/
        if (helmetDefaultTexture == null)
        {
            helmetDefaultTexture = LoadTextureFromAssets("lambert1_Base");
        }
        if (helmetBlueTexture == null)
        {
            helmetBlueTexture = LoadTextureFromAssets("lambert1_blue_Base");
        }
        if (helmetWhiteTexture == null)
        {
            helmetWhiteTexture = LoadTextureFromAssets("lambert1_white_Base");
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
    
    private Texture2D LoadTextureFromAssets(string textureName)
    {
        // Пытаемся найти текстуру в папке Assets/Models/helmet/
        #if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets(textureName + " t:Texture2D");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // Проверяем, что текстура находится в папке helmet
            if (assetPath.ToLower().Contains("helmet") && assetPath.ToLower().Contains(textureName.ToLower()))
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (texture != null)
                {
                    return texture;
                }
            }
        }
        #endif
        
        // Пытаемся загрузить через Resources как fallback
        return Resources.Load<Texture2D>("Models/helmet/" + textureName);
    }
    
    private void InitializeHats()
    {
        // ШАГ 1: Загружаем текстуры
        LoadHelmetTextures();
        
        // ШАГ 2: Создаем материалы (нужно сделать до создания экземпляров касок)
        CreateHelmetMaterials();
        
        // ШАГ 3: Инициализация шапок для всех персонажей
        foreach (var characterHats in characters)
        {
            if (characterHats != null && characterHats.characterObject != null)
            {
                InitializeCharacterHats(characterHats, characterHats.characterObject.transform);
            }
        }
        
        // ШАГ 4: Применяем материалы к созданным каскам (если они еще не применены)
        foreach (var characterHats in characters)
        {
            if (characterHats != null && characterHats.characterObject != null)
            {
                if (characterHats.helmetDefaultInstance != null)
                    ApplyHelmetMaterial(characterHats.helmetDefaultInstance, HelmetColor.Default);
                if (characterHats.helmetBlueInstance != null)
                    ApplyHelmetMaterial(characterHats.helmetBlueInstance, HelmetColor.Blue);
                if (characterHats.helmetWhiteInstance != null)
                    ApplyHelmetMaterial(characterHats.helmetWhiteInstance, HelmetColor.White);
            }
        }
        
        // ШАГ 5: Скрываем все шапки по умолчанию
        SetHatType(HatType.None);
    }
    
    private void InitializeCharacterHats(CharacterHats hats, Transform parent)
    {
        // Создаем кокошник
        if (hats.kokoshnikInstance == null)
        {
            GameObject kokoshnikModel = GetModelPrefab(kokoshnikPrefab, kokoshnikModelPath);
            if (kokoshnikModel != null)
            {
                hats.kokoshnikInstance = Instantiate(kokoshnikModel, parent);
                hats.kokoshnikInstance.SetActive(false);
            }
        }
        
        // Создаем бороду
        if (hats.borodaInstance == null)
        {
            GameObject borodaModel = GetModelPrefab(borodaPrefab, borodaModelPath);
            if (borodaModel != null)
            {
                hats.borodaInstance = Instantiate(borodaModel, parent);
                hats.borodaInstance.SetActive(false);
            }
        }
        
        // Создаем 3 варианта каски с разными цветами
        GameObject helmetModel = GetModelPrefab(helmetPrefab, helmetModelPath);
        if (helmetModel != null)
        {
            // Каска по умолчанию
            if (hats.helmetDefaultInstance == null)
            {
                hats.helmetDefaultInstance = Instantiate(helmetModel, parent);
                hats.helmetDefaultInstance.name = "Helmet_Default";
                hats.helmetDefaultInstance.SetActive(false);
                ApplyHelmetMaterial(hats.helmetDefaultInstance, HelmetColor.Default);
            }
            
            // Синяя каска
            if (hats.helmetBlueInstance == null)
            {
                hats.helmetBlueInstance = Instantiate(helmetModel, parent);
                hats.helmetBlueInstance.name = "Helmet_Blue";
                hats.helmetBlueInstance.SetActive(false);
                ApplyHelmetMaterial(hats.helmetBlueInstance, HelmetColor.Blue);
            }
            
            // Белая каска
            if (hats.helmetWhiteInstance == null)
            {
                hats.helmetWhiteInstance = Instantiate(helmetModel, parent);
                hats.helmetWhiteInstance.name = "Helmet_White";
                hats.helmetWhiteInstance.SetActive(false);
                ApplyHelmetMaterial(hats.helmetWhiteInstance, HelmetColor.White);
            }
        }
        
        // Создаем ящик
        if (hats.orangeBoxInstance == null)
        {
            GameObject orangeBoxModel = GetModelPrefab(orangeBoxPrefab, orangeBoxModelPath);
            if (orangeBoxModel != null)
            {
                hats.orangeBoxInstance = Instantiate(orangeBoxModel, parent);
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
        // Получаем базовый материал из префаба каски
        GameObject prefab = GetModelPrefab(helmetPrefab, helmetModelPath);
        Material baseMaterial = null;
        
        if (prefab != null)
        {
            Renderer prefabRenderer = prefab.GetComponentInChildren<Renderer>();
            if (prefabRenderer != null && prefabRenderer.sharedMaterial != null)
            {
                baseMaterial = prefabRenderer.sharedMaterial;
            }
        }
        
        // Если базовый материал не найден, используем материал из существующего экземпляра
        if (baseMaterial == null)
        {
            foreach (var characterHats in characters)
            {
                if (characterHats != null && characterHats.helmetDefaultInstance != null)
                {
                    Renderer renderer = characterHats.helmetDefaultInstance.GetComponentInChildren<Renderer>();
                    if (renderer != null && renderer.sharedMaterial != null)
                    {
                        baseMaterial = renderer.sharedMaterial;
                        break;
                    }
                }
            }
        }
        
        // Создаем материал для каски по умолчанию
        if (helmetDefaultMaterial == null && baseMaterial != null)
        {
            helmetDefaultMaterial = new Material(baseMaterial);
            ApplyTextureToMaterial(helmetDefaultMaterial, helmetDefaultTexture);
        }
        
        // Создаем материал для синей каски
        if (helmetBlueMaterial == null)
        {
            if (helmetDefaultMaterial != null)
            {
                helmetBlueMaterial = new Material(helmetDefaultMaterial);
            }
            else if (baseMaterial != null)
            {
                helmetBlueMaterial = new Material(baseMaterial);
            }
            ApplyTextureToMaterial(helmetBlueMaterial, helmetBlueTexture);
        }
        
        // Создаем материал для белой каски
        if (helmetWhiteMaterial == null)
        {
            if (helmetDefaultMaterial != null)
            {
                helmetWhiteMaterial = new Material(helmetDefaultMaterial);
            }
            else if (baseMaterial != null)
            {
                helmetWhiteMaterial = new Material(baseMaterial);
            }
            ApplyTextureToMaterial(helmetWhiteMaterial, helmetWhiteTexture);
        }
    }
    
    private void ApplyTextureToMaterial(Material material, Texture2D texture)
    {
        if (material == null || texture == null) return;
        
        // Пробуем разные имена свойств текстуры
        if (material.HasProperty("_MainTexture"))
            material.SetTexture("_MainTexture", texture);
        else if (material.HasProperty("_BaseMap"))
            material.SetTexture("_BaseMap", texture);
        else if (material.HasProperty("_MainTex"))
            material.SetTexture("_MainTex", texture);
        else if (material.HasProperty("_BaseColorMap"))
            material.SetTexture("_BaseColorMap", texture);
    }
    
    private void ApplyHelmetMaterial(GameObject helmetInstance, HelmetColor color)
    {
        if (helmetInstance == null) return;
        
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
        
        // Если материал еще не создан, создаем его сейчас
        if (materialToUse == null)
        {
            CreateHelmetMaterials();
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
        }
        
        if (materialToUse != null)
        {
            Renderer[] renderers = helmetInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    renderer.sharedMaterial = materialToUse;
                }
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
                // Добавляем рендерер активной каски (в зависимости от выбранного цвета)
                GameObject activeHelmet = null;
                switch (currentHelmetColor)
                {
                    case HelmetColor.Default:
                        activeHelmet = hats.helmetDefaultInstance;
                        break;
                    case HelmetColor.Blue:
                        activeHelmet = hats.helmetBlueInstance;
                        break;
                    case HelmetColor.White:
                        activeHelmet = hats.helmetWhiteInstance;
                        break;
                }
                
                if (activeHelmet != null)
                {
                    renderers.AddRange(activeHelmet.GetComponentsInChildren<Renderer>());
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
            // Обновляем активные каски для всех персонажей
            foreach (var characterHats in characters)
            {
                if (characterHats != null)
                {
                    UpdateHelmetColors(characterHats, color);
                }
            }
            
            // Уведомляем об изменении, чтобы обновить rendersToDissolve
            OnHatTypeChanged?.Invoke(HatType.Helmet);
        }
    }
    
    private void UpdateHelmetColors(CharacterHats hats, HelmetColor color)
    {
        // Скрываем все варианты каски
        if (hats.helmetDefaultInstance != null) hats.helmetDefaultInstance.SetActive(false);
        if (hats.helmetBlueInstance != null) hats.helmetBlueInstance.SetActive(false);
        if (hats.helmetWhiteInstance != null) hats.helmetWhiteInstance.SetActive(false);
        
        // Показываем выбранный вариант
        switch (color)
        {
            case HelmetColor.Default:
                if (hats.helmetDefaultInstance != null) hats.helmetDefaultInstance.SetActive(true);
                break;
            case HelmetColor.Blue:
                if (hats.helmetBlueInstance != null) hats.helmetBlueInstance.SetActive(true);
                break;
            case HelmetColor.White:
                if (hats.helmetWhiteInstance != null) hats.helmetWhiteInstance.SetActive(true);
                break;
        }
    }
    
    private void SetCharacterHatsActive(CharacterHats hats, bool active)
    {
        if (hats.kokoshnikInstance != null) hats.kokoshnikInstance.SetActive(false);
        if (hats.borodaInstance != null) hats.borodaInstance.SetActive(false);
        if (hats.helmetDefaultInstance != null) hats.helmetDefaultInstance.SetActive(false);
        if (hats.helmetBlueInstance != null) hats.helmetBlueInstance.SetActive(false);
        if (hats.helmetWhiteInstance != null) hats.helmetWhiteInstance.SetActive(false);
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
                if (active)
                {
                    // Показываем каску выбранного цвета
                    UpdateHelmetColors(hats, currentHelmetColor);
                }
                else
                {
                    // Скрываем все варианты каски
                    if (hats.helmetDefaultInstance != null) hats.helmetDefaultInstance.SetActive(false);
                    if (hats.helmetBlueInstance != null) hats.helmetBlueInstance.SetActive(false);
                    if (hats.helmetWhiteInstance != null) hats.helmetWhiteInstance.SetActive(false);
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
