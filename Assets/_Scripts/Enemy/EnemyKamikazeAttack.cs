using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKamikazeAttack : ComponentScript<Enemy>, IDamager
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField, Min(0)] private float damage = 30;
    [SerializeField, Min(0)] private float explosionRadius = 5f;
    [SerializeField, Min(0)] private float fuseTime = 3f;

    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private ParticleSystem explosionParticlesPrefab;

    [Header("Difficulty"), SerializeField] private float diffDamageAdd = 20;

    private Player _player;
    private bool _isExploding;

    public GameObject GameObject => gameObject;

    private void Start()
    {
        // Apply the difficulty
        ApplyDifficulty();
    }

    private void ApplyDifficulty()
    {
        // Add the difficulty damage to the damage
        damage += diffDamageAdd * WaveManager.Instance.Difficulty;
    }

    private void Update()
    {
        // Get the player
        _player = Player.Instance;

        // Attack the player
        Attack();
    }

    private void Attack()
    {
        // If the player is null, return
        if (_player == null)
            return;

        // If the player is not in range, return
        if (Vector3.Distance(transform.position, _player.transform.position) > attackRange)
            return;

        // If the enemy is already exploding, return
        if (_isExploding)
            return;

        // Explode
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        // Set the enemy to exploding
        _isExploding = true;

        // If the smoke is not null, play it
        smokeParticles?.Play();

        // Wait for the fuse time
        yield return new WaitForSeconds(fuseTime);

        // Stop the smoke particles
        smokeParticles?.Stop();

        // If there is a prefab for the explosion, instantiate it
        if (explosionParticlesPrefab != null)
        {
            var explosionParticles = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
            explosionParticles.Play();
        }

        // Create a hash set to store all the actors that have been damaged
        var damagedActors = new HashSet<IActor>();

        // Get all the colliders in the explosion radius
        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Loop through all the colliders
        foreach (var cCollider in colliders)
        {
            // Get the actor component
            var hasActor = cCollider.TryGetComponentInParent(out IActor actor);

            // If the actor is null, continue
            if (!hasActor || actor == null)
                continue;

            // If the actor is the enemy, continue
            if (actor == ParentComponent)
                continue;

            // If the actor has already been damaged, continue
            // Add the actor to the damaged actors hash set
            if (!damagedActors.Add(actor))
                continue;

            // Damage the actor
            actor.ChangeHealth(-damage, ParentComponent, this, cCollider.ClosestPoint(transform.position));
        }

        // Die
        ParentComponent.ChangeHealth(-ParentComponent.MaxHealth, ParentComponent, this, transform.position);
    }
}