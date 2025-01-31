using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerIconImageManager : MonoBehaviour
{
    [SerializeField] private PowerIconImage powerIconImagePrefab;

    private void Start()
    {
        // Connect to the player's weapon manager's weapon event
        Player.Instance.PlayerWeaponManager.onWeaponAdded += OnWeaponAdded;
    }

    private void OnWeaponAdded(WeaponScriptableObject obj)
    {
        // Instantiate the power icon image prefab
        var powerIconImage = Instantiate(powerIconImagePrefab, transform);

        // Set the icon of the power icon image
        powerIconImage.SetIcon(obj);
    }
}