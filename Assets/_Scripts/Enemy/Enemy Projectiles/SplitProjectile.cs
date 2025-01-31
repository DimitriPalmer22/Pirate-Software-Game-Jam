using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitProjectile : BasicProjectile
{
    [SerializeField, Min(0)] protected int splitsRemaining = 2;
    [SerializeField, Min(2)] protected int splitCount = 2;
    [SerializeField, Min(0)] protected float splitTime = 1f;
    [SerializeField, Min(0)] protected float splitAngle = 45;

    private float _startTime;
    private bool _hasSplit;
    private bool _isAChild = false;

    private void Start()
    {
        // Set the split time
        _startTime = splitTime;

        // Start the repeater coroutine
        if (!_isAChild)
            StartCoroutine(Repeater());
    }

    private IEnumerator Repeater()
    {
        var remaining = splitsRemaining;

        while (remaining > 0)
        {
            yield return new WaitForSeconds(splitTime);
            Duplicate();
            remaining--;
        }
    }

    private void Split()
    {
        // return if there are no splits remaining
        if (splitsRemaining <= 0)
            return;

        // return if the projectile has already split
        if (_hasSplit)
            return;

        // Return if the current time minus the split time is less than the split time
        if (Time.time - _startTime < splitTime)
            return;

        // Get the current forward velocity vector
        var forward = rb.linearVelocity.normalized;

        for (int i = 0; i < splitCount; i++)
        {
            // Create a new projectile
            var projectile = Instantiate(this, transform.position, Quaternion.identity);

            var currentAngle = ((-splitCount / 2f) + i) * (splitAngle / (splitCount));

            /*
             * 2 splitcount
             * 45deg
             * ((-2 / 2) + 0) * (45 / 2) = 0
             * -1 * 22.5 = -22.5
             *
             */

            var newAngle = Quaternion.AngleAxis(currentAngle, Vector3.up) * forward;

            // Set the velocity of the new projectile
            projectile.Shoot(shooter, newAngle, totalDamage, velocity.magnitude);

            // Set the remaining pierce count
            projectile.splitsRemaining = splitsRemaining - 1;
        }

        // Set the has split flag to true
        _hasSplit = true;
    }

    private void Duplicate()
    {
        // Create a new projectile
        var projectile = Instantiate(this, transform.position, Quaternion.identity);

        // Set the velocity of the new projectile
        projectile.Shoot(shooter, rb.linearVelocity.normalized, totalDamage, rb.linearVelocity.magnitude);

        // Set the remaining pierce count
        projectile.splitsRemaining = splitsRemaining - 1;

        // Set the is a child flag to true
        projectile._isAChild = true;
    }
}