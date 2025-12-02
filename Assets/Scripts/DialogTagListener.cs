using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class TaggedDialogEvent
{
    public string eventTag;
    public UnityEvent<string> onDialogStart;
    public UnityEvent<string> onTypewriterComplete;
    public UnityEvent<string> onDialogEnd;
}

public class DialogTagListener : MonoBehaviour
{
    [Header("Tagged Events")]
    public List<TaggedDialogEvent> taggedEvents = new List<TaggedDialogEvent>();
    
    [Header("Fallback Events (any dialog)")]
    public UnityEvent<string> onAnyDialogStart;
    public UnityEvent<string> onAnyTypewriterComplete;
    public UnityEvent<string> onAnyDialogEnd;
    
    void OnEnable()
    {
        DialogEventBroadcaster.OnDialogStarted += HandleDialogStart;
        DialogEventBroadcaster.OnTypewriterCompleted += HandleTypewriterComplete;
        DialogEventBroadcaster.OnDialogEnded += HandleDialogEnd;
    }
    
    void OnDisable()
    {
        DialogEventBroadcaster.OnDialogStarted -= HandleDialogStart;
        DialogEventBroadcaster.OnTypewriterCompleted -= HandleTypewriterComplete;
        DialogEventBroadcaster.OnDialogEnded -= HandleDialogEnd;
    }
    
    private void HandleDialogStart(string dialogText, string eventTag)
    {
        onAnyDialogStart?.Invoke(dialogText);
        
        // Check tagged events
        foreach (var taggedEvent in taggedEvents)
        {
            if (TagMatches(eventTag, taggedEvent.eventTag))
            {
                taggedEvent.onDialogStart?.Invoke(dialogText);
            }
        }
    }
    
    private void HandleTypewriterComplete(string dialogText, string eventTag)
    {
        onAnyTypewriterComplete?.Invoke(dialogText);
        
        // Check tagged events
        foreach (var taggedEvent in taggedEvents)
        {
            if (TagMatches(eventTag, taggedEvent.eventTag))
            {
                taggedEvent.onTypewriterComplete?.Invoke(dialogText);
            }
        }
    }
    
    private void HandleDialogEnd(string dialogText, string eventTag)
    {
        onAnyDialogEnd?.Invoke(dialogText);
        
        // Check tagged events
        foreach (var taggedEvent in taggedEvents)
        {
            if (TagMatches(eventTag, taggedEvent.eventTag))
            {
                taggedEvent.onDialogEnd?.Invoke(dialogText);
            }
        }
    }
    
    private bool TagMatches(string receivedTag, string listenerTag)
    {
        if (string.IsNullOrEmpty(listenerTag)) return false;
        if (string.IsNullOrEmpty(receivedTag)) return false;
        
        return receivedTag.Equals(listenerTag, System.StringComparison.OrdinalIgnoreCase);
    }
}