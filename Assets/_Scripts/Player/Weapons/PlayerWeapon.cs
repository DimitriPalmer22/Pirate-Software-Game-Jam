using System;
using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour, IDamager
{
    #region Serialized Fields

    [SerializeField] private WeaponType weaponType;

    [SerializeField] protected LayerMask layersToIgnore;

    [SerializeField, Min(0)] protected float baseDamage = 10;
    [SerializeField, Min(0)] protected float fireRate = 0.125f;
    [SerializeField, Min(0)] protected float range = 20f;

    #endregion

    #region Protected & Private Fields

    private bool _isShooting;
    private float _fireRateTimer;

    #endregion

    #region Getters

    protected PlayerWeaponManager PlayerWeaponManager { get; private set; }

    public GameObject GameObject => gameObject;

    public WeaponType WeaponType => weaponType;

    #endregion

    private void Awake()
    {
        // Initialize the fire rate timer
        _fireRateTimer = fireRate;

        // Custom Awake function
        CustomAwake();
    }

    protected abstract void CustomAwake();

    private void Update()
    {
        // Update the fire rate timer
        UpdateFireRateTimer();

        // Run the custom update function
        CustomUpdate();

        // Shoot if the fire rate timer is ready
        Shoot(PlayerWeaponManager);
    }

    protected abstract void CustomUpdate();

    private void UpdateFireRateTimer()
    {
        if (!_isShooting)
            _fireRateTimer = Mathf.Clamp(_fireRateTimer + Time.deltaTime, 0, fireRate);
        else
            _fireRateTimer += Time.deltaTime;
    }

    private void Shoot(PlayerWeaponManager playerWeaponManager)
    {
        // Return if the fire rate timer is not ready
        if (_fireRateTimer < fireRate || !_isShooting)
            return;

        // Call the custom shoot function
        CustomShoot(playerWeaponManager);

        // Reset the fire rate timer
        _fireRateTimer -= fireRate;

        // Recurse if the fire rate timer is still ready
        if (_fireRateTimer >= fireRate && fireRate > 0)
            Shoot(playerWeaponManager);
    }

    protected abstract void CustomShoot(PlayerWeaponManager playerWeaponManager);

    public void StartShooting(PlayerWeaponManager playerWeaponManager)
    {
        PlayerWeaponManager = playerWeaponManager;

        // Set the shooting flag
        _isShooting = true;

        CustomStartShooting(playerWeaponManager);
    }

    protected abstract void CustomStartShooting(PlayerWeaponManager playerWeaponManager);

    public void StopShooting(PlayerWeaponManager playerWeaponManager)
    {
        // Reset the shooting flag
        _isShooting = false;

        CustomStopShooting(playerWeaponManager);
    }

    protected abstract void CustomStopShooting(PlayerWeaponManager playerWeaponManager);
}