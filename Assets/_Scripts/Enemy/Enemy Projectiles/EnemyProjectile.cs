using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour, IDamager
{
    [SerializeField] protected LayerMask layersToIgnore;
    
    [SerializeField, Min(0)] protected float damageMultiplier = 1;
    [SerializeField, Min(0)] protected float speedMultiplier = 1;

    protected Enemy shooter;

    public GameObject GameObject => gameObject;

    public void Shoot(Enemy projectileShooter, Vector3 direction, float damageMult, float speed)
    {
        shooter = projectileShooter;

        // Call the custom shoot method
        CustomShoot(shooter, direction, damageMult, speed);
    }

    protected abstract void CustomShoot(Enemy projectileShooter, Vector3 direction, float damageMult, float speed);
}