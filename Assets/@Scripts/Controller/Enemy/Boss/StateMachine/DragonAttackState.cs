using UnityEngine;

public class DragonAttackState : State<DragonController>
{
    AttackType _attackType;

    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.IsAttacking = true;

        float random = Random.Range(0f, 1f);

        Debug.Log("[DragonAttackState] 공격 모션 직전");
        if (context.IsFlying)
        {
            Debug.Log("[DragonAttackState] 공중 공격 모션 진입");
            _attackType = random > 0.5f ? AttackType.FlyFlameAttack : AttackType.FlyFlameAttack;
        }
        else
        {
            _attackType = random > 0.5f ? AttackType.ClawAttack : AttackType.FlameAttack;
        }
        context.SetAttackType(_attackType);
        Debug.Log("[DragonAttackState] 실행중인 공격 모션 : " + _attackType);

    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
