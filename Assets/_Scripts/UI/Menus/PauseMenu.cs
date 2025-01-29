using System;
using UnityEngine.InputSystem;

public class PauseMenu : GameMenu
{
    public static PauseMenu Instance { get; private set; }

    public Action onPause;
    public Action onResume;

    private PlayerControls _playerControls;

    protected override void CustomAwake()
    {
        // Set the instance
        Instance = this;

        // Initialize the input
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    protected override void CustomStart()
    {
        // Initialize the input
        InitializeInput();
    }

    private void InitializeInput()
    {
        _playerControls.Other.Pause.performed += OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext obj)
    {
        // If there is another menu active in the menu manager, deactivate it
        if (MenuManager.Instance.ActiveMenus.Count > 0)
            return;

        // If the menu is active, deactivate it
        if (IsActive)
            Resume();

        // If the menu is not active, activate it
        else
            Activate();
    }

    protected override void CustomDestroy()
    {
    }

    protected override void CustomActivate()
    {
        // Invoke the pause event
        onPause?.Invoke();
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

    public void Resume()
    {
        // Deactivate the menu
        Deactivate();

        // Invoke the resume event
        onResume?.Invoke();
    }
}