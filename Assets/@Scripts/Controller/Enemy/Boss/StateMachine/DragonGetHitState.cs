using UnityEngine;

public class DragonGetHitState : State<DragonController>
{
    float _hitDuration = 0.4f;

    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.GetHit();
        _hitDuration = 0.4f;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        _hitDuration -= deltaTime;
        if (_hitDuration <= 0)
        {
            if (context.IsFlying)
                stateMachine.ChangeState<DragonIdleState>();
            else if (context.IsNearRange())
                stateMachine.ChangeState<DragonChaseState>();
            else
                stateMachine.ChangeState<DragonIdleState>();
        }
    }
}
