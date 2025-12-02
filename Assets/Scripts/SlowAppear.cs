using System.Collections;
using UnityEngine;

public class SlowAppear : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private bool fadeOnEnable = true;
    
    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }
    
    private void OnEnable()
    {
        if (fadeOnEnable)
        {
            StartCoroutine(FadeIn());
        }
    }
    
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        
        // Set initial transparency
        SetAlpha(0f);
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        
        // Ensure final alpha is exactly 1
        SetAlpha(1f);
    }
    
    private void SetAlpha(float alpha)
    {
        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;
            
            foreach (var material in renderer.materials)
            {
                if (material == null) continue;
                
                // Check if material has color property
                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
                
                // Check for BaseColor (URP/HDRP)
                if (material.HasProperty("_BaseColor"))
                {
                    Color color = material.GetColor("_BaseColor");
                    color.a = alpha;
                    material.SetColor("_BaseColor", color);
                }
                
                // Enable transparency rendering if needed
                if (alpha < 1f)
                {
                    EnableTransparency(material);
                }
            }
        }
    }
    
    private void EnableTransparency(Material material)
    {
        // For Standard shader
        if (material.HasProperty("_Mode"))
        {
            material.SetInt("_Mode", 3); // Transparent mode
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
        
        // For URP/HDRP shaders
        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1); // Transparent
            material.SetFloat("_Blend", 0); // Alpha blend
        }
    }
}
