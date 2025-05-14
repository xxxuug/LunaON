using UnityEngine;

public class CactusHitState : State<CactusController>
{
    float _hitDuration = 0.4f;

    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.IsAttacking = false;
        _hitDuration = 0.4f; // 경직 시간 동안 아무것도 안 함
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        _hitDuration -= deltaTime;
        if (_hitDuration <= 0)
        {
            if (context.IsMoveRange())
                stateMachine.ChangeState<CactusChaseState>();
            else
                stateMachine.ChangeState<CactusIdleState>();
        }
    }
}