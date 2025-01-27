using System;
using UnityEngine;

[Serializable]
public struct WeaponUpgradeInfo
{
    [SerializeField, Min(1)] private int maxStack;
    [SerializeField, Min(1)] private int weight;
    
    public int MaxStack => maxStack;
    public int Weight => weight;
}