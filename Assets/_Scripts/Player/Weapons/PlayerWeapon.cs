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

    private WeaponScriptableObject _weaponSo;

    private WeaponScriptableObject WeaponScriptableObject
    {
        get
        {
            if (_weaponSo == null)
                _weaponSo = WeaponScriptableObject.GetWeaponScriptableObject(weaponType);

            return _weaponSo;
        }
    }
    
    public int Upgrade1Count { get; private set; }
    public int Upgrade2Count { get; private set; }
    public int Upgrade3Count { get; private set; }
    public int Upgrade4Count { get; private set; }
    public int Upgrade5Count { get; private set; }
    
    public bool HasAllUpgrades =>
        WeaponScriptableObject != null &&
        Upgrade1Count >= WeaponScriptableObject.Upgrade1Stack &&
        Upgrade2Count >= WeaponScriptableObject.Upgrade2Stack &&
        Upgrade3Count >= WeaponScriptableObject.Upgrade3Stack &&
        Upgrade4Count >= WeaponScriptableObject.Upgrade4Stack &&
        Upgrade5Count >= WeaponScriptableObject.Upgrade5Stack;

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

    #region Upgrades

    public void Upgrade1()
    {
        if (Upgrade1Count >= WeaponScriptableObject.Upgrade1Stack)
        {
            Debug.Log($"Returning!: {Upgrade1Count}, {WeaponScriptableObject.Upgrade1Stack}");
            
            return;
        }

        // Set the upgrade flags
        Upgrade1Count++;

        CustomUpgrade1();
    }

    protected abstract void CustomUpgrade1();

    public void Upgrade2()
    {
        if (Upgrade2Count >= WeaponScriptableObject.Upgrade2Stack)
            return;

        // Set the upgrade flags
        Upgrade2Count++;

        CustomUpgrade2();
    }

    protected abstract void CustomUpgrade2();


    public void Upgrade3()
    {
        if (Upgrade3Count >= WeaponScriptableObject.Upgrade3Stack)
            return;

        // Set the upgrade flags
        Upgrade3Count++;

        CustomUpgrade3();
    }

    protected abstract void CustomUpgrade3();


    public void Upgrade4()
    {
        if (Upgrade4Count >= WeaponScriptableObject.Upgrade4Stack)
            return;

        // Set the upgrade flags
        Upgrade4Count++;

        CustomUpgrade4();
    }

    protected abstract void CustomUpgrade4();


    public void Upgrade5()
    {
        if (Upgrade5Count >= WeaponScriptableObject.Upgrade5Stack)
            return;

        // Set the upgrade flags
        Upgrade5Count++;

        CustomUpgrade5();
    }

    protected abstract void CustomUpgrade5();

    #endregion
}