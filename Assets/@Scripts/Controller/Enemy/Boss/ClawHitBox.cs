using System.Collections.Generic;
using UnityEngine;

public class ClawHitBox : MonoBehaviour
{
    public int Damage = 20;
    private HashSet<GameObject> _hitTarget = new(); // 맞은 대상 저장

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ClawHitBox에서 GameManager 해시: " + GameManager.Instance.GetHashCode());
        if (other.CompareTag("Player") && !_hitTarget.Contains(other.gameObject))
        {
            var player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(Damage);
                //sDebug.Log("플레이어 hp : " + GameManager.Instance.PlayerInfo.CurrentHp);
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
