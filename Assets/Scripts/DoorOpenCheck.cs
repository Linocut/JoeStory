using UnityEngine;
using UnityEngine.Events;

public class DoorOpenCheck : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private GameObject door;
    [SerializeField] private float yRotationThreshold = 45f;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onDoorOpened;
    
    private float startingYRotation;
    
    private void Start()
    {
        if (door != null)
        {
            startingYRotation = door.transform.localEulerAngles.y;
        }
    }
    
    /// <summary>
    /// Checks if the door's Y rotation exceeds the threshold from its starting rotation and invokes the event
    /// </summary>
    public void CheckDoorPosition()
    {
        if (door == null)
            return;
        
        float currentYRotation = door.transform.localEulerAngles.y;
        float upperThreshold = startingYRotation + yRotationThreshold;
        float lowerThreshold = startingYRotation - yRotationThreshold;
        
        // Check if door opened in either direction from starting rotation
        if (currentYRotation >= upperThreshold || currentYRotation <= lowerThreshold)
        {
            onDoorOpened?.Invoke();
        }
    }
}
