using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    [SerializeField] private CharacterHats genaHats;
    [SerializeField] private CharacterHats shopoklyakHats;
    
    [SerializeField] private GameObject kokoshnikPrefab;
    [SerializeField] private GameObject borodaPrefab;
    [SerializeField] private GameObject helmetPrefab;
    [SerializeField] private GameObject orangeBoxPrefab;
    
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
        InitializeHats();
    }
    
    private void InitializeHats()
    {
        // Загружаем текстуры и создаем материалы перед инициализацией персонажей
        LoadHelmetTextures();
        
        // Инициализация шапок для Гены
        if (genaHats.characterObject != null)
        {
            InitializeCharacterHats(genaHats, genaHats.characterObject.transform);
        }
        
        // Инициализация шапок для Шапокляк
        if (shopoklyakHats.characterObject != null)
        {
            InitializeCharacterHats(shopoklyakHats, shopoklyakHats.characterObject.transform);
        }
        
        // Создаем материалы после инициализации рендереров
        CreateHelmetMaterials();
        
        // Скрываем все шапки по умолчанию
        SetHatType(HatType.None);
    }
    
    private void InitializeCharacterHats(CharacterHats hats, Transform parent)
    {
        // Создаем кокошник
        if (kokoshnikPrefab != null && hats.kokoshnikInstance == null)
        {
            hats.kokoshnikInstance = Instantiate(kokoshnikPrefab, parent);
            hats.kokoshnikInstance.SetActive(false);
        }
        
        // Создаем бороду
        if (borodaPrefab != null && hats.borodaInstance == null)
        {
            hats.borodaInstance = Instantiate(borodaPrefab, parent);
            hats.borodaInstance.SetActive(false);
        }
        
        // Создаем каску
        if (helmetPrefab != null)
        {
            if (hats.helmetInstance == null)
            {
                hats.helmetInstance = Instantiate(helmetPrefab, parent);
                hats.helmetInstance.SetActive(false);
                
                // Получаем рендерер каски
                hats.helmetRenderer = hats.helmetInstance.GetComponentInChildren<Renderer>();
            }
        }
        
        // Создаем ящик
        if (orangeBoxPrefab != null && hats.orangeBoxInstance == null)
        {
            hats.orangeBoxInstance = Instantiate(orangeBoxPrefab, parent);
            hats.orangeBoxInstance.SetActive(false);
        }
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
        Renderer helmetRenderer = genaHats.helmetRenderer ?? shopoklyakHats.helmetRenderer;
        
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
        
        // Скрываем все шапки
        SetCharacterHatsActive(genaHats, false);
        SetCharacterHatsActive(shopoklyakHats, false);
        
        // Показываем выбранную шапку
        if (hatType != HatType.None)
        {
            SetCharacterHatsActive(genaHats, hatType, true);
            SetCharacterHatsActive(shopoklyakHats, hatType, true);
        }
        
        // Уведомляем об изменении
        OnHatTypeChanged?.Invoke(hatType);
    }
    
    public List<Renderer> GetActiveHatRenderers()
    {
        List<Renderer> renderers = new List<Renderer>();
        
        if (currentHatType == HatType.None) return renderers;
        
        // Получаем рендереры для Гены
        AddCharacterHatRenderers(genaHats, renderers);
        
        // Получаем рендереры для Шапокляк
        AddCharacterHatRenderers(shopoklyakHats, renderers);
        
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
            UpdateHelmetColors(genaHats, color);
            UpdateHelmetColors(shopoklyakHats, color);
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
