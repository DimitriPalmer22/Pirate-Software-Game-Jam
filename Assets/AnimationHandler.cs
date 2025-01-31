using System.Collections;
using UnityEngine;

public class AnimationHandler : StateMachineBehaviour
{
    private int idleHash = Animator.StringToHash("IdleNum");
    private Coroutine _idleRoutine;
    private AnimationHelperScript _helper;
    [SerializeField] private float idleTime = 10f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get or create the helper component
        _helper = animator.GetComponent<AnimationHelperScript>();
        if (_helper == null) 
        {
            _helper = animator.gameObject.AddComponent<AnimationHelperScript>();
        }

        // Start the looping coroutine
        _idleRoutine = _helper.StartCoroutine(IdleCycle(animator));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop the coroutine when leaving the state
        if (_idleRoutine != null && _helper != null)
        {
            _helper.StopCoroutine(_idleRoutine);
        }
    }

    IEnumerator IdleCycle(Animator animator)
    {
        while (true)
        {
            yield return new WaitForSeconds(idleTime);
            
            int randomIdle = Random.Range(1, 4);
            animator.SetInteger(idleHash, randomIdle);
        }
    }
}


