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

    [SerializeField] private RawImage transparentSampleCouple;
    [SerializeField] private float transparentSampleDuration = 0.2f;
    
    [SerializeField] private List<Graphic> imageListToDisable = new();
    [SerializeField] private List<Graphic> imageListToEnable = new();
    
    [SerializeField] private float dissolveDuration = 0.5f;
    [SerializeField] private float transparentUIDuration = 0.5f;

    private Renderer _genaObjectRenderer;
    private ParticleSystem _genaObjectParticleSystem;

    private MindARImageTrackingManager imageTracker;

    private void Start()
    {
        _genaObjectRenderer = genaObject.GetComponent<Renderer>();
        _genaObjectParticleSystem = genaObject.GetComponentInChildren<ParticleSystem>();
        
        _genaObjectRenderer.sharedMaterial.SetFloat(Amount, 1);
        
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
    
    

    private void OnTargetFound(int targetIndex)
    {
        print("Target found: " + targetIndex);
        if (targetIndex is 0 or 1)
        {
            genaObject.SetActive(true);
            
            Tween.StopAll(onTarget: transparentSampleCouple);
            Tween.Alpha(transparentSampleCouple, 0, transparentSampleDuration);
            
            Tween.StopAll(onTarget: _genaObjectRenderer.sharedMaterial);
            Tween.MaterialProperty(_genaObjectRenderer.sharedMaterial, Amount, 0, dissolveDuration);
            _genaObjectParticleSystem.Play();
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
        
        Tween.StopAll(onTarget: _genaObjectRenderer.sharedMaterial);
        Tween.MaterialProperty(_genaObjectRenderer.sharedMaterial, Amount, 1, dissolveDuration).OnComplete(() => genaObject.SetActive(false));
        _genaObjectParticleSystem.Stop();
    }
}