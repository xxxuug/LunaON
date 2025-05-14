using UnityEngine;

public class EnemyController : BaseController, IDamageable
{
    public string Tag { get; set; } = Define.EnemyTag;

    [Header("Status")]
    protected float _maxHp = 100f;
    protected float _currentHp;

    protected Transform _target;


    protected override void Initialize()
    {
        // 체력 초기화는 CactusController에서 호출
    }

    public virtual bool AnyDamage(float damage, GameObject damageCauser, Vector2 hitPoint = default)
    {
        return false; // override에서 처리
    }

    protected virtual void HitAndDie(float damage)
    {
        // override에서 처리
    }

    protected void Despawn()
    {
        ObjectManager.Instance.Despawn(this);
    }

    public virtual void SetSpawnPoint(Vector3 spawnPos)
    {
        // override에서 처리
    }
}
