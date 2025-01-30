using System;
using TMPro;
using UnityEngine;

public class GameInformation : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text waveText;

    #endregion

    #region Private Fields

    #endregion

    #region Getters

    #endregion

    private void Start()
    {
    }

    private void Update()
    {
        // Update the wave text
        UpdateWaveText();
    }

    private void UpdateWaveText()
    {
        var waveManager = WaveManager.Instance;

        // Return if the instance of the WaveManager is null
        if (waveManager == null)
        {
            waveText.text = "Wave ???";
            return;
        }

        // If the wave manager is waiting for the next wave or the current wave index is less than or equal to 0
        if (waveManager.IsWaitingForNextWave || waveManager.CurrentWaveIndex <= 0)
        {
            // Set the wave text to the next wave
            waveText.text = $"Wave {waveManager.CurrentWaveIndex + 1}";

            if (waveManager.NextWave.IsBossWave)
                waveText.text = "Boss Wave!";

            return;
        }

        // Set the wave text to the current wave
        waveText.text = $"Wave {waveManager.CurrentWaveIndex}";

        if (waveManager.CurrentWave.IsBossWave)
            waveText.text = "Boss Wave!";
    }
}