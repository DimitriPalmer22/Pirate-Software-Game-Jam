using UnityEngine;

public class EnergyBolts : PlayerWeapon
{
    #region Serialized Fields

    [SerializeField, Min(0)] private float boltCount = 3;
    [SerializeField, Min(0)] private float boltAngle = 90;
    [SerializeField, Min(0)] private float boltSpeed = 10f;

    [SerializeField] private EnergyBoltsProjectile projectilePrefab;

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

        var rotationInterval = boltAngle / boltCount;
        
        for (var i = 0; i < boltCount; i++)
        {
            // Rotate the forward direction by the rotation interval
            var rotation = Quaternion.Euler(0, i * rotationInterval - boltAngle / 2, 0);
            
            var newForward = rotation * forward;
            
            // Instantiate the projectile
            var projectile = Instantiate(projectilePrefab, playerWeaponManager.FirePoint.position, rotation);
            
            // Shoot the projectile
            projectile.Shoot(playerWeaponManager.ParentComponent, newForward, baseDamage, boltSpeed);
        }
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