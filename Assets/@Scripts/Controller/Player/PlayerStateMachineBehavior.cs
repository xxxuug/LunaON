using UnityEngine;

public class PlayerStateMachineBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        PlayerController controller = animator.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.PlaySwordEffect();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        float currentTime = stateInfo.normalizedTime;
        bool isNextCombo = animator.GetBool(Define.isNextCombo);
        bool isAttacking = animator.GetBool(Define.isAttacking);
        
        if (currentTime < 0.98f && currentTime > 0.25f && isNextCombo)
        {
            // ���� ���ð� �ʱ�ȭ
            PlayerController._battleIdleTime = 0f;
            int comboCount = animator.GetInteger(Define.ComboCount);
            comboCount = comboCount < 3 ? ++comboCount : 0;
            animator.SetInteger(Define.ComboCount, comboCount);

            //Debug.Log($"[SMB] �޺� ī��Ʈ ���� �� {comboCount}");

        }
        if (currentTime >= 1.1f)
        {
            // ���� ��� ��� ����
            animator.SetInteger(Define.ComboCount, 0);
            animator.SetBool(Define.isAttacking, false);
            animator.SetBool(Define.isNextCombo, false);

            //Debug.Log("[SMB] ���� ���� ���� �� ���µ�");

        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        animator.SetBool(Define.isNextCombo, false);
    }
}
