using UnityEngine;

public class MushroomAttackState : State<MushroomController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.IsAttacking = true;
        context.Attack(); // 애니메이션 트리거
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!context.Target)
        {
            stateMachine.ChangeState<MushroomIdleState>();
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
