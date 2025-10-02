using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    private static bool firstDownload = true;
    
    public Transform contentParent; // Content объекта в ScrollView (Vertical Layout Group)
    public GameObject thumbnailPrefab; // Prefab: RawImage с Button
    public GameObject fullViewPanel; // Панель для увеличенного фото (с RawImage и кнопкой "Загрузить")
    public RawImage fullViewImage; // RawImage для увеличенного фото
    public Button downloadButton; // Кнопка "Загрузить" в fullViewPanel
    public Button closeFullViewButton; // Кнопка "Закрыть" в fullViewPanel
    public GameObject bannerPanel; // Баннер после загрузки (Panel с Text и Button)
    public Button buyTicketsButton; // Кнопка "Купить билеты" в баннере
    public Button closeBannerButton; // Кнопка "Закрыть" в баннере
    public ScreenshotCapture captureScript; // Ссылка на ScreenshotCapture для DownloadPhoto

    private List<byte[]> currentPhotos; // Кэш фото из буфера
    private byte[] selectedPhoto; // Выбранное фото для скачивания

    void OnEnable()
    {
        LoadGallery();
    }

    private void LoadGallery()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        currentPhotos = PhotoBuffer.Photos;
        for (int i = 0; i < currentPhotos.Count; i++)
        {
            byte[] photoBytes = currentPhotos[i];
            GameObject thumb = Instantiate(thumbnailPrefab, contentParent);
            RawImage thumbImage = thumb.GetComponent<RawImage>();
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(photoBytes);
            thumbImage.texture = tex;

            thumb.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 500);

            Button thumbButton = thumb.GetComponent<Button>();
            int index = i;
            thumbButton.onClick.AddListener(() => ShowFullView(index));
        }
    }

    private void ShowFullView(int index)
    {
        selectedPhoto = currentPhotos[index];
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(selectedPhoto);
        fullViewImage.texture = tex;

        fullViewPanel.SetActive(true);

        downloadButton.onClick.RemoveAllListeners();
        downloadButton.onClick.AddListener(() =>
        {
            //captureScript.DownloadPhoto(selectedPhoto);
            ShowBanner();
        });

        closeFullViewButton.onClick.AddListener(() =>
        {
            fullViewPanel.SetActive(false);
            Destroy(tex);
        });
    }

    private void ShowBanner()
    {
        // Проверяем, первая ли это загрузка в сессии
        if (firstDownload)
        {
            print(1);
            bannerPanel.SetActive(true);

            // Кнопка "Купить билеты"
            buyTicketsButton.onClick.AddListener(() =>
            {
                Application.OpenURL("https://souzmultpark.ru/");
            });

            // Закрыть баннер
            closeBannerButton.onClick.AddListener(() =>
            {
                bannerPanel.SetActive(false);
            });

            // Устанавливаем флаг, чтобы больше не показывать баннер в этой сессии
            firstDownload = false;
        }
    }
}