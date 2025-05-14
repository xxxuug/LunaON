using UnityEngine;

public class MushroomChaseState : State<MushroomController>
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
            stateMachine.ChangeState<MushroomAttackState>();
        }
        else if (context.IsReturnToSpawn())
        {
            stateMachine.ChangeState<MushroomReturnState>();
        }
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);
        context.MoveTo(context.Target.position);
    }
}
