using System;
using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour, IDamager
{
    [SerializeField] protected LayerMask layersToIgnore;

    [SerializeField, Min(0)] protected float baseDamage = 10;
    [SerializeField, Min(0)] protected float fireRate = 0.125f;
    [SerializeField, Min(0)] protected float range = 20f;

    protected bool isShooting;
    protected float fireRateTimer;

    protected PlayerWeaponManager PlayerWeaponManager { get; private set; }

    public GameObject GameObject => gameObject;

    private void Awake()
    {
        // Initialize the fire rate timer
        fireRateTimer = fireRate;

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
        if (!isShooting)
            fireRateTimer = Mathf.Clamp(fireRateTimer + Time.deltaTime, 0, fireRate);
        else
            fireRateTimer += Time.deltaTime;
    }

    private void Shoot(PlayerWeaponManager playerWeaponManager)
    {
        // Return if the fire rate timer is not ready
        if (fireRateTimer < fireRate || !isShooting)
            return;

        // Call the custom shoot function
        CustomShoot(playerWeaponManager);

        // Reset the fire rate timer
        fireRateTimer -= fireRate;

        // Recurse if the fire rate timer is still ready
        if (fireRateTimer >= fireRate && fireRate > 0)
            Shoot(playerWeaponManager);
    }

    protected abstract void CustomShoot(PlayerWeaponManager playerWeaponManager);

    public void StartShooting(PlayerWeaponManager playerWeaponManager)
    {
        PlayerWeaponManager = playerWeaponManager;

        // Set the shooting flag
        isShooting = true;

        CustomStartShooting(playerWeaponManager);
    }

    protected abstract void CustomStartShooting(PlayerWeaponManager playerWeaponManager);

    public void StopShooting(PlayerWeaponManager playerWeaponManager)
    {
        // Reset the shooting flag
        isShooting = false;

        CustomStopShooting(playerWeaponManager);
    }

    protected abstract void CustomStopShooting(PlayerWeaponManager playerWeaponManager);
}