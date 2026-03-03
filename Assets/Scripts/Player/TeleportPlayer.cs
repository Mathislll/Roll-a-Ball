using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public GameObject objectToTeleport;
    public GameObject teleportPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == objectToTeleport)
        {
            objectToTeleport.transform.position = teleportPoint.transform.position;
        }
    }
}
