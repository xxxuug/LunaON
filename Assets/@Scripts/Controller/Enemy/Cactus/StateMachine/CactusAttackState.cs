using UnityEngine;

public class CactusAttackState : State<CactusController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.IsAttacking = true;
        context.Attack(); // �ִϸ��̼� Ʈ����
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!context.Target)
        {
            stateMachine.ChangeState<CactusIdleState>();
        }
        else
        {
            context.transform.LookAt(context.Target.position);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        context.IsAttacking = false;
    }
}
