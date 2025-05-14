using UnityEngine;

public class CactusDeadState : State<CactusController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.Die(); // �ִϸ��̼� Ʈ���� �� DieBehaviour���� despawn ����
    }
}
