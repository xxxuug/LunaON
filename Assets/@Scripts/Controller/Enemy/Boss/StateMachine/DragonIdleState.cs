using UnityEngine;

public class DragonIdleState : State<DragonController>
{
    float _coolTime;

    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.IsAttacking = false;
        context.SetAttackType(AttackType.None);
        _coolTime = 1f;
        Debug.Log($"상태 진입: {GetType().Name}");
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        _coolTime -= deltaTime;
        //Debug.Log("현재 쿨타임 : " + _coolTime);

        if (context.IsFlying)
        {
            Debug.Log($"[DragonIdleState] isFlying == true → TakeOff 진입");
            stateMachine.ChangeState<DragonTakeOffState>();
            return;
        }

        if (_coolTime < 0f)
        {
            if (context.Target && context.IsNearRange())
            {
                if (!context.IsAttacking)
                {
                    stateMachine.ChangeState<DragonAttackState>();
                }
            }
            else
            {
                stateMachine.ChangeState<DragonChaseState>();
            }
        }
    }
}
