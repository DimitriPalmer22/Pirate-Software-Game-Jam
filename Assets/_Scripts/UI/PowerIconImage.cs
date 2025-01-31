using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PowerIconImage : MonoBehaviour
{
    [SerializeField] private WeaponScriptableObject weapon;
    
    private Image _image;
    
    private void Awake()
    {
        // Get the Image component
        _image = GetComponent<Image>();
    }

    public void SetIcon(WeaponScriptableObject weaponSo)
    {
        // Set the weapon
        weapon = weaponSo;
        
        // Set the icon of the Image component
        _image.sprite = weapon.Icon;
    }
}