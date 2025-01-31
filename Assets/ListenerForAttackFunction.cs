using UnityEngine;

public class ListenerForAttackFunction : MonoBehaviour
{
    //reference to EnemyAttack script
    private EnemyRangedAttack enemyAttack;
    private EnemyKamikazeAttack bombAttack;
    
    // Start is called before the first frame update
    void Awake()
    {
        //get the EnemyAttack script from parent object
        enemyAttack = GetComponentInParent<EnemyRangedAttack>(); 
        
        bombAttack = GetComponentInParent<EnemyKamikazeAttack>();       
    }

    //function to listen for attack
    public void ListenForAttack()
    {
        //call the Attack function
        enemyAttack.Attack();
        
        bombAttack.Attack();
    }
}
