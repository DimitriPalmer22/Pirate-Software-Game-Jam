using System;
using UnityEngine;

[Serializable]
public struct WeaponUpgradeInfo
{
    [SerializeField] private string upgradeName;
    [SerializeField, Min(0)] private int maxStack;
    [SerializeField, Min(0)] private int weight;
    [SerializeField] private WeaponUpgradeRarity rarity;
    
    public string UpgradeName => upgradeName;
    
    public int MaxStack => maxStack;
    public int Weight => weight;
    
    public WeaponUpgradeRarity Rarity => rarity;
}