using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class FreezeAndContinueMovement : MonoBehaviour
{
    [Header("XR Origin Reference")]
    [SerializeField] private GameObject xrOrigin;

    private LocomotionProvider[] locomotionProviders;
    private CharacterController characterController;
    private XRInteractionSimulator deviceSimulator;

    private void Awake()
    {
        if (xrOrigin != null)
        {
            // Get all locomotion providers (move, turn, teleport, etc.) from the XR Origin
            locomotionProviders = xrOrigin.GetComponentsInChildren<LocomotionProvider>();
            Debug.Log($"[FreezeMovement] Found {locomotionProviders.Length} locomotion providers");
            foreach (var provider in locomotionProviders)
            {
                Debug.Log($"[FreezeMovement] Provider: {provider.GetType().Name} on {provider.gameObject.name}");
            }
            
            // Get CharacterController if present
            characterController = xrOrigin.GetComponent<CharacterController>();
            if (characterController != null)
            {
                Debug.Log("[FreezeMovement] Found CharacterController");
            }
        }
        
        // Find XR Interaction Simulator in the scene
        deviceSimulator = FindFirstObjectByType<XRInteractionSimulator>();
        if (deviceSimulator != null)
        {
            Debug.Log($"[FreezeMovement] Found XR Interaction Simulator on {deviceSimulator.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("[FreezeMovement] XR Interaction Simulator not found in scene!");
        }
    }

    /// <summary>
    /// Freezes player movement by disabling all locomotion providers and related components
    /// </summary>
    public void FreezeMovement()
    {
        Debug.Log("[FreezeMovement] === FREEZING MOVEMENT ===");
        
        // Disable all locomotion providers
        if (locomotionProviders != null && locomotionProviders.Length > 0)
        {
            foreach (var provider in locomotionProviders)
            {
                if (provider != null && provider.enabled)
                {
                    Debug.Log($"[FreezeMovement] Disabling: {provider.GetType().Name}");
                    provider.enabled = false;
                }
            }
        }
        else
        {
            Debug.LogWarning("[FreezeMovement] No locomotion providers to disable!");
        }

        // Disable CharacterController if present
        if (characterController != null)
        {
            Debug.Log("[FreezeMovement] Disabling CharacterController");
            characterController.enabled = false;
        }

        // Disable XR Device Simulator if present
        if (deviceSimulator != null)
        {
            Debug.Log("[FreezeMovement] Disabling XR Device Simulator");
            deviceSimulator.enabled = false;
        }
        
        Debug.Log("[FreezeMovement] === FREEZE COMPLETE ===");
    }

    /// <summary>
    /// Continues player movement by enabling all locomotion providers and related components
    /// </summary>
    public void ContinueMovement()
    {
        Debug.Log("[FreezeMovement] === CONTINUING MOVEMENT ===");
        
        // Enable all locomotion providers
        if (locomotionProviders != null)
        {
            foreach (var provider in locomotionProviders)
            {
                if (provider != null)
                {
                    Debug.Log($"[FreezeMovement] Enabling: {provider.GetType().Name}");
                    provider.enabled = true;
                }
            }
        }

        // Enable CharacterController if present
        if (characterController != null)
        {
            Debug.Log("[FreezeMovement] Enabling CharacterController");
            characterController.enabled = true;
        }

        // Enable XR Device Simulator if present
        if (deviceSimulator != null)
        {
            Debug.Log("[FreezeMovement] Enabling XR Device Simulator");
            deviceSimulator.enabled = true;
        }
        
        Debug.Log("[FreezeMovement] === CONTINUE COMPLETE ===");
    }
}
