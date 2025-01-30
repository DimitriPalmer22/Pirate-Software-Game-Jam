using System.Linq;
using UnityEngine;

public class EnergizedBulletsProjectile : BasicProjectile
{
    private Enemy _target;
    private float _homingStrength;
    private float _speed;
    private Vector3 _direction;

    protected override void UpdatePosition()
    {
        // Determine the target
        DetermineTarget();

        // Move the projectile
        MoveProjectile();
    }

    private void DetermineTarget()
    {
        // Get all the enemies  
        var enemies = Enemy.AllEnemies;

        // If there are no enemies, return
        if (enemies.Count == 0)
            return;

        // Sort the enemies by homing score
        var enemy = enemies.OrderByDescending(HomingScore).First();

        // Set the target
        _target = enemy;

        // If there is a target 
        if (_target == null)
            return;

        // Get the difference between the target position and the projectile position
        var difference = _target.transform.position - transform.position;

        // Calculate the desired velocity
        var desiredDirection = difference.normalized;

        // Lerp the direction
        _direction = Vector3.Lerp(
            velocity.normalized, desiredDirection,
            CustomFunctions.FrameAmount(_homingStrength, true)
        );
    }

    private void MoveProjectile()
    {
        // Set the velocity
        velocity = _direction * _speed;

        // Set the velocity of the Rigidbody
        rb.linearVelocity = velocity;
    }

    private float HomingScore(Enemy enemy)
    {
        // If the enemy is null, return 0
        if (enemy == null)
            return 0;

        // Get the vector to the enemy
        var vectorToEnemy = enemy.transform.position - transform.position;

        // Get the dot product
        var dot = Vector3.Dot(vectorToEnemy.normalized, velocity.normalized);

        // If the dot is less than 0, return 0
        if (dot <= 0)
            return 0;

        // Get the distance to the enemy
        var distance = vectorToEnemy.magnitude;

        // Get the homing score
        var homingScore = dot * (1 - distance / 10);

        return homingScore;
    }

    public void Shoot(IActor projectileShooter, Vector3 direction, float damageMult, float speed, float homingStrength)
    {
        // Set the homing strength
        _homingStrength = homingStrength;

        // Set the speed
        _speed = speed;

        // Set the direction
        _direction = direction;

        // Call the base shoot method
        base.Shoot(projectileShooter, direction, damageMult, speed);
    }
}