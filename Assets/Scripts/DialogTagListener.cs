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
        Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - Subscribed to dialog events. Listening for tags: {string.Join(", ", taggedEvents.ConvertAll(t => $"'{t.eventTag}'"))}");
    }
    
    void OnDisable()
    {
        DialogEventBroadcaster.OnDialogStarted -= HandleDialogStart;
        DialogEventBroadcaster.OnTypewriterCompleted -= HandleTypewriterComplete;
        DialogEventBroadcaster.OnDialogEnded -= HandleDialogEnd;
    }
    
    private void HandleDialogStart(string dialogText, string eventTag)
    {
        Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - Received Dialog Start. Event Tag: '{eventTag}'");
        
        onAnyDialogStart?.Invoke(dialogText);
        
        // Check tagged events
        foreach (var taggedEvent in taggedEvents)
        {
            Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - Checking tag '{taggedEvent.eventTag}' against received tag '{eventTag}'");
            
            if (TagMatches(eventTag, taggedEvent.eventTag))
            {
                Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - MATCH! Triggering onDialogStart for tag '{taggedEvent.eventTag}'");
                taggedEvent.onDialogStart?.Invoke(dialogText);
            }
        }
    }
    
    private void HandleTypewriterComplete(string dialogText, string eventTag)
    {
        Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - Received Typewriter Complete. Event Tag: '{eventTag}'");
        
        onAnyTypewriterComplete?.Invoke(dialogText);
        
        // Check tagged events
        foreach (var taggedEvent in taggedEvents)
        {
            if (TagMatches(eventTag, taggedEvent.eventTag))
            {
                Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - MATCH! Triggering onTypewriterComplete for tag '{taggedEvent.eventTag}'");
                taggedEvent.onTypewriterComplete?.Invoke(dialogText);
            }
        }
    }
    
    private void HandleDialogEnd(string dialogText, string eventTag)
    {
        Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - Received Dialog End. Event Tag: '{eventTag}'");
        
        onAnyDialogEnd?.Invoke(dialogText);
        
        // Check tagged events
        foreach (var taggedEvent in taggedEvents)
        {
            if (TagMatches(eventTag, taggedEvent.eventTag))
            {
                Debug.Log($"[TAG LISTENER DEBUG] {gameObject.name} - MATCH! Triggering onDialogEnd for tag '{taggedEvent.eventTag}'");
                taggedEvent.onDialogEnd?.Invoke(dialogText);
            }
        }
    }
    
    private bool TagMatches(string receivedTag, string listenerTag)
    {
        if (string.IsNullOrEmpty(listenerTag)) return false;
        if (string.IsNullOrEmpty(receivedTag)) return false;
        
        bool matches = receivedTag.Equals(listenerTag, System.StringComparison.OrdinalIgnoreCase);
        Debug.Log($"[TAG LISTENER DEBUG] Tag comparison: '{receivedTag}' vs '{listenerTag}' = {matches}");
        return matches;
    }
}