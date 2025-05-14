using UnityEngine;

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Return
}

public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }

    public void ChangeSate(EnemyState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }
}
