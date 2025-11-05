using UnityEngine;
using TMPro;
using System.Collections;

public class UIAppearance : MonoBehaviour
{
    [Header("Dialog Data")]
    [SerializeField] private DialogData dialogData;
    
    [Header("UI References")]
    [SerializeField] private RectTransform uiBackground;
    [SerializeField] private RectTransform textRectTransform;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Typewriter Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float fadeOutSpeed = 0.02f;
    
    [Header("Scaling Settings")]
    [SerializeField] private Vector2 minBackgroundSize = new Vector2(200f, 100f);
    [SerializeField] private Vector2 maxBackgroundSize = new Vector2(800f, 400f);
    [SerializeField] private float padding = 20f;
    
    private string fullText;
    private string currentDisplayText = "";
    private bool isTyping = false;
    private bool isPlayingDialog = false;
    private int currentDialogIndex = 0;
    private Vector2 targetBackgroundSize;
    private Vector2 targetTextSize;
    
    void Start()
    {
        // Auto-find components if not assigned
        if (textComponent == null)
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
        
        if (textRectTransform == null && textComponent != null)
            textRectTransform = textComponent.GetComponent<RectTransform>();
            
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            
            // Create AudioSource if it doesn't exist
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2D audio for UI
            }
        }
        
        // Configure AudioSource for UI audio
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
        
        // Initialize UI
        if (textComponent != null)
            textComponent.text = "";
            
        // Auto-start dialog if enabled
        if (dialogData != null && dialogData.autoStartOnEnable)
        {
            StartDialog();
        }
    }
    
    void Update()
    {
        
    }
    
    public void StartTypewriter(string newText = "")
    {
        if (!string.IsNullOrEmpty(newText))
        {
            SetNewText(newText);
        }
        else if (dialogData != null)
        {
            StartDialog();
        }
    }
    
    private void CalculateTargetSizes()
    {
        if (textComponent == null || uiBackground == null) return;
        
        // Temporarily set full text to measure size
        string originalText = textComponent.text;
        textComponent.text = fullText;
        
        // Force text to update and get preferred size
        textComponent.ForceMeshUpdate();
        Vector2 textSize = textComponent.GetPreferredValues();
        
        // Calculate background size with padding
        targetBackgroundSize = new Vector2(
            Mathf.Clamp(textSize.x + padding * 2, minBackgroundSize.x, maxBackgroundSize.x),
            Mathf.Clamp(textSize.y + padding * 2, minBackgroundSize.y, maxBackgroundSize.y)
        );
        
        // Set text size to fit within background
        targetTextSize = new Vector2(
            targetBackgroundSize.x - padding * 2,
            targetBackgroundSize.y - padding * 2
        );
        
        // Apply sizes immediately
        uiBackground.sizeDelta = targetBackgroundSize;
        textRectTransform.sizeDelta = targetTextSize;
        
        // Restore original text
        textComponent.text = originalText;
    }
    
    // Legacy method for backwards compatibility
    public void SetNewText(string newText)
    {
        // Create a simple dialog data with one entry
        if (isPlayingDialog)
            StopDialog();
        
        fullText = newText;
        CalculateTargetSizes();
        
        // Create temporary dialog entry
        DialogEntry tempEntry = new DialogEntry();
        tempEntry.text = newText;
        tempEntry.displayDuration = 3f; // Default duration
        
        StartCoroutine(PlayDialogEntry(tempEntry));
    }
    
    // Manual size adjustment methods
    public void SetBackgroundSize(Vector2 size)
    {
        if (uiBackground != null)
            uiBackground.sizeDelta = size;
    }
    
    public void SetTextSize(Vector2 size)
    {
        if (textRectTransform != null)
            textRectTransform.sizeDelta = size;
    }
    
    // Dialog System Methods
    public void SetDialogData(DialogData newDialogData)
    {
        dialogData = newDialogData;
    }
    
    public void StartDialog()
    {
        if (dialogData == null || dialogData.GetDialogCount() == 0)
        {
            Debug.LogWarning("No dialog data assigned or dialog data is empty!");
            return;
        }
        
        if (isPlayingDialog)
            StopDialog();
            
        currentDialogIndex = 0;
        isPlayingDialog = true;
        gameObject.SetActive(true);
        StartCoroutine(PlayDialogSequence());
    }
    
    public void StopDialog()
    {
        if (isPlayingDialog)
        {
            StopAllCoroutines();
            isPlayingDialog = false;
            isTyping = false;
            
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }
    
    private IEnumerator PlayDialogSequence()
    {
        while (currentDialogIndex < dialogData.GetDialogCount())
        {
            DialogEntry currentEntry = dialogData.GetDialogEntry(currentDialogIndex);
            
            if (currentEntry != null)
            {
                yield return StartCoroutine(PlayDialogEntry(currentEntry));
                
                // Wait between entries if specified
                if (dialogData.delayBetweenEntries > 0 && currentDialogIndex < dialogData.GetDialogCount() - 1)
                {
                    yield return new WaitForSeconds(dialogData.delayBetweenEntries);
                }
            }
            
            currentDialogIndex++;
        }
        
        // Dialog sequence complete
        isPlayingDialog = false;
        
        // Loop if enabled
        if (dialogData.loopDialog)
        {
            yield return new WaitForSeconds(dialogData.delayBetweenEntries);
            StartDialog();
        }
        else if (dialogData.hideUIWhenComplete)
        {
            // Hide UI when complete
            gameObject.SetActive(false);
        }
    }
    
    private IEnumerator PlayDialogEntry(DialogEntry entry)
    {
        // Wait for the delay before starting this entry
        if (entry.delayBeforeStart > 0f)
        {
            yield return new WaitForSeconds(entry.delayBeforeStart);
        }
        
        // Trigger dialog start event via broadcaster
        DialogEventBroadcaster.TriggerDialogStart(entry.text, entry.eventTag);
        Debug.Log($"[DIALOG TAG DEBUG] Dialog Started - Event Tag: '{entry.eventTag}' | Text Preview: '{entry.text.Substring(0, Mathf.Min(20, entry.text.Length))}...'");
        
        // Set up for this entry
        fullText = entry.text;
        CalculateTargetSizes();
        
        // Play audio if available
        if (entry.audioClip != null && audioSource != null)
        {

            audioSource.Stop(); // Stop any currently playing audio
            audioSource.clip = entry.audioClip;
            audioSource.volume = entry.audioVolume;
            audioSource.pitch = 1f; // Ensure pitch is normal
            audioSource.Play();
            

        }

        
        // Start typewriter effect
        yield return StartCoroutine(TypewriterEffect(entry));
        
        // Wait for audio to finish if specified
        if (entry.waitForAudioToFinish && audioSource != null && audioSource.isPlaying)
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }
        
        // Trigger dialog end event via broadcaster
        DialogEventBroadcaster.TriggerDialogEnd(entry.text, entry.eventTag);
        Debug.Log($"[DIALOG TAG DEBUG] Dialog Ended - Event Tag: '{entry.eventTag}'");
    }
    
    private IEnumerator TypewriterEffect(DialogEntry entry)
    {
        isTyping = true;
        currentDisplayText = "";
        textComponent.text = "";
        
        // Type out each character
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentDisplayText = fullText.Substring(0, i);
            textComponent.text = currentDisplayText;
            yield return new WaitForSeconds(typeSpeed);
        }
        
        // Trigger typewriter complete event via broadcaster
        DialogEventBroadcaster.TriggerTypewriterComplete(entry.text, entry.eventTag);
        Debug.Log($"[DIALOG TAG DEBUG] Typewriter Complete - Event Tag: '{entry.eventTag}'");
        
        // Display complete text for specified duration
        yield return new WaitForSeconds(entry.displayDuration);
        
        // Fade out effect (remove characters)
        for (int i = fullText.Length; i >= 0; i--)
        {
            currentDisplayText = fullText.Substring(0, i);
            textComponent.text = currentDisplayText;
            yield return new WaitForSeconds(fadeOutSpeed);
        }
        
        isTyping = false;
    }
    
    // Public methods for external control
    public void NextDialog()
    {
        if (isPlayingDialog && currentDialogIndex < dialogData.GetDialogCount() - 1)
        {
            StopAllCoroutines();
            currentDialogIndex++;
            StartCoroutine(PlayDialogSequence());
        }
    }
    
    public void RestartDialog()
    {
        if (dialogData != null)
        {
            StopDialog();
            StartDialog();
        }
    }
    
    public bool IsDialogPlaying()
    {
        return isPlayingDialog;
    }
    
    public int GetCurrentDialogIndex()
    {
        return currentDialogIndex;
    }
    
    // Debug method to check audio setup
    public void TestAudioSetup()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is null!");
            return;
        }
        
        Debug.Log($"AudioSource Status:");
        Debug.Log($"- Enabled: {audioSource.enabled}");
        Debug.Log($"- Volume: {audioSource.volume}");
        Debug.Log($"- Mute: {audioSource.mute}");
        Debug.Log($"- Current Clip: {(audioSource.clip != null ? audioSource.clip.name : "None")}");
        Debug.Log($"- Is Playing: {audioSource.isPlaying}");
        
        // Check if dialog data has audio clips
        if (dialogData != null)
        {
            Debug.Log($"Dialog Data has {dialogData.GetDialogCount()} entries:");
            for (int i = 0; i < dialogData.GetDialogCount(); i++)
            {
                var entry = dialogData.GetDialogEntry(i);
                Debug.Log($"- Entry {i}: {(entry.audioClip != null ? entry.audioClip.name : "No audio")}");
            }
        }
    }
}
