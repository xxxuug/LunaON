using UnityEngine;

public class CactusPatrolState : State<CactusController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 1f;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (context.IsMoveRange())
        {
            stateMachine.ChangeState<CactusChaseState>();
        }
        else if (context.ArrivedTo(context.IdleTarget))
        {
            stateMachine.ChangeState<CactusIdleState>();
        }
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        context.MoveTo(context.IdleTarget);
    }
}
