using UnityEngine;

public abstract class Projectile : MonoBehaviour, IDamager
{
    [SerializeField] protected LayerMask layersToIgnore;

    [SerializeField, Min(0)] protected float damageMultiplier = 1;
    [SerializeField, Min(0)] protected float speedMultiplier = 1;

    [SerializeField, Min(0)] protected float lifeTime = 5;

    protected IActor shooter;

    public GameObject GameObject => gameObject;

    public void Shoot(IActor projectileShooter, Vector3 direction, float damageMult, float speed)
    {
        shooter = projectileShooter;

        // Call the custom shoot method
        CustomShoot(shooter, direction, damageMult, speed);

        // Destroy the projectile after the lifetime
        Invoke(nameof(Destruct), lifeTime);
    }

    protected abstract void CustomShoot(IActor projectileShooter, Vector3 direction, float damageMult, float speed);

    protected void Destruct()
    {
        // Call the custom destruct method
        CustomDestruct();

        // Destroy the projectile's game object
        Destroy(gameObject);
    }

    protected abstract void CustomDestruct();
}