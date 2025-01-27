using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : PlayerWeapon
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField, Min(0)] private int maxChainCount = 3;
    [SerializeField, Min(0)] private float maxChainDistance = 10f;
    [SerializeField, Min(0)] private float chainDelayTime = .25f;
    [SerializeField, Min(0)] private float chainStopTime = .25f;
    [SerializeField, Min(0)] private int chainStepCount = 3;
    [SerializeField, Min(0)] private float enemyStunTime = .5f;
    
    [SerializeField] private Sound chainLightningSound;

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
        // Fire out a raycast in the direction of the player's aim to get the first enemy
        var ray = new Ray(playerWeaponManager.FirePoint.transform.position,
            playerWeaponManager.FirePoint.transform.forward
        );

        Physics.Raycast(ray, out var hitInfo, range, ~layersToIgnore);

        // Get the enemy component from the hit object
        Enemy currentEnemy = null;

        if (hitInfo.collider != null)
            hitInfo.collider.TryGetComponentInParent(out currentEnemy);

        // Instantiate the trail prefab
        var trail = Instantiate(bulletPrefab, playerWeaponManager.FirePoint.position, Quaternion.identity);

        // Create a hash set of all the enemies
        var remainingEnemies = new HashSet<Enemy>(Enemy.AllEnemies);

        // Wait for 1 frame
        yield return null;

        var remainingChainCount = maxChainCount;

        var previousPosition = playerWeaponManager.FirePoint.position;

        // If the current enemy is null, break
        if (currentEnemy == null)
        {
            var endPosition = ray.GetPoint(range);

            var remainingStepCount = chainStepCount;
            while (remainingStepCount > 0)
            {
                remainingStepCount--;

                // Lerp the position of the trail to the enemy position
                var trailPosition = Vector3.Lerp(previousPosition, endPosition,
                    1f - (remainingStepCount / (float)chainStepCount));

                // Set the position of the trail
                trail.transform.position = trailPosition;

                // Set the forward direction of the trail
                trail.transform.forward = endPosition - previousPosition;

                // Wait for the step delay time
                yield return new WaitForSeconds(chainDelayTime / chainStepCount);
            }
        }

        // Keep shooting the projectile at the current enemy
        while (remainingChainCount > 0 && currentEnemy != null)
        {
            // Remove the current enemy from the remaining enemies
            remainingEnemies.Remove(currentEnemy);

            // If the current enemy is null, break
            if (currentEnemy == null)
                break;

            // Get the position of the current enemy
            var enemyPosition = currentEnemy.transform.position;

            // TODO: Do more stuff here. Spawn a VFX, play a sound, etc.
            
            // Play the sound at the enemy position
            var audioSource = SoundManager.Instance.PlaySfxAtPoint(chainLightningSound, enemyPosition);

            var remainingStepCount = chainStepCount;
            while (remainingStepCount > 0)
            {
                remainingStepCount--;

                // Lerp the position of the trail to the enemy position
                var trailPosition = Vector3.Lerp(previousPosition, enemyPosition,
                    1f - (remainingStepCount / (float)chainStepCount));

                // Set the position of the trail
                trail.transform.position = trailPosition;

                // Set the forward direction of the trail
                trail.transform.forward = enemyPosition - previousPosition;
                
                // Move the audio source to the trail position
                if (audioSource != null)
                    audioSource.transform.position = trailPosition;

                // Wait for the step delay time
                yield return new WaitForSeconds(chainDelayTime / chainStepCount);
            }

            if (currentEnemy != null)
            {
                // Damage the current enemy
                currentEnemy.ChangeHealth(-baseDamage, playerWeaponManager.Player, this, enemyPosition);

                // // Start the coroutine to stun the enemy
                // StartCoroutine(StunEnemy(currentEnemy, enemyStunTime));
            }

            // Decrement the remaining chain count
            remainingChainCount--;

            var chainStopTimeStart = Time.time;

            // Wait for the chain stop time
            while (Time.time - chainStopTimeStart < chainStopTime)
            {
                // If the current enemy is null, break
                if (currentEnemy == null)
                {
                    yield return null;
                    continue;
                }

                // Update the position of the trail
                trail.transform.position = currentEnemy.transform.position;

                // Wait for 1 frame
                yield return null;
            }

            // Find the closest enemy to the current enemy
            if (remainingChainCount > 0)
                currentEnemy = GetClosestEnemy(enemyPosition, remainingEnemies);

            // Set the previous position to the current enemy position
            previousPosition = trail.transform.position;
        }

        // Destroy the trail
        Destroy(trail.gameObject);
    }

    private Enemy GetClosestEnemy(Vector3 position, IEnumerable<Enemy> enemies)
    {
        Enemy closestEnemy = null;
        var closestDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy == null)
                continue;

            var distance = Vector3.Distance(position, enemy.transform.position);

            // Continue if the enemy is out of range
            if (distance > maxChainDistance)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
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
        Debug.Log($"Chain Lightning Upgrade 1!");

        fireRate /= 1.5f;
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