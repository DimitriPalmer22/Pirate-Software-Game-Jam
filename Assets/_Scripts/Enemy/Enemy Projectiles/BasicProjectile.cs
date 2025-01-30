using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicProjectile : Projectile
{
    #region Serialized Fields

    [SerializeField] protected int pierceCount = 0;

    #endregion

    #region Private Fields

    protected Rigidbody rb;

    protected Vector3 velocity;
    protected float totalDamage;

    private int _remainingPierceCount;

    #endregion

    private void Awake()
    {
        // Initialize the components
        InitializeComponents();

        // Set the remaining pierce count
        _remainingPierceCount = pierceCount + 1;
    }

    private void InitializeComponents()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Set the rigidbody to ALSO ignore the layers to ignore
        rb.excludeLayers |= layersToIgnore;
    }

    private void FixedUpdate()
    {
        // Move the projectile
        UpdatePosition();
    }

    protected virtual void UpdatePosition()
    {
        // Set the velocity of the Rigidbody
        rb.linearVelocity = velocity;
    }

    protected override void CustomShoot(IActor projectileShooter, Vector3 direction, float damageMult, float speed)
    {
        // Set the total damage
        totalDamage = damageMult * damageMultiplier;

        // Set the velocity
        velocity = direction * (speed * speedMultiplier);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get the actor component
        var isActor = other.gameObject.transform.TryGetComponentInParent(out IActor actor);

        // If the actor is null, return
        if (!isActor)
        {
            // Destroy the projectile
            Destruct();
            return;
        }

        // If the actor is the shooter, return
        if (actor == shooter)
            return;

        // If the actor is a player, and the player is currently dodging, return
        if (actor is Player player && player.PlayerController.IsDodging)
            return;

        // Damage the actor
        actor.ChangeHealth(-totalDamage, shooter, this, other.ClosestPoint(transform.position));

        // Decrement the remaining pierce count
        _remainingPierceCount--;

        // Destroy the projectile IFF the remaining pierce count is 0
        // If the pierce count is -1, the projectile will never be destroyed
        if (_remainingPierceCount == 0)
            Destruct();
    }

    protected override void CustomDestruct()
    {
    }
}