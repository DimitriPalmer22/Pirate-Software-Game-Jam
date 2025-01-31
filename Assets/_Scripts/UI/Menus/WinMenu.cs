using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : GameMenu
{
    [SerializeField] private string mainMenuSceneName;

    protected override void CustomAwake()
    {
    }

    protected override void CustomStart()
    {
        // Connect to the onWaveComplete event of the wave manager
        WaveManager.Instance.onWaveComplete += ActivateOnWaveComplete;
    }

    private void ActivateOnWaveComplete()
    {
        // Return if the current wave is not a boss wave
        if (!WaveManager.Instance.CurrentWave.IsBossWave)
            return;

        // // Return if the current wave is not the last wave (25)
        // if (WaveManager.Instance.CurrentWaveIndex != WaveManager.WAVE_CYCLE * 5 + 1)
        //     return;

        if (WaveManager.Instance.BossWaveCount != 5)
            return;

        // Activate the win menu
        Activate();
    }

    protected override void CustomDestroy()
    {
    }

    protected override void CustomActivate()
    {
    }

    protected override void CustomDeactivate()
    {
    }

    protected override void CustomUpdate()
    {
    }

    public override void OnBackPressed()
    {
    }

    public void OnMainMenuPressed()
    {
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}