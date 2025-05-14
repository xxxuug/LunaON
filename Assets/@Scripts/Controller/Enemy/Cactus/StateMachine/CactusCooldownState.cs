using UnityEngine;

public class CactusCooldownState : State<CactusController>
{
    float _cooldown = 1.5f;

    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        _cooldown -= deltaTime;

        if (_cooldown <= 0)
        {
            if (context.IsMoveRange())
                stateMachine.ChangeState<CactusChaseState>();
            else
                stateMachine.ChangeState<CactusIdleState>();
        }
    }
}
