using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour, IDamager
{
    [SerializeField] protected LayerMask layersToIgnore;
    
    public GameObject GameObject => gameObject;

    public abstract void Shoot(PlayerWeaponManager playerWeaponManager);
}