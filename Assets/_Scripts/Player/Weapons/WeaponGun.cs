using System;
using System.Collections;
using UnityEngine;

public class WeaponGun : PlayerWeapon
{
    #region Serialized Fields

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField, Min(0)] private float baseDamage = 10;
    [SerializeField, Min(0)] private float fireRate = 0.125f;
    [SerializeField, Min(0)] private float range = 20f;
    [SerializeField, Min(0)] private float velocity = 100;

    #endregion

    #region Private Fields

    private bool _isShooting;
    private float _fireRateTimer;

    #endregion

    private void Awake()
    {
        // Initialize the fire rate timer
        _fireRateTimer = fireRate;
    }

    private void Update()
    {
        // Update the fire rate timer
        UpdateFireRateTimer();
        
        Shoot(PlayerWeaponManager);
    }

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
        
        // Start the shoot coroutine
        playerWeaponManager.StartCoroutine(ShootCoroutine(playerWeaponManager));
        
        // Reset the fire rate timer
        _fireRateTimer -= fireRate;
        
        // Recurse if the fire rate timer is still ready
        if (_fireRateTimer >= fireRate && fireRate > 0)
            Shoot(playerWeaponManager);
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
            new Vector3(playerWeaponManager.FirePoint.forward.x, 0, playerWeaponManager.FirePoint.forward.z)
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

        if (hit)
        {
            // If the object hit has an IActor component, deal damage to it
            if (hitInfo.collider.TryGetComponent(out IActor actor))
                actor?.ChangeHealth(-baseDamage, playerWeaponManager.Player, this, hitInfo.point);
        }

        // Wait for 5 seconds to let the trail disappear
        yield return new WaitForSeconds(5);

        // Destroy the bullet
        Destroy(bullet.gameObject);
    }

    protected override void CustomStartShooting(PlayerWeaponManager playerWeaponManager)
    {
        // Set the shooting flag
        _isShooting = true;
    }

    protected override void CustomStopShooting(PlayerWeaponManager playerWeaponManager)
    {
        // Reset the shooting flag
        _isShooting = false;
    }
}