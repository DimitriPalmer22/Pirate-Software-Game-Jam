using System;
using System.Collections;
using UnityEngine;

public class WeaponGun : PlayerWeapon
{
    #region Serialized Fields

    [SerializeField, Min(0)] private float baseDamage = 10;
    [SerializeField, Min(0)] private float range = 20f;
    [SerializeField, Min(0)] private float velocity = 100;

    #endregion

    public override void Shoot(PlayerWeaponManager playerWeaponManager)
    {
        // Start the shoot coroutine
        playerWeaponManager.StartCoroutine(ShootCoroutine(playerWeaponManager));
    }

    private IEnumerator ShootCoroutine(PlayerWeaponManager playerWeaponManager)
    {
        yield return null;

        // Perform a raycast
        var ray = new Ray(playerWeaponManager.FirePoint.position,
            new Vector3(playerWeaponManager.FirePoint.forward.x, 0, playerWeaponManager.FirePoint.forward.z)
        );

        // Keep track of the start and end positions of the ray
        var endPosition = ray.origin + ray.direction * range;

        var hit = Physics.Raycast(ray, out var hitInfo, range, ~layersToIgnore);

        // If the object hit has an IActor component, deal damage to it
        if (hit)
            endPosition = hitInfo.point;

        // Move the position of the bullet over the next couple frames
        while (transform.position != endPosition)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPosition,
                velocity * Time.deltaTime
            );
            yield return null;
        }

        if (hit)
        {
            // If the object hit has an IActor component, deal damage to it
            if (hitInfo.collider.TryGetComponent(out IActor actor))
                actor?.ChangeHealth(-baseDamage, playerWeaponManager.Player, this, hitInfo.point);
        }

        // Wait for 5 seconds to let the trail disappear
        yield return new WaitForSeconds(5);

        // Destroy the bullet
        Destroy(gameObject);
    }
}