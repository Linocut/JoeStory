using UnityEngine;
using UnityEngine.Events;

public class StatChecker : MonoBehaviour
{
    //reference to the player "PlayerStats" script
    public PlayerStats playerStats;
    public int requiredWillPower;
    public int requiredSelfReflection;
    public UnityEvent onCanAfford;
    public UnityEvent onCannotAfford;

    public void CheckStats()
    {
        if (playerStats.CanAfford(requiredWillPower, requiredSelfReflection))
        {
            onCanAfford.Invoke();
        }
        else
        {
            onCannotAfford.Invoke();
        }
    }
    
}
