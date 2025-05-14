using UnityEngine;

public class WeaponHitbox : BaseController
{
    private bool _canHit = false;

    public void EnableAttack() { _canHit = true; }
    public void DisableAttack() { _canHit = false; }

    protected override void Initialize() { }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null || damageable.Tag != Define.EnemyTag) return;

        if (other.gameObject.CompareTag("Enemy") && _canHit)
        {
            float atk = GameManager.Instance.PlayerInfo.Atk;
            // IDamageable�� �� ��ü�� �ƴ� �÷��̾ �Ѱܾ� ���Ͱ� �÷��̾ �߰��� �� ����
            GameObject player = ObjectManager.Instance.Player.gameObject;
            damageable.AnyDamage(atk, player);
            DisableAttack();
        }
    }

}
