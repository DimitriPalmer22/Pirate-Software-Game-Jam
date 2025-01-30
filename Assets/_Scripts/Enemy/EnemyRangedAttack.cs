using System;
using UnityEngine;

public class EnemyRangedAttack : ComponentScript<Enemy>
{
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Projectile projectilePrefab;
    private Animator _animatorManeki;


    [Header("Difficulty"), SerializeField] private float diffIntervalAdd = -.25f;
    [SerializeField] private float diffDamageAdd = 10;
    [SerializeField] private float diffRangeAdd = 5;
    [SerializeField] private float diffSpeedAdd = 2;

    private Player _player;
    private float _lastAttackTime;
    
    //static hash for the animator
    private static readonly int Fire = Animator.StringToHash("Fire");

    protected override void CustomAwake()
    {
        //get componenet in child and set it to the animator
        _animatorManeki = GetComponentInChildren<Animator>();
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
        //Attack();
    }

    public void Attack()
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

        // Set the last attack time to the current time
        _lastAttackTime = Time.time;

        // Instantiate the projectile
        _animatorManeki.SetTrigger(Fire);
        var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Shoot the projectile at the player
        projectile.Shoot(
            ParentComponent,
            (_player.transform.position - transform.position).normalized,
            attackDamage,
            projectileSpeed
        );
    }
}