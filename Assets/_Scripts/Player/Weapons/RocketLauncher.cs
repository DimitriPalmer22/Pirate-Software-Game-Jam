using UnityEngine;

public class RocketLauncher : PlayerWeapon
{
    #region Serialized Fields

    [SerializeField, Range(0, 1)] protected float homingStrength = 0f;
    [SerializeField, Min(0)] protected float bulletSpeed = 10f;
    [SerializeField, Min(0)] protected float explosionRadius = 5f;
    [SerializeField, Min(0)] protected float explosionDamage = 40f;

    [SerializeField] private RocketLauncherProjectile projectilePrefab;

    #endregion

    protected override void CustomAwake()
    {
    }

    protected override void CustomUpdate()
    {
    }

    protected override void CustomShoot(PlayerWeaponManager playerWeaponManager)
    {
        // Get the forward direction of the player
        var forward = playerWeaponManager.AimForward;

        // Instantiate the projectile
        var projectile = Instantiate(projectilePrefab, playerWeaponManager.FirePoint.position, Quaternion.identity);

        // Shoot the projectile
        projectile.Shoot(
            playerWeaponManager.ParentComponent, forward, baseDamage, bulletSpeed, homingStrength,
            explosionRadius, explosionDamage
        );
    }

    protected override void CustomStartShooting(PlayerWeaponManager playerWeaponManager)
    {
    }

    protected override void CustomStopShooting(PlayerWeaponManager playerWeaponManager)
    {
    }

    #region Upgrades

    protected override void CustomUpgrade1()
    {
    }

    protected override void CustomUpgrade2()
    {
    }

    protected override void CustomUpgrade3()
    {
    }

    protected override void CustomUpgrade4()
    {
    }

    protected override void CustomUpgrade5()
    {
    }

    #endregion
}