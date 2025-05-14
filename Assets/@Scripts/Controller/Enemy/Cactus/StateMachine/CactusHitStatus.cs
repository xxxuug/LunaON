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
        _hitDuration = 0.4f; // ���� �ð� ���� �ƹ��͵� �� ��
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