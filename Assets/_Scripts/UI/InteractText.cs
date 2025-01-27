using System;
using TMPro;
using UnityEngine;

public class InteractText : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text interactText;
    [SerializeField, Range(0, 1)] private float maxOpacity = 1;
    [SerializeField, Min(0)] private float opacityLerpAmount = .2f;

    #endregion

    #region Private Fields

    private float _desiredOpacity;

    #endregion

    private void Awake()
    {
        _desiredOpacity = 0;
        canvasGroup.alpha = 0;
    }

    private void Start()
    {
    }

    private void Update()
    {
        // Update the interact text
        UpdateInteractText();
        
        // Update the opacity
        UpdateOpacity();
    }

    private void UpdateInteractText()
    {
        var playerInteraction = Player.Instance?.PlayerInteraction;

        // If the player interaction is null, return
        if (playerInteraction == null)
        {
            _desiredOpacity = 0;
            interactText.text = "";
            return;
        }

        // If there is no current interactable, return
        if (playerInteraction.CurrentInteractable == null)
        {
            _desiredOpacity = 0;
            interactText.text = "";
            return;
        }

        // Set the interact text
        interactText.text = playerInteraction.CurrentInteractable.InteractText(playerInteraction);

        // Set the desired opacity
        _desiredOpacity = maxOpacity;
    }

    private void UpdateOpacity()
    {
        var frameAmount = CustomFunctions.DEFAULT_FRAME_AMOUNT / Time.deltaTime;
        
        // Lerp the alpha
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _desiredOpacity, opacityLerpAmount * frameAmount);  
    }
}