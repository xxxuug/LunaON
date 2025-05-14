using UnityEngine;

public class DragonTakeOffState : State<DragonController>
{
    public override void OnInitialized() { }
    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        Debug.Log("[DragonTakeOffState] 진입");
        context.TakeOff(); // DragonController에 정의돼 있어야 함
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!(stateMachine.CurrentState is DragonFlyFloatState))
        {
            stateMachine.ChangeState<DragonFlyFloatState>();
        }
    }
}
