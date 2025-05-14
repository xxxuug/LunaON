using UnityEngine;

public class Define
{
    #region Input
    public const string MouseX = "Mouse X";
    public const string MouseY = "Mouse Y";
    public const string MouseScroll = "Mouse ScrollWheel";
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    #endregion

    #region Animation
    public readonly static int Speed = Animator.StringToHash("Speed");
    public readonly static int ComboCount = Animator.StringToHash("ComboCount");
    public readonly static int isNextCombo = Animator.StringToHash("isNextCombo");
    public readonly static int isAttacking = Animator.StringToHash("isAttacking");
    public readonly static int isBattleIdle = Animator.StringToHash("isBattleIdle");
    public readonly static int Jump = Animator.StringToHash("Jump");
    public readonly static int Ground = Animator.StringToHash("Ground");
    public readonly static int DefendHit = Animator.StringToHash("DefendHit");
    public readonly static int Hit = Animator.StringToHash("Hit");
    public readonly static int Die = Animator.StringToHash("Die");
    public readonly static int DieStay = Animator.StringToHash("DieStay");
    public readonly static int isDead = Animator.StringToHash("isDead");
    #endregion

    #region Path
    public const string CactusPath = "@Prefabs/Cactus";
    public const string PlayerPath = "@Prefabs/Player";
    public const string MushroomPath = "@Prefabs/Mushroom";
    #endregion

    #region Tag
    public const string EnemyTag = "Enemy";
    #endregion
}
