using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class DialogEntry
{
    [Header("Dialog Content")]
    [TextArea(3, 6)]
    public string text;
    
    [Header("Timing")]
    public float delayBeforeStart = 0f;
    public float displayDuration = 3f;
    
    [Header("Audio")]
    public AudioClip audioClip;
    
    [Header("Optional Settings")]
    [Range(0f, 1f)]
    public float audioVolume = 1f;
    public bool waitForAudioToFinish = false;
    
    [Header("Event Tags (for script listeners)")]
    [Space(5)]
    public string eventTag = "";
    [Tooltip("Custom data to pass with events")]
    public string eventData = "";
}

[CreateAssetMenu(fileName = "New Dialog Data", menuName = "Dialog System/Dialog Data")]
public class DialogData : ScriptableObject
{
    [Header("Dialog Sequence")]
    public List<DialogEntry> dialogEntries = new List<DialogEntry>();
    
    [Header("Global Settings")]
    public bool loopDialog = false;
    public float delayBetweenEntries = 0.5f;
    
    [Header("UI Settings")]
    public bool autoStartOnEnable = true;
    public bool hideUIWhenComplete = true;
    
    public int GetDialogCount()
    {
        return dialogEntries.Count;
    }
    
    public DialogEntry GetDialogEntry(int index)
    {
        if (index >= 0 && index < dialogEntries.Count)
            return dialogEntries[index];
        return null;
    }
    
    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < dialogEntries.Count;
    }
}