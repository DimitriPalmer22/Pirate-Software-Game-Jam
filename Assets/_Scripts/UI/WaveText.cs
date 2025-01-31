﻿using System;
using TMPro;
using UnityEngine;

public class WaveText : MonoBehaviour
{
    private const float OPACITY_THRESHOLD = .001f;

    #region Serialized Fields

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text text;

    [SerializeField, Range(0, 1)] private float maxAlpha = 1f;
    [SerializeField, Min(0)] private float lerpAmount = .2f;
    [SerializeField, Min(0)] private float stayOnScreenTime = 1f;

    #endregion

    #region Private Fields

    private float _desiredOpacity;
    private CountdownTimer _stayOnScreenTimer;

    #endregion

    private void Awake()
    {
        // Initialize the stay on screen timer
        _stayOnScreenTimer = new CountdownTimer(stayOnScreenTime);
        _stayOnScreenTimer.OnTimerEnd += () => { _desiredOpacity = 0; };
        _stayOnScreenTimer.Stop();

        // Set the text to be invisible
        _desiredOpacity = 0;
        canvasGroup.alpha = 0;
    }

    private void OnWaveStart()
    {
        // Set the desired opacity to the max
        _desiredOpacity = maxAlpha;

        // Reset the stay on screen timer
        _stayOnScreenTimer.SetMaxTimeAndReset(stayOnScreenTime);
        _stayOnScreenTimer.Start();

        // Set the text
        if (!WaveManager.Instance.CurrentWave.IsBossWave)
            text.text = $"Wave {WaveManager.Instance.CurrentWaveIndex}";
        else
            text.text = "Boss Wave!";
    }

    private void Start()
    {
        // Connect to the wave manager
        WaveManager.Instance.onWaveStart += OnWaveStart;
    }

    private void Update()
    {
        if (WaveManager.Instance.TimeBetweenWavesRemaining > 0)
        {
            _stayOnScreenTimer.SetMaxTimeAndReset(stayOnScreenTime);
            _desiredOpacity = maxAlpha;
            text.text = $"Next Wave In\n{WaveManager.Instance.TimeBetweenWavesRemaining:0.00}";
        }

        // Update the stay on screen timer
        _stayOnScreenTimer.SetMaxTime(stayOnScreenTime);
        _stayOnScreenTimer.Update(Time.deltaTime);

        // Lerp the opacity
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _desiredOpacity, CustomFunctions.FrameAmount(lerpAmount));

        // If the opacity is close enough to the desired opacity, set it to the desired opacity
        if (Math.Abs(canvasGroup.alpha - _desiredOpacity) < OPACITY_THRESHOLD)
            canvasGroup.alpha = _desiredOpacity;
    }
}