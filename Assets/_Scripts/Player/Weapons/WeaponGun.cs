using System;
using System.Collections;
using UnityEngine;

public class WeaponGun : PlayerWeapon
{
    #region Serialized Fields

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Min(0)] private float velocity = 100;

    #endregion

    #region Private Fields

    #endregion

    protected override void CustomAwake()
    {
    }

    protected override void CustomUpdate()
    {
    }

    protected override void CustomShoot(PlayerWeaponManager playerWeaponManager)
    {
        // Start the shoot coroutine
        playerWeaponManager.StartCoroutine(ShootCoroutine(playerWeaponManager));
    }

    private IEnumerator ShootCoroutine(PlayerWeaponManager playerWeaponManager)
    {
        var bullet = Instantiate(
            bulletPrefab,
            playerWeaponManager.FirePoint.position,
            playerWeaponManager.FirePoint.rotation
        );

        yield return null;

        // Perform a raycast
        var ray = new Ray(playerWeaponManager.FirePoint.position,
            new Vector3(playerWeaponManager.AimForward.x, 0, playerWeaponManager.AimForward.z)
        );

        // Keep track of the start and end positions of the ray
        var endPosition = ray.origin + ray.direction * range;

        var hit = Physics.Raycast(ray, out var hitInfo, range, ~layersToIgnore);

        // If the object hit has an IActor component, deal damage to it
        if (hit)
            endPosition = hitInfo.point;

        // Move the position of the bullet over the next couple frames
        while (bullet.transform.position != endPosition)
        {
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                endPosition,
                velocity * Time.deltaTime
            );
            yield return null;
        }

        // If the object hit has an IActor component, deal damage to it
        if (hit && hitInfo.collider != null && hitInfo.collider.TryGetComponent(out IActor actor))
            actor?.ChangeHealth(-baseDamage, playerWeaponManager.ParentComponent, this, hitInfo.point);

        // Wait for 5 seconds to let the trail disappear
        yield return new WaitForSeconds(5);

        // Destroy the bullet
        Destroy(bullet.gameObject);
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