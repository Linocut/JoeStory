using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[System.Serializable]
public class LightingDebugger : MonoBehaviour
{
    [Header("Debug Options")]
    public bool debugOnStart = true;
    public bool autoFixCommonIssues = true;
    
    [Header("Manual Light Testing")]
    public Light testLight;
    
    void Start()
    {
        if (debugOnStart)
        {
            DebugLightingSetup();
        }
        
        if (autoFixCommonIssues)
        {
            FixCommonLightingIssues();
        }
    }
    
    [ContextMenu("Debug Lighting Setup")]
    public void DebugLightingSetup()
    {
        Debug.Log("=== LIGHTING DEBUG REPORT ===");
        
        // Check Render Pipeline
        var pipeline = GraphicsSettings.currentRenderPipeline;
        if (pipeline != null)
        {
            Debug.Log($"Render Pipeline: {pipeline.GetType().Name}");
            
            if (pipeline is UniversalRenderPipelineAsset urpAsset)
            {
                Debug.Log("✓ Using Universal Render Pipeline");
                // Note: Some URP settings are not accessible via script in newer versions
                Debug.Log("Check URP Asset settings manually in Project Settings > Graphics");
            }
        }
        else
        {
            Debug.LogWarning("⚠ Using Built-in Render Pipeline - Point/Spot lights should work");
        }
        
        // Check Scene Lights
        Light[] allLights = FindObjectsOfType<Light>();
        Debug.Log($"Found {allLights.Length} lights in scene:");
        
        foreach (Light light in allLights)
        {
            Debug.Log($"- {light.name}: Type={light.type}, Enabled={light.enabled}, " +
                     $"Intensity={light.intensity}, Range={light.range}, RenderMode={light.renderMode}");
            
            if (light.type != LightType.Directional)
            {
                if (!light.enabled)
                    Debug.LogWarning($"⚠ Light '{light.name}' is DISABLED");
                if (light.intensity <= 0)
                    Debug.LogWarning($"⚠ Light '{light.name}' has zero intensity");
                if (light.range <= 0 && light.type != LightType.Directional)
                    Debug.LogWarning($"⚠ Light '{light.name}' has zero range");
            }
        }
        
        // Check Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            var cameraData = mainCam.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                Debug.Log($"Camera Render Type: {cameraData.renderType}");
            }
        }
        
        Debug.Log("=== END LIGHTING DEBUG ===");
    }
    
    [ContextMenu("Fix Common Issues")]
    public void FixCommonLightingIssues()
    {
        Debug.Log("Attempting to fix common lighting issues...");
        
        Light[] allLights = FindObjectsOfType<Light>();
        
        foreach (Light light in allLights)
        {
            if (light.type != LightType.Directional)
            {
                // Fix common light issues
                if (light.intensity <= 0)
                {
                    light.intensity = 1f;
                    Debug.Log($"Fixed intensity for {light.name}");
                }
                
                if (light.range <= 0)
                {
                    light.range = 10f;
                    Debug.Log($"Fixed range for {light.name}");
                }
                
                // Ensure render mode is appropriate for URP
                if (light.renderMode == LightRenderMode.Auto)
                {
                    // Auto is usually fine, but you can force Important for testing
                    // light.renderMode = LightRenderMode.ForcePixel;
                }
            }
        }
        
        // Check and fix materials
        FixMaterialShaders();
    }
    
    private void FixMaterialShaders()
    {
        Debug.Log("Checking materials for URP compatibility...");
        
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material != null && material.shader != null)
                {
                    string shaderName = material.shader.name;
                    
                    // Check for Built-in RP shaders that won't work with URP
                    if (shaderName.Contains("Standard") && !shaderName.Contains("Universal"))
                    {
                        Debug.LogWarning($"⚠ Material '{material.name}' uses Built-in shader: {shaderName}");
                        Debug.LogWarning($"  → Should use 'Universal Render Pipeline/Lit' instead");
                    }
                }
            }
        }
    }
    
    [ContextMenu("Create Test Light")]
    public void CreateTestLight()
    {
        GameObject lightGO = new GameObject("Test Point Light");
        lightGO.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        
        Light testLight = lightGO.AddComponent<Light>();
        testLight.type = LightType.Point;
        testLight.intensity = 2f;
        testLight.range = 10f;
        testLight.color = Color.white;
        testLight.renderMode = LightRenderMode.ForcePixel; // Force important for testing
        
        Debug.Log("Created test point light. If you can't see it, there's a URP configuration issue.");
    }
}