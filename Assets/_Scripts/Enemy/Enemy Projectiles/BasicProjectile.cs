using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicProjectile : Projectile
{
    #region Serialized Fields

    [SerializeField] protected int pierceCount = 0;

    #endregion

    #region Private Fields

    private Rigidbody _rigidbody;

    private Vector3 _velocity;
    private float _totalDamage;

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
        _rigidbody = GetComponent<Rigidbody>();

        // Set the rigidbody to ALSO ignore the layers to ignore
        _rigidbody.excludeLayers |= layersToIgnore;
    }

    private void FixedUpdate()
    {
        // Move the projectile
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        // Set the velocity of the Rigidbody
        _rigidbody.linearVelocity = _velocity;
    }

    protected override void CustomShoot(IActor projectileShooter, Vector3 direction, float damageMult, float speed)
    {
        // Set the total damage
        _totalDamage = damageMult * damageMultiplier;

        // Set the velocity
        _velocity = direction * (speed * speedMultiplier);
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
        actor.ChangeHealth(-_totalDamage, shooter, this, other.ClosestPoint(transform.position));

        // Decrement the remaining pierce count
        _remainingPierceCount--;
        
        Debug.Log(_remainingPierceCount);

        // Destroy the projectile IFF the remaining pierce count is 0
        // If the pierce count is -1, the projectile will never be destroyed
        if (_remainingPierceCount == 0)
            Destruct();
    }

    protected override void CustomDestruct()
    {
    }
}