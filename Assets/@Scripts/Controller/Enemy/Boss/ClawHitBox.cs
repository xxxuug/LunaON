using System.Collections.Generic;
using UnityEngine;

public class ClawHitBox : MonoBehaviour
{
    public int Damage = 20;
    private HashSet<GameObject> _hitTarget = new(); // ���� ��� ����

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ClawHitBox���� GameManager �ؽ�: " + GameManager.Instance.GetHashCode());
        if (other.CompareTag("Player") && !_hitTarget.Contains(other.gameObject))
        {
            var player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(Damage);
                //sDebug.Log("�÷��̾� hp : " + GameManager.Instance.PlayerInfo.CurrentHp);
                _hitTarget.Add(other.gameObject);
            }
        }
    }

    public void ActivateHitbox()
    {
        gameObject.SetActive(true);
        _hitTarget.Clear();
    }

    public void DeactivateHitbox()
    {
        gameObject.SetActive(false);
    }
}
