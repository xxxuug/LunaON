using UnityEngine;

public class CactusDeadState : State<CactusController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.Die(); // 애니메이션 트리거 → DieBehaviour에서 despawn 예정
    }
}
