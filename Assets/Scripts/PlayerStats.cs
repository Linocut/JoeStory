using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int willPower;
    public int selfReflection;




    public void IncreaseWillPower(int amount)
    {
        willPower += amount;
    }
    public void DecreaseWillPower(int amount)
    {
        willPower -= amount;
    }
    public void IncreaseSelfReflection(int amount)
    {
        selfReflection += amount;
    }
    public void DecreaseSelfReflection(int amount)
    {
        selfReflection -= amount;
    }

    public bool CanAfford(int willPowerCost, int selfReflectionCost)
    {
        return willPower >= willPowerCost && selfReflection >= selfReflectionCost;
    }




}
