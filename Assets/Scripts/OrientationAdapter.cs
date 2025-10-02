using System;
using UnityEngine;
using UnityEngine.UI;

public class OrientationAdapter : MonoBehaviour
{
    [SerializeField] private RawImage transparentSampleCouple;
    
    public RectTransform headerRect; // HeaderText
    public RectTransform buttonRect; // SaveButton
    public RectTransform iconLeft; // CharacterIconLeft
    public RectTransform iconRight; // CharacterIconRight
    
    private ScreenOrientation _previousOrientation = ScreenOrientation.Portrait;

    [SerializeField] private Rect transparentSampleCouplePortrait;
    [SerializeField] private Rect transparentSampleCoupleLandscape;

    private void Start()
    {
        if (transparentSampleCouple == null) Debug.LogWarning("Transparent sample couple is null in OrientationAdapter.", this);

        transparentSampleCouplePortrait.width = Display.main.systemWidth - 100;
        transparentSampleCouplePortrait.height = transparentSampleCouplePortrait.width / 4 * 3 - 100;
        
        transparentSampleCoupleLandscape.width = Display.main.systemHeight - 100;
        transparentSampleCoupleLandscape.height = transparentSampleCoupleLandscape.width / 4 * 3 - 100;
        
        
        
        UpdateUI();
    }

    private void Update()
    {
        if (_previousOrientation != Screen.orientation)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            transparentSampleCouple.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transparentSampleCouplePortrait.width);
            transparentSampleCouple.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, transparentSampleCouplePortrait.height);
        }
        else
        {
            transparentSampleCouple.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transparentSampleCoupleLandscape.width);
            transparentSampleCouple.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, transparentSampleCoupleLandscape.height);
        }
        _previousOrientation = Screen.orientation;
    }
}