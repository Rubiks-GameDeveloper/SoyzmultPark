using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotCapture : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    public Button saveButton; // Кнопка "Сохранить" в UI

    void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(CaptureAndDownload);
        }
        else
        {
            Debug.LogError("Save Button is not assigned in the Inspector!");
        }
    }

    public void CaptureAndDownload()
    {
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame(); // Ждать конца кадра для AR-вида

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        Destroy(screenshot);

        // Формируем имя файла с датой и временем
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"ar_photo_{timestamp}.png";

        // Скачать файл
        DownloadFile(bytes, bytes.Length, fileName);
    }
}