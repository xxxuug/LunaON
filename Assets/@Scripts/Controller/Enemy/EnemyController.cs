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
        // ü�� �ʱ�ȭ�� CactusController���� ȣ��
    }

    public virtual bool AnyDamage(float damage, GameObject damageCauser, Vector2 hitPoint = default)
    {
        return false; // override���� ó��
    }

    protected virtual void HitAndDie(float damage)
    {
        // override���� ó��
    }

    protected void Despawn()
    {
        ObjectManager.Instance.Despawn(this);
    }

    public virtual void SetSpawnPoint(Vector3 spawnPos)
    {
        // override���� ó��
    }
}
