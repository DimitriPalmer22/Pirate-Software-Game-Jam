using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : GameMenu
{
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private TMP_Text waveText;

    protected override void CustomAwake()
    {
    }

    protected override void CustomStart()
    {
        // Connect to the onDeath event of the player
        Player.Instance.OnDeath += ActivateOnDeath;
        Player.Instance.OnDeath += UpdateWaveText;
    }

    private void UpdateWaveText(object sender, HealthChangedEventArgs args)
    {
        // Set the wave text to the current wave
        waveText.text = $"You Died On Wave {WaveManager.Instance.CurrentWaveIndex}";
    }

    private void ActivateOnDeath(object sender, HealthChangedEventArgs args)
    {
        // Activate the death menu
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