using UnityEngine;

public class DragonDieState : State<DragonController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0f;
        context.Die();
    }
}
