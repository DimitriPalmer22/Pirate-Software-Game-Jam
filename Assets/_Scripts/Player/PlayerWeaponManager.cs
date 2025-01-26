using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerWeaponManager : MonoBehaviour
{
    private static readonly int NormalFiringAnimationID = Animator.StringToHash("NormalFiring");

    #region Serialized Fields

    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator animatorForShooting;

    [SerializeField] private List<PlayerWeapon> weapons;

    #endregion

    #region Private Fields

    private readonly Dictionary<PlayerWeapon, PlayerWeapon> _prefabInstances = new();

    #endregion

    #region Getters

    public Player Player { get; private set; }

    public Transform FirePoint => firePoint;

    public IReadOnlyCollection<PlayerWeapon> Weapons => weapons;

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
        foreach (var weapon in weapons)
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
        foreach (var weapon in weapons.Where(weapon => _prefabInstances.ContainsKey(weapon)))
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
        if (weapons.Contains(weapon))
            return;
        
        weapons.Add(weapon);
    }
}