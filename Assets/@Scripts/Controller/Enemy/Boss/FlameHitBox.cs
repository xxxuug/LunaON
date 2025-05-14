using System.Collections.Generic;
using UnityEngine;

public class FlameHitBox : MonoBehaviour
{
    public int Damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ClawHitBox에서 GameManager 해시: " + GameManager.Instance.GetHashCode());
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(Damage);
                //Debug.Log("플레이어 hp : " + GameManager.Instance.PlayerInfo.CurrentHp);
            }
        }
    }

    public void ActivateHitbox()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        gameObject.SetActive(false);
    }
}
