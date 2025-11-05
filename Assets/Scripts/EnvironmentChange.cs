using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class EnvironmentChange : MonoBehaviour
{
    [Header("Fog Transition Settings")]
    public float transitionDuration = 3f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Target Fog Settings")]
    public Color targetFogColor = Color.black;
    public float targetFogDensity = 0.1f;
    
    private bool isTransitioning = false;
    
    // Public method to start the slow fog change
    public void ChangeFogIntensity()
    {
        if (!isTransitioning)
        {
            StartCoroutine(ChangeFogSlowly());
        }
    }
    
    // Overloaded method with custom settings
    public void ChangeFogIntensity(Color newColor, float newDensity, float duration = -1f)
    {
        if (!isTransitioning)
        {
            StartCoroutine(ChangeFogSlowly(newColor, newDensity, duration > 0 ? duration : transitionDuration));
        }
    }
    
    private IEnumerator ChangeFogSlowly()
    {
        yield return StartCoroutine(ChangeFogSlowly(targetFogColor, targetFogDensity, transitionDuration));
    }
    
    private IEnumerator ChangeFogSlowly(Color newColor, float newDensity, float duration)
    {
        isTransitioning = true;
        
        // Store starting values
        Color startColor = RenderSettings.fogColor;
        float startDensity = RenderSettings.fogDensity;
        
        float elapsedTime = 0f;
        
        Debug.Log($"Starting fog transition: Color {startColor} -> {newColor}, Density {startDensity} -> {newDensity} over {duration} seconds");
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // Use animation curve for smooth transition
            float curvedProgress = transitionCurve.Evaluate(progress);
            
            // Interpolate fog color
            RenderSettings.fogColor = Color.Lerp(startColor, newColor, curvedProgress);
            
            // Interpolate fog density
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, newDensity, curvedProgress);
            
            yield return null; // Wait one frame
        }
        
        // Ensure final values are set exactly
        RenderSettings.fogColor = newColor;
        RenderSettings.fogDensity = newDensity;
        
        isTransitioning = false;
        Debug.Log("Fog transition completed");
    }
    
    // Method to reset fog to original settings
    public void ResetFog(float duration = 2f)
    {
        if (!isTransitioning)
        {
            StartCoroutine(ChangeFogSlowly(Color.gray, 0.01f, duration));
        }
    }
    
    // Method to stop current transition
    public void StopTransition()
    {
        StopAllCoroutines();
        isTransitioning = false;
    }
    
    // Check if currently transitioning
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
}
