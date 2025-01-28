using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : ComponentScript<Player>
{
    #region Serialized Fields

    [SerializeField, Min(0)] private float interactionRange = 5f;

    #endregion

    #region Private Fields

    private IInteractable _currentInteractable;

    #endregion

    #region Getters

    public IInteractable CurrentInteractable => _currentInteractable;

    #endregion

    protected override void CustomAwake()
    {
    }

    private void Start()
    {
        // Initialize the input
        InitializeInput();
    }

    private void InitializeInput()
    {
        ParentComponent.PlayerControls.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnInteractPerformed(InputAction.CallbackContext obj)
    {
        // If the current interactable is null, return
        if (_currentInteractable == null)
            return;

        // Interact with the current interactable
        _currentInteractable.Interact(this);
    }

    private void Update()
    {
        // Check for interactables
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        // Get through all the interactables in the scene
        var interactables = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>();

        // Get the closest interactable
        var closestInteractable = interactables
            .Where(interactable => interactable.IsInteractable &&
                                   Vector3.Distance(transform.position, interactable.GameObject.transform.position) <=
                                   interactionRange)
            .OrderBy(interactable => Vector3.Distance(transform.position, interactable.GameObject.transform.position))
            .FirstOrDefault();

        // If the closest interactable is null, return
        _currentInteractable = closestInteractable;
    }
}