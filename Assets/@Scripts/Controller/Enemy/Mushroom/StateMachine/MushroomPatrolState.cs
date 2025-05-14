using UnityEngine;

public class MushroomPatrolState : State<MushroomController>
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
            stateMachine.ChangeState<MushroomChaseState>();
        }
        else if (context.ArrivedTo(context.IdleTarget))
        {
            stateMachine.ChangeState<MushroomIdleState>();
        }
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        context.MoveTo(context.IdleTarget);
    }
}
