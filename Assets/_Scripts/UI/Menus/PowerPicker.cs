using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerPicker : GameMenu
{
    public static PowerPicker Instance { get; private set; }

    [SerializeField, Min(0)] private int maxWeapons = 3;
    [SerializeField] private PowerButton powerButtonPrefab;
    [SerializeField] private GameObject powerButtonParent;

    protected override void CustomAwake()
    {
        Instance = this;
    }

    protected override void CustomDestroy()
    {
    }

    protected override void CustomActivate()
    {
        // Create the power buttons
        var weaponsCreated = CreateWeaponButtons();
        
        // If no weapons were created, deactivate the menu
        if (!weaponsCreated)
            ResumeGame();
    }

    protected override void CustomStart()
    {
    }

    private bool CreateWeaponButtons()
    {
        // Clear all the children of the powerButtonParent
        foreach (Transform child in powerButtonParent.transform)
            Destroy(child.gameObject);

        // Get ALL the possible weapons
        var allWeapons = new HashSet<WeaponType>(WeaponScriptableObject.AllWeapons.Select(n => n.WeaponType));

        // Get the current weapons
        var currentWeapons =
            new HashSet<WeaponType>(Player.Instance.PlayerWeaponManager.WeaponPrefabs.Select(n => n.WeaponType));

        // For each current weapon, remove it from the allWeapons
        foreach (var currentWeapon in currentWeapons)
            allWeapons.Remove(currentWeapon);

        var weaponCount = 0;
        
        // Create power buttons for each weapon up to maxWeapons (and the remaining current weapons)
        for (var i = 0; i < maxWeapons && allWeapons.Count > 0; i++)
        {
            // Get a random weapon from the allWeapons
            var randomWeapon = allWeapons.ElementAt(Random.Range(0, allWeapons.Count));

            // Remove it from the allWeapons
            allWeapons.Remove(randomWeapon);

            // Create a power button for it
            var powerButton = Instantiate(powerButtonPrefab, powerButtonParent.transform);

            // Set the data for the power button
            powerButton.SetData(this, WeaponScriptableObject.GetWeaponScriptableObject(randomWeapon));
            
            weaponCount++;
        }
        
        return weaponCount > 0;
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

    public void ResumeGame()
    {
        Deactivate();
    }
}