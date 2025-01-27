using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Player))]
public class PlayerWeaponManager : MonoBehaviour
{
    private static readonly int NormalFiringAnimationID = Animator.StringToHash("NormalFiring");

    #region Serialized Fields

    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator animatorForShooting;

    [SerializeField] private List<PlayerWeapon> weaponPrefabs;

    #endregion

    #region Private Fields

    private readonly Dictionary<PlayerWeapon, PlayerWeapon> _prefabInstances = new();

    #endregion

    #region Getters

    public Player Player { get; private set; }

    public Transform FirePoint => firePoint;

    public IReadOnlyCollection<PlayerWeapon> WeaponPrefabs => weaponPrefabs;
    
    public bool HasAllUpgrades => weaponPrefabs.All(weapon => weapon.HasAllUpgrades);

    #endregion

    #region Initialization Functions

    private void Awake()
    {
        // Initialize the components
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // Get the player component
        Player = GetComponent<Player>();
    }

    private void Start()
    {
        // Initialize the input
        InitializeInput();
    }

    private void InitializeInput()
    {
        // Shoot input
        Player.PlayerControls.Player.Shoot.performed += OnShootPerformed;
        Player.PlayerControls.Player.Shoot.canceled += OnShootCanceled;
    }

    private void OnShootPerformed(InputAction.CallbackContext obj)
    {
        // Start shooting
        StartShooting();

        // start the firing animation
        animatorForShooting?.SetBool(NormalFiringAnimationID, true);
    }

    private void OnShootCanceled(InputAction.CallbackContext obj)
    {
        // Stop shooting
        StopShooting();

        // stop the firing animation
        animatorForShooting?.SetBool(NormalFiringAnimationID, false);
    }

    #endregion

    private void StartShooting()
    {
        foreach (var weapon in weaponPrefabs)
        {
            // Check if the weapon is already instantiated
            // Instantiate the weapon prefab
            if (!_prefabInstances.ContainsKey(weapon))
                _prefabInstances.Add(weapon, Instantiate(weapon, firePoint.transform));

            // Get the weapon instance
            var weaponInstance = _prefabInstances[weapon];

            // Shoot the bullet
            weaponInstance.StartShooting(this);
        }
    }

    private void StopShooting()
    {
        foreach (var weapon in weaponPrefabs.Where(weapon => _prefabInstances.ContainsKey(weapon)))
        {
            // Get the weapon instance
            var weaponInstance = _prefabInstances[weapon];

            // Stop the bullet
            weaponInstance.StopShooting(this);
        }
    }

    public void AddWeapon(PlayerWeapon weapon)
    {
        // Check if the weapon is already in the list
        if (weaponPrefabs.Contains(weapon))
            return;
        
        weaponPrefabs.Add(weapon);
    }

    public PlayerWeapon GetWeapon(PlayerWeapon weaponPrefab)
    {
        // Check if the weapon is already instantiated
        if (_prefabInstances.ContainsKey(weaponPrefab))
            return _prefabInstances[weaponPrefab];

        // Instantiate the weapon prefab
        var weapon = Instantiate(weaponPrefab, firePoint.transform);

        // Add the weapon to the list
        _prefabInstances.Add(weaponPrefab, weapon);

        return weapon;
    }
}