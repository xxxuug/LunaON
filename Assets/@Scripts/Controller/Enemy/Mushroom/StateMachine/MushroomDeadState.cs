using UnityEngine;

public class MushroomDeadState : State<MushroomController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.Die(); // �ִϸ��̼� Ʈ���� �� DieBehaviour���� despawn ����
    }
}
