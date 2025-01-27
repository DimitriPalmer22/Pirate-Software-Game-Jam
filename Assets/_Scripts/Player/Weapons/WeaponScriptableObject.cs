using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon SO", menuName = "Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public const int UPGRADES_COUNT = 5;
    
    private static readonly HashSet<WeaponScriptableObject> allWeapons = new();
    public static IReadOnlyCollection<WeaponScriptableObject> AllWeapons => allWeapons;

    #region Serialized Fields

    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private PlayerWeapon weaponPrefab;

    [Header("Upgrades")]
    [SerializeField] private WeaponUpgradeInfo[] upgrades = new WeaponUpgradeInfo[UPGRADES_COUNT];

    #endregion

    #region Getters

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public WeaponType WeaponType => weaponType;
    public PlayerWeapon WeaponPrefab => weaponPrefab;

    public int Upgrade1Stack => upgrades[0].MaxStack;
    public int Upgrade2Stack => upgrades[1].MaxStack;
    public int Upgrade3Stack => upgrades[2].MaxStack;
    public int Upgrade4Stack => upgrades[3].MaxStack;
    public int Upgrade5Stack => upgrades[4].MaxStack;

    #endregion

    public WeaponScriptableObject()
    {
        // Add this weapon to the static collection
        allWeapons.Add(this);
    }

    public static PlayerWeapon GetWeaponPrefab(WeaponType weaponType)
    {
        return AllWeapons.FirstOrDefault(weapon => weapon.weaponType == weaponType)?.weaponPrefab;
    }

    public static WeaponScriptableObject GetWeaponScriptableObject(WeaponType weaponType)
    {
        return AllWeapons.FirstOrDefault(weapon => weapon.weaponType == weaponType);
    }
}