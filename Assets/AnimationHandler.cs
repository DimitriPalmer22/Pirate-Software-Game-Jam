using UnityEngine;

public class AnimationHandler : StateMachineBehaviour
{
   public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      //static variable to store the hash of the idle parameter
      int idleHash = Animator.StringToHash("IdleNum");
      
      int randomIdle = Random.Range(1, 4);
      
      //set idle parameter to random number
      animator.SetInteger(idleHash, randomIdle);
   }
}


