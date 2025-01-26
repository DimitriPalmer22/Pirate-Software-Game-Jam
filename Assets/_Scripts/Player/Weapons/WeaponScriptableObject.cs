using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon SO", menuName = "Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    private static readonly HashSet<WeaponScriptableObject> allWeapons = new();
    public static IReadOnlyCollection<WeaponScriptableObject> AllWeapons => allWeapons;

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private PlayerWeapon weaponPrefab;

    public WeaponScriptableObject()
    {
        // Add this weapon to the static collection
        allWeapons.Add(this);
    }

    public static PlayerWeapon GetWeaponPrefab(WeaponType weaponType)
    {
        return AllWeapons.FirstOrDefault(weapon => weapon.weaponType == weaponType)?.weaponPrefab;
    }
}