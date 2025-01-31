using UnityEngine;

public class ListenerForAttackBombEnemy : MonoBehaviour
{
    //reference to EnemyAttack script
    private EnemyKamikazeAttack bombAttack;
    
    // Start is called before the first frame update
    void Awake()
    {
        //get the EnemyAttack script from parent object
        bombAttack = GetComponentInParent<EnemyKamikazeAttack>();       
    }

    //function to listen for attack
    public void ListenForAttack()
    {
        //call the Attack function        
        bombAttack.Attack();
    }
}
