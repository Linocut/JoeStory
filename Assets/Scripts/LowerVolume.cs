using UnityEngine;
using System.Collections;

public class LowerVolume : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;
    
    [Header("Transition Settings")]
    public float transitionDuration = 2f;
    public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private bool isTransitioning = false;
    private Coroutine currentVolumeCoroutine;

    // Instant volume changes (original functionality)
    public void LowerAudioVolumeInstant(float amount)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Max(0f, audioSource.volume - amount);
        }
    }
    
    public void RaiseAudioVolumeInstant(float amount)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Min(1f, audioSource.volume + amount);
        }
    }
    
    // Smooth volume transitions
    public void LowerAudioVolume(float amount)
    {
        LowerAudioVolumeSmooth(amount, transitionDuration);
    }
    
    public void RaiseAudioVolume(float amount)
    {
        RaiseAudioVolumeSmooth(amount, transitionDuration);
    }
    
    public void LowerAudioVolumeSmooth(float amount, float duration = -1f)
    {
        if (audioSource == null) return;
        
        float targetVolume = Mathf.Max(0f, audioSource.volume - amount);
        TransitionToVolume(targetVolume, duration > 0 ? duration : transitionDuration);
    }
    
    public void RaiseAudioVolumeSmooth(float amount, float duration = -1f)
    {
        if (audioSource == null) return;
        
        float targetVolume = Mathf.Min(1f, audioSource.volume + amount);
        TransitionToVolume(targetVolume, duration > 0 ? duration : transitionDuration);
    }
    
    public void SetVolumeSmooth(float targetVolume, float duration = -1f)
    {
        if (audioSource == null) return;
        
        targetVolume = Mathf.Clamp01(targetVolume);
        TransitionToVolume(targetVolume, duration > 0 ? duration : transitionDuration);
    }
    
    private void TransitionToVolume(float targetVolume, float duration)
    {
        // Stop any existing transition
        if (currentVolumeCoroutine != null)
        {
            StopCoroutine(currentVolumeCoroutine);
        }
        
        currentVolumeCoroutine = StartCoroutine(VolumeTransitionCoroutine(targetVolume, duration));
    }
    
    private IEnumerator VolumeTransitionCoroutine(float targetVolume, float duration)
    {
        isTransitioning = true;
        
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;
        
        Debug.Log($"Volume transition: {startVolume:F2} -> {targetVolume:F2} over {duration:F1}s");
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // Apply animation curve for smooth transition
            float curvedProgress = volumeCurve.Evaluate(progress);
            
            // Interpolate volume
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, curvedProgress);
            
            yield return null;
        }
        
        // Ensure final volume is set exactly
        audioSource.volume = targetVolume;
        isTransitioning = false;
        currentVolumeCoroutine = null;
        
        Debug.Log($"Volume transition completed. Final volume: {targetVolume:F2}");
    }
    
    // Utility methods
    public void FadeOut(float duration = -1f)
    {
        SetVolumeSmooth(0f, duration > 0 ? duration : transitionDuration);
    }
    
    public void FadeIn(float duration = -1f)
    {
        SetVolumeSmooth(1f, duration > 0 ? duration : transitionDuration);
    }
    
    public void FadeToHalf(float duration = -1f)
    {
        SetVolumeSmooth(0.5f, duration > 0 ? duration : transitionDuration);
    }
    
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
    
    public void StopVolumeTransition()
    {
        if (currentVolumeCoroutine != null)
        {
            StopCoroutine(currentVolumeCoroutine);
            currentVolumeCoroutine = null;
            isTransitioning = false;
        }
    }
}
