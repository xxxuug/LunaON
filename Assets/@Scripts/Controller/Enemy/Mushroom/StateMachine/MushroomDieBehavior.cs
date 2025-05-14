using UnityEngine;

public class MushroomDieBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<MushroomController>()?.OnDeathAnimationEnd();
    }
}
