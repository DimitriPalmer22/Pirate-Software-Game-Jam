using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon SO", menuName = "Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    private static readonly HashSet<WeaponScriptableObject> allWeapons = new();
    public static IReadOnlyCollection<WeaponScriptableObject> AllWeapons => allWeapons;

    #region Serialized Fields

    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private PlayerWeapon weaponPrefab;

    [Header("Upgrades"), SerializeField, Min(1)]
    private int upgrade1Stack = 1;

    [SerializeField, Min(1)] private int upgrade2Stack = 1;
    [SerializeField, Min(1)] private int upgrade3Stack = 1;
    [SerializeField, Min(1)] private int upgrade4Stack = 1;
    [SerializeField, Min(1)] private int upgrade5Stack = 1;

    #endregion

    #region Getters

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public WeaponType WeaponType => weaponType;
    public PlayerWeapon WeaponPrefab => weaponPrefab;

    public int Upgrade1Stack => upgrade1Stack;
    public int Upgrade2Stack => upgrade2Stack;
    public int Upgrade3Stack => upgrade3Stack;
    public int Upgrade4Stack => upgrade4Stack;
    public int Upgrade5Stack => upgrade5Stack;

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