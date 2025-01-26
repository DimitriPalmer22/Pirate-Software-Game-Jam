using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerButton : MonoBehaviour
{
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private WeaponScriptableObject weapon;
    
    private PowerPicker _powerPicker;
    
    public void SetData(PowerPicker powerPicker, WeaponScriptableObject weaponInfo)
    {
        _powerPicker = powerPicker;
        weapon = weaponInfo;
        weaponNameText.text = weaponInfo.DisplayName;
        weaponImage.sprite = weaponInfo.Icon;
    }

    public void AddWeaponToPlayer()
    {
        Player.Instance.PlayerWeaponManager.AddWeapon(weapon.WeaponPrefab);
    }

    public void ResumeGame()
    {
        _powerPicker.Deactivate();
    }
}