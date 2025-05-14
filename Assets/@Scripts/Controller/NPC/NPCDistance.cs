using UnityEngine;

public class NPCDistance : MonoBehaviour
{
    public Transform Player;

    public bool IsDistanceToPlayer()
    {
        float minX = transform.position.x - 2;
        float maxX = transform.position.x + 2;
        float minZ = transform.position.z - 1;
        float maxZ = transform.position.z + 1;

        return (Player.position.x <= maxX && Player.position.x >= minX
            && Player.position.z <= maxZ && Player.position.z >= minZ);
    }
}
