using UnityEngine;

public class EnemyRangedAttack : ComponentScript<Enemy>
{
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private EnemyProjectile projectilePrefab;

    private Player _player;
    private float _lastAttackTime;

    protected override void CustomAwake()
    {
    }

    private void InitializeComponents()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
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

        // If the time since the last attack is less than the attack interval, return
        if (Time.time - _lastAttackTime < attackInterval)
            return;

        // Set the last attack time to the current time
        _lastAttackTime = Time.time;
        
        // Instantiate the projectile
        var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        
        // Shoot the projectile at the player
        projectile.Shoot(ParentComponent, (_player.transform.position - transform.position).normalized, attackDamage, 10f);
    }
}