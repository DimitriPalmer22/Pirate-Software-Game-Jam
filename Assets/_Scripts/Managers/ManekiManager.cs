using System;
using UnityEngine;
using UnityEngine.Events;
using static System.String;

public class ManekiManager : MonoBehaviour, IInteractable
{
    public static ManekiManager Instance { get; private set; }

    #region Serialized Fields

    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField, Min(0)] private float notificationLerpAmount = 0.20f;
    [SerializeField] private GameObject manekiObject;

    [SerializeField] private UnityEvent onInteract;
    
    #endregion

    #region Private Fields

    private ManekiMode _currentMode = ManekiMode.None;
    private bool _hasInteracted;

    #endregion

    #region Getters

    public bool IsInteractable => _currentMode != ManekiMode.None;

    public GameObject GameObject => gameObject;
    
    public GameObject ManekiObject => manekiObject;

    #endregion


    private void Awake()
    {
        // Set the instance to this
        Instance = this;
    }

    private void Start()
    {
        notificationCanvasGroup.alpha = 0;

        // Update the notification
        UpdateNotification();
    }

    private void Update()
    {
        // Update the maneki mode
        UpdateManekiMode();

        // Update the notification
        UpdateNotification();
    }

    private void UpdateManekiMode()
    {
        // Set the mode back to none
        _currentMode = ManekiMode.None;

        if (!WaveManager.Instance.HasStartedGame)
            _currentMode = ManekiMode.Start;
        else if (WaveManager.Instance.HasStartedGame && WaveManager.Instance.IsWaitingForNextWave)
            _currentMode = ManekiMode.ChoosePower;
    }

    private void UpdateNotification()
    {
        var desiredAlpha = _currentMode == ManekiMode.None ? 0 : 1;

        // Set the notification game object active based on the current mode
        notificationCanvasGroup.alpha = Mathf.Lerp(
            notificationCanvasGroup.alpha,
            desiredAlpha,
            CustomFunctions.FrameAmount(notificationLerpAmount)
        );
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

            case ManekiMode.ChoosePower:
                PowerPicker.Instance.Activate();
                WaveManager.Instance.ResumeGame();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        // Set the mode back to none
        _currentMode = ManekiMode.None;
        
        // Invoke the on interact event
        onInteract.Invoke();
    }

    public string InteractText(PlayerInteraction playerInteraction)
    {
        switch (_currentMode)
        {
            case ManekiMode.Start:
                return "E to Start Game";

            case ManekiMode.ChoosePower:
                return "E to Choose Power & Start Next Wave";

            case ManekiMode.None:
            default:
                return Empty;
        }
    }

    private enum ManekiMode
    {
        None,
        Start,
        ChoosePower
    }
}