using UnityEngine;
using UnityEngine.Events;

public class OnEnableScript : MonoBehaviour
{
    public UnityEvent onEnableEvent;

    // OnEnable is called when the object becomes enabled and active
    void OnEnable()
    {
        onEnableEvent?.Invoke();
    }


}
