using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class PhotoBuffer
{
    public static List<byte[]> Photos = new List<byte[]>();
}

public class ScreenshotCapture : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    [SerializeField] private Image cameraShotVisual;
    [SerializeField] private float shotDuration;
    
    public Button captureButton;
    public Button galleryButton;
    public GameObject galleryPanel;

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
    }

    public void CaptureAndAddToBuffer()
    {
        Sequence.Create(1, CycleMode.Yoyo)
            .Chain(Tween.Alpha(cameraShotVisual, 1, shotDuration))
            .Chain(Tween.Alpha(cameraShotVisual, 0, shotDuration));
        StartCoroutine(TakeScreenshot(addToBuffer: true));
    }

    public void DownloadPhoto(byte[] photoBytes)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"ar_photo_{timestamp}.png";
        DownloadFile(photoBytes, photoBytes.Length, fileName);
    }

    private IEnumerator TakeScreenshot(bool addToBuffer = false)
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        Destroy(screenshot);

        if (addToBuffer)
        {
            PhotoBuffer.Photos.Add(bytes);
            Debug.Log("Фото добавлено в буфер. Всего фото: " + PhotoBuffer.Photos.Count);
        }
    }

    private void OpenGallery()
    {
        galleryPanel.SetActive(true);
    }
}