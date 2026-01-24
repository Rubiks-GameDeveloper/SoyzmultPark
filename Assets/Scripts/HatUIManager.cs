using UnityEngine;
using UnityEngine.UI;

public class HatUIManager : MonoBehaviour
{
    [Header("Hat Controller")]
    [SerializeField] private HatController hatController;
    
    [Header("Hat Type Buttons")]
    [SerializeField] private Button noneButton;
    [SerializeField] private Button kokoshnikButton;
    [SerializeField] private Button borodaButton;
    [SerializeField] private Button helmetButton;
    [SerializeField] private Button orangeBoxButton;
    
    [Header("Helmet Color Buttons")]
    [SerializeField] private GameObject helmetColorPanel;
    [SerializeField] private Button helmetDefaultButton;
    [SerializeField] private Button helmetBlueButton;
    [SerializeField] private Button helmetWhiteButton;
    
    [Header("UI Panel")]
    [SerializeField] private GameObject hatSelectionPanel;
    [SerializeField] private Button toggleHatPanelButton;
    
    private void Start()
    {
        InitializeButtons();
        
        // Скрываем панель выбора цвета каски по умолчанию
        if (helmetColorPanel != null)
        {
            helmetColorPanel.SetActive(false);
        }
        
        // Скрываем панель выбора шапок по умолчанию (можно настроить)
        if (hatSelectionPanel != null)
        {
            hatSelectionPanel.SetActive(true);
        }
    }
    
    private void InitializeButtons()
    {
        // Кнопки выбора типа шапки
        if (noneButton != null)
        {
            noneButton.onClick.AddListener(() => OnHatTypeSelected(HatType.None));
        }
        
        if (kokoshnikButton != null)
        {
            kokoshnikButton.onClick.AddListener(() => OnHatTypeSelected(HatType.Kokoshnik));
        }
        
        if (borodaButton != null)
        {
            borodaButton.onClick.AddListener(() => OnHatTypeSelected(HatType.Boroda));
        }
        
        if (helmetButton != null)
        {
            helmetButton.onClick.AddListener(() => OnHatTypeSelected(HatType.Helmet));
        }
        
        if (orangeBoxButton != null)
        {
            orangeBoxButton.onClick.AddListener(() => OnHatTypeSelected(HatType.OrangeBox));
        }
        
        // Кнопки выбора цвета каски
        if (helmetDefaultButton != null)
        {
            helmetDefaultButton.onClick.AddListener(() => OnHelmetColorSelected(HelmetColor.Default));
        }
        
        if (helmetBlueButton != null)
        {
            helmetBlueButton.onClick.AddListener(() => OnHelmetColorSelected(HelmetColor.Blue));
        }
        
        if (helmetWhiteButton != null)
        {
            helmetWhiteButton.onClick.AddListener(() => OnHelmetColorSelected(HelmetColor.White));
        }
        
        // Кнопка переключения панели
        if (toggleHatPanelButton != null)
        {
            toggleHatPanelButton.onClick.AddListener(ToggleHatPanel);
        }
    }
    
    private void OnHatTypeSelected(HatType hatType)
    {
        if (hatController != null)
        {
            hatController.SetHatType(hatType);
            
            // Показываем/скрываем панель выбора цвета каски
            if (helmetColorPanel != null)
            {
                helmetColorPanel.SetActive(hatType == HatType.Helmet);
            }
        }
    }
    
    private void OnHelmetColorSelected(HelmetColor color)
    {
        if (hatController != null)
        {
            hatController.SetHelmetColor(color);
        }
    }
    
    public void ToggleHatPanel()
    {
        if (hatSelectionPanel != null)
        {
            hatSelectionPanel.SetActive(!hatSelectionPanel.activeSelf);
        }
    }
    
    public void ShowHatPanel()
    {
        if (hatSelectionPanel != null)
        {
            hatSelectionPanel.SetActive(true);
        }
    }
    
    public void HideHatPanel()
    {
        if (hatSelectionPanel != null)
        {
            hatSelectionPanel.SetActive(false);
        }
    }
}
