using UnityEngine;

public class DragonSleepState : State<DragonController>
{
    public override void OnInitialized() { }
    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (context.Target && context.IsStartRange())
        {
            context.IsPlayerNear = true;

            if (!(stateMachine.CurrentState is DragonIdleState))
            {
                Debug.Log("Sleep → Idle 상태 전환!");
                stateMachine.ChangeState<DragonIdleState>();
            }
        }
    }
}
