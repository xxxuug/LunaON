using UnityEngine;

public class MushroomIdleState : State<MushroomController>
{
    float _idleTime;

    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();

        context.Speed = 0f;
        context.IsAttacking = false;
        _idleTime = Random.Range(2f, 4f);

        context.SetRandomIdleTarget();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        _idleTime -= deltaTime;

        if (context.IsMoveRange())
        {
            stateMachine.ChangeState<MushroomChaseState>();
        }
        else if (_idleTime <= 0f)
        {
            context.SetRandomIdleTarget();
            stateMachine.ChangeState<MushroomPatrolState>();
        }
    }
}
