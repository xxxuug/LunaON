using UnityEngine;

public class CactusDieBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<CactusController>()?.OnDeathAnimationEnd();
    }
}
