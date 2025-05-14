using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Vector3 spawnPoint = new Vector3(0,0,0);

    void Start()
    {
        var player = ObjectManager.Instance.Spawn<PlayerController>(spawnPoint);
        player.transform.localScale = new Vector3(2, 2, 2);

        PlayerController.MoveSpeed = 10f;

        Camera playerCamera = player.GetComponentInChildren<Camera>();

        if (playerCamera != null)
            playerCamera.transform.localPosition = new Vector3(0, 3.8f, -3.5f);
    }

}
