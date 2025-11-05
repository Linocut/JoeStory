using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    public Transform targetLocation;
    public GameObject objectToTeleport;

    public void Teleport()
    {
        if (objectToTeleport != null && targetLocation != null)
        {
            objectToTeleport.transform.position = targetLocation.position;
            objectToTeleport.transform.rotation = targetLocation.rotation;
        }
    }
 
}
