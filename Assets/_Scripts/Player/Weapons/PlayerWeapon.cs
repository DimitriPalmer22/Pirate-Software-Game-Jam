using System;
using System.Collections.Generic;
using System.Linq;
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

    private readonly int[] _upgradeCounts = new int[WeaponScriptableObject.UPGRADES_COUNT];
    private readonly Action[] _upgradeFunctions = new Action[WeaponScriptableObject.UPGRADES_COUNT];

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

    public bool HasAllUpgrades =>
        WeaponScriptableObject != null &&
        Enumerable
            .Range(0, WeaponScriptableObject.UPGRADES_COUNT)
            .All(i => _upgradeCounts[i] >= WeaponScriptableObject.Upgrades[i].MaxStack);

    #endregion

    private void Awake()
    {
        // Initialize the fire rate timer
        _fireRateTimer = fireRate;

        // Custom Awake function
        CustomAwake();

        // Map the upgrade functions
        MapUpgradeFunctions();
    }

    private void MapUpgradeFunctions()
    {
        MapUpgradeFunction(0, CustomUpgrade1);
        MapUpgradeFunction(1, CustomUpgrade2);
        MapUpgradeFunction(2, CustomUpgrade3);
        MapUpgradeFunction(3, CustomUpgrade4);
        MapUpgradeFunction(4, CustomUpgrade5);
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

    public void Upgrade(int index)
    {
        if (_upgradeCounts[index] >= WeaponScriptableObject.Upgrades[index].MaxStack)
        {
            Debug.Log(
                $"Returning!: ({index}) {_upgradeCounts[index]}, {WeaponScriptableObject.Upgrades[index].MaxStack}");
            return;
        }

        // Set the upgrade flags
        _upgradeCounts[index]++;

        // Run the upgrade function
        _upgradeFunctions[index]?.Invoke();
    }


    protected abstract void CustomUpgrade1();
    protected abstract void CustomUpgrade2();
    protected abstract void CustomUpgrade3();
    protected abstract void CustomUpgrade4();
    protected abstract void CustomUpgrade5();

    public WeaponUpgradeToken[] GetUpgradeTokens()
    {
        var upgradeTokens = new List<WeaponUpgradeToken>();

        // For each upgrade
        for (var upgradeIndex = 0; upgradeIndex < WeaponScriptableObject.UPGRADES_COUNT; upgradeIndex++)
        {
            for (
                var remainingStacks = _upgradeCounts[upgradeIndex];
                remainingStacks < WeaponScriptableObject.Upgrades[upgradeIndex].MaxStack;
                remainingStacks++
            )
            {
                var weight = WeaponScriptableObject.Upgrades[upgradeIndex].Weight;

                // Create an upgrade token
                var upgradeToken = new WeaponUpgradeToken(WeaponScriptableObject, weight, upgradeIndex);

                // Add the upgrade token to the list
                upgradeTokens.Add(upgradeToken);
            }
        }

        // Return the upgrade tokens
        return upgradeTokens.ToArray();
    }

    private void MapUpgradeFunction(int index, Action action)
    {
        _upgradeFunctions[index] = action;
    }

    #endregion
}