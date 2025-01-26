using System;
using UnityEngine.InputSystem;

public class PauseMenu : GameMenu
{
    private PlayerControls _playerControls;

    
    protected override void CustomAwake()
    {
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

    private void Start()
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
        // If the menu is active, deactivate it
        if (IsActive)
            Deactivate();

        // If the menu is not active, activate it
        else
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

    public void Resume()
    {
        // Deactivate the menu
        Deactivate();
    }
}