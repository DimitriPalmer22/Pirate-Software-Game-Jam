using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour, IDamager
{
    [SerializeField] protected LayerMask layersToIgnore;
    
    protected PlayerWeaponManager PlayerWeaponManager { get; private set; }
    
    public GameObject GameObject => gameObject;

    public void StartShooting(PlayerWeaponManager playerWeaponManager)
    {
        PlayerWeaponManager = playerWeaponManager;
        
        CustomStartShooting(playerWeaponManager);
    }
    
    protected abstract void CustomStartShooting(PlayerWeaponManager playerWeaponManager);

    public void StopShooting(PlayerWeaponManager playerWeaponManager)
    {
        CustomStopShooting(playerWeaponManager);
    }
    
    protected abstract void CustomStopShooting(PlayerWeaponManager playerWeaponManager);

}