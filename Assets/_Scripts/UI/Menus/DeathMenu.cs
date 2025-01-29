using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : GameMenu
{
    [SerializeField] private string mainMenuSceneName;

    protected override void CustomAwake()
    {
    }

    protected override void CustomStart()
    {
        // Connect to the onDeath event of the player
        Player.Instance.OnDeath += ActivateOnDeath;
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