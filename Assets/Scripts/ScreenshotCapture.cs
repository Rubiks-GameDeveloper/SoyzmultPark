using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PhotoBuffer // Статический класс для хранения фото (буфер в памяти)
{
    public static List<byte[]> Photos = new List<byte[]>(); // Храним PNG-байты
}

public class ScreenshotCapture : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    public Button captureButton; // Кнопка "Сделать фото"
    public Button galleryButton; // Кнопка "Галерея" для открытия меню
    public GameObject galleryPanel; // UI-панель галереи
    public Canvas uiCanvas; // Ссылка на Canvas с UI (настройте в Inspector)
    private Camera arCamera; // AR-камера из WebAR Foundation

    void Start()
    {
        if (captureButton != null)
        {
            captureButton.onClick.AddListener(CaptureAndAddToBuffer);
        }
        if (galleryButton != null)
        {
            galleryButton.onClick.AddListener(OpenGallery);
        }
        arCamera = Camera.main; // Предполагаем, что AR-камера — главная
        if (uiCanvas == null) Debug.LogError("UI Canvas not assigned in Inspector!");
    }

    public void CaptureAndAddToBuffer()
    {
        StartCoroutine(TakeScreenshotWithoutUI(addToBuffer: true));
    }

    public void DownloadPhoto(byte[] photoBytes)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"ar_photo_{timestamp}.png";
        DownloadFile(photoBytes, photoBytes.Length, fileName);
    }

    private IEnumerator TakeScreenshotWithoutUI(bool addToBuffer = false)
    {
        // Отключаем UI
        if (uiCanvas != null) uiCanvas.enabled = false;

        // Создаем RenderTexture для захвата
        int width = Screen.width;
        int height = Screen.height;
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        arCamera.targetTexture = renderTexture;
        arCamera.Render(); // Рендерим сцену с 3D-объектами

        // Захватываем в Texture2D
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();
        RenderTexture.active = null; // Сбрасываем
        arCamera.targetTexture = null; // Возвращаем к нормальному рендеру
        Destroy(renderTexture); // Освобождаем память

        // Включаем UI обратно
        if (uiCanvas != null) uiCanvas.enabled = true;

        byte[] bytes = screenshot.EncodeToPNG();
        Destroy(screenshot); // Освобождаем память

        if (addToBuffer)
        {
            PhotoBuffer.Photos.Add(bytes); // Добавляем в буфер
            Debug.Log("Фото добавлено в буфер. Всего фото: " + PhotoBuffer.Photos.Count);
        }

        yield return null;
    }

    private void OpenGallery()
    {
        galleryPanel.SetActive(true); // Открываем меню галереи
    }
}