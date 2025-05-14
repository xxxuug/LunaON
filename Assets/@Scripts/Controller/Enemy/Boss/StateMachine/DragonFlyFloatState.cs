using UnityEngine;

public class DragonFlyFloatState : State<DragonController>
{
    float _coolTime;

    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        _coolTime = 3f;
        Debug.Log("[DragonFlyFloatState] ����");
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        _coolTime -= deltaTime;
        if (_coolTime <= 0f && !context.IsAttacking)
        {
            Debug.Log("[DragonFlyFloatState] AttackState ���� ����");
            stateMachine.ChangeState<DragonAttackState>();
        }
    }
}
