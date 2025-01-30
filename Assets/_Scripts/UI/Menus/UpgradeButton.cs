using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image rarityImage;

    [SerializeField] private Sprite commonImage;
    [SerializeField] private Sprite rareImage;
    [SerializeField] private Sprite legendaryImage;

    [SerializeField] private float commonRotationSpeed = .5f;
    [SerializeField] private float rareRotationSpeed = 1;
    [SerializeField] private float legendaryRotationSpeed = 2;

    private UpgradePicker _upgradePicker;
    private WeaponUpgradeToken _upgradeToken;
    private PlayerWeapon _weapon;
    private WeaponUpgradeRarity _rarity;

    private void Update()
    {
        var rotationSpeed = _rarity switch
        {
            WeaponUpgradeRarity.Common => commonRotationSpeed,
            WeaponUpgradeRarity.Rare => rareRotationSpeed,
            WeaponUpgradeRarity.Legendary => legendaryRotationSpeed,
            _ => commonRotationSpeed
        };

        rarityImage.transform.Rotate(Vector3.forward, rotationSpeed * Mathf.PI * Time.unscaledDeltaTime);
        
        Debug.Log("Update");
    }

    public void SetData(UpgradePicker upgradePicker, WeaponUpgradeToken upgradeToken)
    {
        _upgradePicker = upgradePicker;
        _upgradeToken = upgradeToken;

        var playerWeaponManager = Player.Instance.PlayerWeaponManager;

        // Get the weapon from the prefab
        _weapon = playerWeaponManager.GetWeapon(upgradeToken.WeaponScriptableObject.WeaponPrefab);

        var upgradeInfo = upgradeToken.WeaponScriptableObject.Upgrades[upgradeToken.UpgradeIndex];

        // Set the data for the upgrade button
        upgradeNameText.text = upgradeInfo.UpgradeName;

        // Set the data for the weapon button
        weaponNameText.text = _weapon.WeaponScriptableObject.DisplayName;

        // Set the image for the upgrade button
        weaponImage.sprite = upgradeToken.WeaponScriptableObject.Icon;

        // Set the rarity for the upgrade button
        _rarity = upgradeInfo.Rarity;

        // Set the color for the upgrade button
        rarityImage.sprite = upgradeInfo.Rarity switch
        {
            WeaponUpgradeRarity.Common => commonImage,
            WeaponUpgradeRarity.Rare => rareImage,
            WeaponUpgradeRarity.Legendary => legendaryImage,
            _ => commonImage
        };
    }

    public void UpgradeWeapon()
    {
        _weapon.Upgrade(_upgradeToken.UpgradeIndex);
    }

    public void ResumeGame()
    {
        _upgradePicker.ResumeGame();
    }
}