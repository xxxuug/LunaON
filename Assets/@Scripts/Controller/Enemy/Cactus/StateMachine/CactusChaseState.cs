using UnityEngine;

public class CactusChaseState : State<CactusController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        context.Speed = 2.5f;

        if (context.IsAttackRange())
        {
            stateMachine.ChangeState<CactusAttackState>();
        }
        else if (context.IsReturnToSpawn())
        {
            stateMachine.ChangeState<CactusReturnState>();
        }
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);
        context.MoveTo(context.Target.position);
    }
}
