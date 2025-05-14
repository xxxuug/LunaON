using UnityEngine;

public class DragonTakeOffState : State<DragonController>
{
    public override void OnInitialized() { }
    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        Debug.Log("[DragonTakeOffState] ����");
        context.TakeOff(); // DragonController�� ���ǵ� �־�� ��
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
