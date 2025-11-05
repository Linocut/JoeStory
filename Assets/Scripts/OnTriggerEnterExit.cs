
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterExit : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public bool specificTag = false;
    public string tagName;

    public bool onlyOnce = false;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (onlyOnce && hasTriggered)
        {
            return;
        }
        if (specificTag && other.CompareTag(tagName))
        {
            onTriggerEnter.Invoke();
            hasTriggered = true;
        }
        else if (!specificTag)
        {
            onTriggerEnter.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {   
        if(onlyOnce && hasTriggered)
        {
            return;
        }
        if (specificTag && other.CompareTag(tagName))
        {
            onTriggerExit.Invoke();
            hasTriggered = true;
        }
        else if (!specificTag)
        {
            onTriggerExit.Invoke();
        }
    }
    
}
