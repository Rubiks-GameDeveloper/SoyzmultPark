using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
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

            thumb.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);

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
            captureScript.DownloadPhoto(selectedPhoto);
            fullViewPanel.SetActive(false);
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
        if (PlayerPrefs.GetInt("FirstDownload", 0) == 0)
        {
            bannerPanel.SetActive(true);

            buyTicketsButton.onClick.AddListener(() =>
            {
                Application.OpenURL("https://souzmultpark.ru/");
            });

            closeBannerButton.onClick.AddListener(() =>
            {
                bannerPanel.SetActive(false);
            });

            PlayerPrefs.SetInt("FirstDownload", 1);
            PlayerPrefs.Save();
        }
    }
}