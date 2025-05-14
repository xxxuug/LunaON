using UnityEngine;

public class CactusReturnState : State<CactusController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnExit();
        context.Speed = 2f;
        context.IsAttacking = false;
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);
        context.MoveTo(context.SpawnPoint);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (context.ArrivedTo(context.SpawnPoint))
        {
            stateMachine.ChangeState<CactusIdleState>();
        }
    }
}
