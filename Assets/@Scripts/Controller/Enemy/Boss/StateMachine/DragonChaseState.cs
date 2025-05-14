using UnityEngine;

public class DragonChaseState : State<DragonController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 1.0f;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (context.Target && context.IsNearRange())
        {
            stateMachine.ChangeState<DragonAttackState>();
        }
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);
        context.MoveTo(context.Target.position);
    }
}


