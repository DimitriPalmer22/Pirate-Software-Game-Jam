using System;
using UnityEngine;

public class EnemyRangedAttack : ComponentScript<Enemy>
{
    // static hash for the animator
    private static readonly int FireAnimationID = Animator.StringToHash("Fire");

    #region Serialized Fields

    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Projectile projectilePrefab;

    [Header("Difficulty"), SerializeField] private float diffIntervalAdd = -.25f;
    [SerializeField] private float diffDamageAdd = 10;
    [SerializeField] private float diffRangeAdd = 5;
    [SerializeField] private float diffSpeedAdd = 2;

    #endregion

    #region Private Fields

    private Player _player;
    private float _lastAttackTime;
    private Animator _animator;
    
    private bool _isFiring;

    #endregion

    protected override void CustomAwake()
    {
        //get component in child and set it to the animator
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // Apply the difficulty
        ApplyDifficulty();
    }

    private void ApplyDifficulty()
    {
        // Add the difficulty interval to the attack interval
        attackInterval += diffIntervalAdd * WaveManager.Instance.Difficulty;

        // Add the difficulty damage to the attack damage
        attackDamage += diffDamageAdd * WaveManager.Instance.Difficulty;

        // Add the difficulty range to the attack range
        attackRange += diffRangeAdd * WaveManager.Instance.Difficulty;
    }

    // Update is called once per frame
    private void Update()
    {
        // Get the player
        _player = Player.Instance;

        // Attack the player
        // Attack();

        // Play the attack animation
        PlayAttackAnimation();
    }

    public void PlayAttackAnimation()
    {
        // If the player is null, return
        if (_player == null)
            return;

        // If the player is not in range, return
        if (Vector3.Distance(transform.position, _player.transform.position) > attackRange)
            return;

        // If the time since the last attack is less than the attack interval, return
        if (Time.time - _lastAttackTime < attackInterval)
            return;

        // Return if the enemy is already firing
        if (_isFiring)
            return;

        // Set the last attack time to the current time
        _lastAttackTime = Time.time;

        // Set the is firing to true
        _isFiring = true;

        // Set the trigger for the fire animation
        _animator?.SetTrigger(FireAnimationID);
    }

    public void Attack()
    {
        // // If the player is null, return
        // if (_player == null)
        //     return;
        //
        // // If the player is not in range, return
        // if (Vector3.Distance(transform.position, _player.transform.position) > attackRange)
        //     return;
        //
        // // If the time since the last attack is less than the attack interval, return
        // if (Time.time - _lastAttackTime < attackInterval)
        //     return;

        // Instantiate the projectile
        var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Shoot the projectile at the player
        projectile.Shoot(
            ParentComponent,
            (_player.transform.position - projectileSpawnPoint.position).normalized,
            attackDamage,
            projectileSpeed
        );

        // Set the is firing to false
        _isFiring = false;
    }
}