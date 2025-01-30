using System;
using UnityEngine;


public class CanvasGroupAlphaControl : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, Range(0, 1)] private float lerpAmount = .2f;
    [SerializeField] private float startAlpha = 1;
    
    private float _desiredAlpha = 1;

    private void Start()
    {
        // Set the alpha to the start alpha
        _desiredAlpha = startAlpha;
        canvasGroup.alpha = startAlpha;
    }

    public void SetAlpha(float alpha)
    {
        _desiredAlpha = alpha;
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _desiredAlpha, CustomFunctions.FrameAmount(lerpAmount, false, true));
    }
}