using System;
using UnityEngine;

public class WeaponHelper : MonoBehaviour
{
    [SerializeField] private WeaponScriptableObject[] weapons;

    private void Awake()
    {
        // Initialize all the powers
        foreach (var weapon in weapons)
            weapon.Initialize();
    }
}