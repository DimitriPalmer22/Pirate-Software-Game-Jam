using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Update()
    {
        // Update the slider
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        // Return if the player instance is null
        if (Player.Instance == null)
            return;
        
        // Set the slider's value to the health percentage
        slider.value = Player.Instance.CurrentHealth / Player.Instance.MaxHealth;
        slider.minValue = 0;
        slider.maxValue = 1;
    }
}