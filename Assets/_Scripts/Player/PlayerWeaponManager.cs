using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerWeaponManager : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerWeapon bulletPrefab;
    [SerializeField] private float fireRate = 0.5f;

    #endregion
    
    #region Private Fields

    private bool _isShooting;
    
    private float _fireRateTimer;
    
    #endregion

    #region Getters

    public Player Player { get; private set; }

    public Transform FirePoint => firePoint;

    public PlayerWeapon BulletPrefab => bulletPrefab;

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

    private void OnShootCanceled(InputAction.CallbackContext obj)
    {
        _isShooting = false;
    }

    private void OnShootPerformed(InputAction.CallbackContext obj)
    {
        _isShooting = true;
    }

    #endregion

    private void Update()
    {
        // Update the fire rate timer
        UpdateFireRateTimer();
        
        // Shoot if the player is shooting
        while (_isShooting && _fireRateTimer >= fireRate)
            Shoot();
    }

    private void UpdateFireRateTimer()
    {
        if (!_isShooting)
            _fireRateTimer = Mathf.Clamp(_fireRateTimer + Time.deltaTime, 0, fireRate);
        else
            _fireRateTimer += Time.deltaTime;
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Shoot the bullet
        bullet.Shoot(this);
        
        // Reset the fire rate timer
        _fireRateTimer -= fireRate;
    }
    
}