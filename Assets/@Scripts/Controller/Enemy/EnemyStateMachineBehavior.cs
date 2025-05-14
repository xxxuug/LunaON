using UnityEngine;

public class EnemyStateMachineBehavior : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemyController = animator.GetComponent<EnemyController>();

        //if (enemyController.CurrentState == EnemyState.Attack)
        //{
        //    enemyController.ChangeState(EnemyState.Chase);
        //}
    }
}
