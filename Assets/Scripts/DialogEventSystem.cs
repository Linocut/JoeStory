using UnityEngine;
using UnityEngine.Events;
using System;

public static class DialogEventBroadcaster
{
    // Static events that any script can subscribe to
    public static event Action<string, string> OnDialogStarted; // dialogText, eventTag
    public static event Action<string, string> OnTypewriterCompleted; // dialogText, eventTag
    public static event Action<string, string> OnDialogEnded; // dialogText, eventTag
    
    // Methods called by the dialog system
    public static void TriggerDialogStart(string dialogText, string eventTag = "")
    {
        Debug.Log($"[BROADCASTER DEBUG] Broadcasting Dialog Start - Tag: '{eventTag}' | Text: '{dialogText.Substring(0, Mathf.Min(30, dialogText.Length))}...'");
        OnDialogStarted?.Invoke(dialogText, eventTag);
    }
    
    public static void TriggerTypewriterComplete(string dialogText, string eventTag = "")
    {
        Debug.Log($"[BROADCASTER DEBUG] Broadcasting Typewriter Complete - Tag: '{eventTag}'");
        OnTypewriterCompleted?.Invoke(dialogText, eventTag);
    }
    
    public static void TriggerDialogEnd(string dialogText, string eventTag = "")
    {
        Debug.Log($"[BROADCASTER DEBUG] Broadcasting Dialog End - Tag: '{eventTag}'");
        OnDialogEnded?.Invoke(dialogText, eventTag);
    }
}

// Component to easily subscribe to dialog events in the Inspector
public class DialogEventListener : MonoBehaviour
{
    [Header("Dialog Event Responses")]
    public UnityEvent<string> OnDialogStart;
    public UnityEvent<string> OnTypewriterComplete;
    public UnityEvent<string> OnDialogEnd;
    
    [Header("Filter Settings (Optional)")]
    public bool filterByText = false;
    [TextArea(2, 4)]
    public string textToMatch = "";
    
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
        if (ShouldTrigger(dialogText))
            OnDialogStart?.Invoke(dialogText);
    }
    
    private void HandleTypewriterComplete(string dialogText, string eventTag)
    {
        if (ShouldTrigger(dialogText))
            OnTypewriterComplete?.Invoke(dialogText);
    }
    
    private void HandleDialogEnd(string dialogText, string eventTag)
    {
        if (ShouldTrigger(dialogText))
            OnDialogEnd?.Invoke(dialogText);
    }
    
    private bool ShouldTrigger(string dialogText)
    {
        if (!filterByText) return true;
        return dialogText.Contains(textToMatch);
    }
}