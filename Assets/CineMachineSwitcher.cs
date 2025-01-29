using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class CineMachineSwitcher : MonoBehaviour
{
    private Animator _animator;

    [Header("References")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayableDirector cutscenePlayer;
    [SerializeField] private AudioSource MenuMusic;
    [SerializeField] private AudioSource PlayerMusic;
        
    [SerializeField] private Button _hideBackButton;
    [SerializeField] private GameObject _backButton;
    [SerializeField] private Button[] buttonsToHide;
    
    [SerializeField] private GameObject[] buttonGameObjects;

    // Tracks the current camera state
    private CameraState _currentCameraState = CameraState.Menu;

    // Define the camera states
    private enum CameraState
    {
        Menu,
        Player,
        Credits
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // Optionally ensure we start in Menu state
        SetCameraState(CameraState.Menu);
    }

    // Centralized camera state logic
    private void SetCameraState(CameraState newState)
    {
        switch (newState)
        {
            case CameraState.Menu:
                _animator.Play("MenuCam");
                ShowButtons();
                _backButton.gameObject.SetActive(false);
                break;

            case CameraState.Player:
                _animator.Play("PlayerCam");
                HideButtons();
                playerAnimator.SetLayerWeight(2, 0);
                cutscenePlayer.Play();
                MenuMusic.Stop();
                
                
                _backButton.gameObject.SetActive(false);
                break;

            case CameraState.Credits:
                _animator.Play("CreditsCam");
                HideButtons();
                _backButton.gameObject.SetActive(true);
                break;
        }

        _currentCameraState = newState;
    }

    // Show / hide UI buttons
    private void HideButtons()
    {
        foreach (var button in buttonsToHide)
        {
            if (button != null)
            {
                button.enabled = false;
            }
        }

        foreach (var go in buttonGameObjects)
        {
            if (go != null)
            {
                go.SetActive(false);
            }
        }
    }

    private void ShowButtons()
    {
        foreach (var button in buttonsToHide)
        {
            if (button != null)
            {
                button.enabled = true;
            }
        }

        foreach (var go in buttonGameObjects)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }
    }

    #region Public Methods (Menu Input)

    public void SwitchToPlayer()
    {
        if (_currentCameraState == CameraState.Menu)
            SetCameraState(CameraState.Player);
        else
            SetCameraState(CameraState.Menu);
    }

    public void SwitchToCredits()
    {
        if (_currentCameraState == CameraState.Menu)
            SetCameraState(CameraState.Credits);
        else
            SetCameraState(CameraState.Menu);
    }

    public void SwitchToMenu()
    {
        SetCameraState(CameraState.Menu);
    }

    #endregion
}
