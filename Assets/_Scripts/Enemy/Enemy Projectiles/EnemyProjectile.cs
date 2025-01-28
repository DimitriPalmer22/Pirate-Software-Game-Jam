using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour
{
    public abstract void Shoot(Enemy shooter, Vector3 direction, float damageMult, float speed);
}