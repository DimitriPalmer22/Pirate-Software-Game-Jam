using System;
using UnityEngine;
using static System.String;

public class ManekiManager : MonoBehaviour, IInteractable
{
    private ManekiMode _currentMode = ManekiMode.None;
    
    public bool IsInteractable => _currentMode != ManekiMode.None;
    
    public GameObject GameObject => gameObject;
    
    private bool _hasInteracted;

    private void Update()
    {
        // Update the maneki mode
        UpdateManekiMode();
    }

    private void UpdateManekiMode()
    {
        
        // Set the mode back to none
        _currentMode = ManekiMode.None;
        
        if (!WaveManager.Instance.HasStartedGame)
            _currentMode = ManekiMode.Start;
    }

    public void Interact(PlayerInteraction playerInteraction)
    {
        switch (_currentMode)
        {
            case ManekiMode.None:
                break;
            
            case ManekiMode.Start:
                PowerPicker.Instance.Activate();
                WaveManager.Instance.StartGame();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // Set the mode back to none
        _currentMode = ManekiMode.None;
    }

    public string InteractText(PlayerInteraction playerInteraction)
    {
        switch (_currentMode)
        {
            case ManekiMode.Start:
                return "E to Start Game";
                break;

            case ManekiMode.None:
            default:
                return Empty;
        }
    }

    private enum ManekiMode
    {
        None,
        Start,
    }
    
}