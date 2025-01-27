using System.Collections.Generic;
using UnityEngine;

public class UpgradePicker : GameMenu
{
    public static UpgradePicker Instance { get; private set; }
    
    [SerializeField, Min(0)] private int maxUpgrades = 3;
    [SerializeField] private UpgradeButton upgradeButtonPrefab;
    [SerializeField] private GameObject upgradeButtonParent;

    protected override void CustomAwake()
    {
        // Set the instance
        Instance = this;
    }
    
    private void Start()
    {
    }

    private void CreateWeaponButtons()
    {
        // Clear all the children of the upgradeButtonParent
        foreach (Transform child in upgradeButtonParent.transform)
            Destroy(child.gameObject);

        // Create a list for storing all possible upgrades
        var allUpgrades = new List<WeaponUpgradeToken>();

        var playerWeaponManager = Player.Instance.PlayerWeaponManager;

        // Add all the upgrades from the player's weapons to the allUpgrades list
        foreach (var weaponPrefab in playerWeaponManager.WeaponPrefabs)
        {
            // Get the weapon from the prefab
            var weapon = playerWeaponManager.GetWeapon(weaponPrefab);

            // Create the upgrade tokens for the weapon
            // Add the tokens to the allUpgrades list
            allUpgrades.AddRange(weapon.GetUpgradeTokens());
        }

        //  Create upgrade buttons for each upgrade up to maxUpgrades (and the remaining current upgrades)
        for (var i = 0; i < maxUpgrades && allUpgrades.Count > 0; i++)
        {
            // Get a random upgrade from the allUpgrades
            var randomUpgrade = WeaponUpgradeToken.ChooseRandomUpgradeToken(allUpgrades);

            // Remove it from the allUpgrades
            allUpgrades.Remove(randomUpgrade);

            // Create an upgrade button for it
            var upgradeButton = Instantiate(upgradeButtonPrefab, upgradeButtonParent.transform);

            // Set the data for the upgrade button
            upgradeButton.SetData(this, randomUpgrade);
        }
    }

    protected override void CustomDestroy()
    {
    }

    protected override void CustomActivate()
    {
        // Create the power buttons
        CreateWeaponButtons();
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
}