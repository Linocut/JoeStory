using UnityEngine;
using TMPro;

public class DynamicTextFormatter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    
    void Start()
    {
        // Example of programmatically setting formatted text
        string formattedText = "Welcome to <b>JoeVR</b>! " +
                              "This is <color=blue>blue text</color> and " +
                              "this is <b><color=red>bold red text</color></b>.";
        
        textComponent.text = formattedText;
    }
    
    // Method to update text with formatting
    public void UpdateFormattedText(string baseText, string highlightWord, Color highlightColor)
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);
        string formattedText = baseText.Replace(highlightWord, 
            $"<color=#{colorHex}><b>{highlightWord}</b></color>");
        
        textComponent.text = formattedText;
    }
}