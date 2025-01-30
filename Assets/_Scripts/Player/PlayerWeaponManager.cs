using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Player))]
public class PlayerWeaponManager : ComponentScript<Player>
{
    private static readonly int NormalFiringAnimationID = Animator.StringToHash("NormalFiring");

    #region Serialized Fields

    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator animatorForShooting;

    [SerializeField] private List<PlayerWeapon> weaponPrefabs;

    [Header("Aim Snapping"), SerializeField, Min(0)]
    private float aimSnapAngle = 30;

    [SerializeField, Min(0)] private float aimSnapRange;

    #endregion

    #region Private Fields

    private readonly Dictionary<PlayerWeapon, PlayerWeapon> _prefabInstances = new();

    #endregion

    #region Getters

    public Transform FirePoint => firePoint;

    public IReadOnlyCollection<PlayerWeapon> WeaponPrefabs => weaponPrefabs;

    public bool HasAllUpgrades => weaponPrefabs.TrueForAll(weapon => weapon.HasAllUpgrades);

    public Vector3 AimForward { get; private set; }

    #endregion

    #region Initialization Functions

    protected override void CustomAwake()
    {
    }

    private void Start()
    {
        // Initialize the input
        InitializeInput();
    }

    private void InitializeInput()
    {
        // Shoot input
        ParentComponent.PlayerControls.Player.Shoot.performed += OnShootPerformed;
        ParentComponent.PlayerControls.Player.Shoot.canceled += OnShootCanceled;
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

    private void Update()
    {
        // Update the aim forward
        UpdateAimForward();
    }

    private void UpdateAimForward()
    {
        var aimCandidates = new HashSet<Transform>();

        // For each enemy,
        foreach (var enemy in Enemy.AllEnemies)
        {
            // Continue if the enemy is null
            if (enemy == null)
                continue;

            // Get the vector between the player and the enemy
            var vectorToEnemy = enemy.transform.position - transform.position;

            // If the distance is greater than the aim snap range, continue
            if (vectorToEnemy.magnitude > aimSnapRange)
                continue;

            // Get the angle between the player forward and the vector to the enemy
            var angle = Vector3.Angle(ParentComponent.Rigidbody.transform.forward, vectorToEnemy);

            // If the angle is greater than the aim snap angle, continue
            if (angle > aimSnapAngle)
                continue;

            // Add the enemy to the aim candidates
            aimCandidates.Add(enemy.transform);
        }

        // If there are no aim candidates, set the aim forward to the player forward
        if (aimCandidates.Count == 0)
        {
            AimForward = ParentComponent.Rigidbody.transform.forward;
            return;
        }

        // Sort the aim candidates by dot product
        var candidate = aimCandidates.OrderBy(n => Vector3.Dot(
            (n.position - transform.position).normalized,
            ParentComponent.Rigidbody.transform.forward)
        ).First();
        
        // Set the aim forward to the candidate forward
        AimForward = (candidate.position - transform.position).normalized;
    }

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
    
    private void OnDrawGizmos()
    {
        // Draw a line representing the weapon's aim forward
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, AimForward * 10);
        
        // Draw 2 lines representing the aim snap range
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, aimSnapAngle, 0) * transform.forward * aimSnapRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -aimSnapAngle, 0) * transform.forward * aimSnapRange);
    }
}