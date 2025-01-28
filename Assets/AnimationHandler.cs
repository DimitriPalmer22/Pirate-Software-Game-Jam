using System.Collections;
using UnityEngine;

public class AnimationHandler : StateMachineBehaviour
{
   //static variable to store the hash of the idle parameter
   int idleHash = Animator.StringToHash("IdleNum");
   public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      // Start the coroutine using a helper MonoBehaviour
      animator.GetComponent<AnimationHelperScript>()?.StartCoroutine(Wait(animator));
   }
   
   //Wait for 10 seconds before changing the idle animation
   IEnumerator Wait(Animator animator)
   {
      yield return new WaitForSeconds(10);
      
      int randomIdle = Random.Range(1, 4);
      
      //set idle parameter to random number
      animator.SetInteger(idleHash, randomIdle);
   }
}


