using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WebARFoundation;

public class ARController : MonoBehaviour
{
    private static readonly int Amount = Shader.PropertyToID("_Amount");
    [SerializeField] private GameObject genaObject;
    [SerializeField] private GameObject shopoklyakObject;

    [SerializeField] private RawImage transparentSampleCouple;
    [SerializeField] private float transparentSampleDuration = 0.2f;
    
    [SerializeField] private List<Graphic> imageListToDisable = new();
    [SerializeField] private List<Graphic> imageListToEnable = new();
    
    [SerializeField] private float dissolveDuration = 0.5f;
    [SerializeField] private float transparentUIDuration = 0.5f;

    [SerializeField] private List<Renderer> rendersToDissolve;
    [SerializeField] private ParticleSystem _snowfallParticleSystem;
    
    [SerializeField] private HatController hatController;

    private MindARImageTrackingManager imageTracker;

    private void Start()
    {
        foreach (var render in rendersToDissolve)
        {
            render.sharedMaterial.SetFloat(Amount, 1);
        }
        
        // Добавляем рендереры шапок в список для анимации dissolve
        if (hatController != null)
        {
            AddHatRenderersToDissolveList();
        }
        
        imageTracker = GetComponent<MindARImageTrackingManager>();
        imageTracker.onTargetFoundEvent += OnTargetFound;
        imageTracker.onTargetLostEvent += OnTargetLost;

        imageTracker.OnARStarted += () =>
        {
            foreach (var image in imageListToDisable)
            {
                Tween.Alpha(image, 0, transparentUIDuration).
                    OnComplete(() => Tween.Alpha(transparentSampleCouple, 0.5f, transparentSampleDuration));
            }
            
            foreach (var image in imageListToEnable)
            {
                Tween.Alpha(image, 1, transparentUIDuration);
            }
        };
    }
    
    private void AddHatRenderersToDissolveList()
    {
        // Подписываемся на изменение шапок
        if (hatController != null)
        {
            hatController.OnHatTypeChanged += OnHatTypeChanged;
        }
    }
    
    private void OnHatTypeChanged(HatType hatType)
    {
        // Обновляем список рендереров для анимации dissolve
        UpdateRendersToDissolve();
    }
    
    private void UpdateRendersToDissolve()
    {
        if (hatController == null) return;
        
        // Получаем активные рендереры шапок
        var hatRenderers = hatController.GetActiveHatRenderers();
        
        // Добавляем их в список для анимации (если еще не добавлены)
        foreach (var renderer in hatRenderers)
        {
            if (!rendersToDissolve.Contains(renderer))
            {
                rendersToDissolve.Add(renderer);
                // Инициализируем материал для dissolve эффекта
                if (renderer.sharedMaterial != null)
                {
                    renderer.sharedMaterial.SetFloat(Amount, 1);
                }
            }
        }
    }
    
    

    private void OnTargetFound(int targetIndex)
    {
        print("Target found: " + targetIndex);
        if (targetIndex is 0 or 1)
        {
            // Обновляем список рендереров перед показом
            UpdateRendersToDissolve();
            
            Tween.StopAll(onTarget: transparentSampleCouple);
            Tween.Alpha(transparentSampleCouple, 0, transparentSampleDuration);

            foreach (var renderer in rendersToDissolve)
            {
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    Tween.CompleteAll(onTarget: renderer.sharedMaterial);
                    Tween.MaterialProperty(renderer.sharedMaterial, Amount, 0, dissolveDuration);
                    
                    renderer.gameObject.SetActive(true);
                }
            }
            
            _snowfallParticleSystem.Play();
        }
        else if (targetIndex is 2 or 3)
        {
            //wolfObject.SetActive(true);
        }
    }

    private void OnTargetLost(int targetIndex)
    {
        Tween.StopAll(onTarget: transparentSampleCouple);
        Tween.Alpha(transparentSampleCouple, 0.5f, transparentSampleDuration);
        
        foreach (var renderer in rendersToDissolve)
        {
            Tween.CompleteAll(onTarget: renderer.sharedMaterial);
            Tween.MaterialProperty(renderer.sharedMaterial, Amount, 1, dissolveDuration).OnComplete(() => renderer.gameObject.SetActive(false));
        }
        
        _snowfallParticleSystem.Stop();
    }
}