using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int wood = 100;
    public int stone = 50;
    public int food = 50;

    private void Awake()
    {
        Instance = this;
    }

    public void AddWood(int amount)
    {
        wood += amount;
    }

    public bool SpendWood(int amount)
    {
        if (wood < amount)
            return false;

        wood -= amount;
        return true;
    }

    public void AddStone(int amount)
    {
        stone += amount;
    }

    public bool SpendStone(int amount)
    {
        if (stone < amount)
            return false;

        stone -= amount;
        return true;
    }
    public void AddFood(int amount)
    {
        food += amount;
    }

    public bool SpendFood(int amount)
    {
        if (food < amount)
            return false;

        food -= amount;
        return true;
    }
    public bool CanAfford(int woodCost, int stoneCost, int foodCost = 0)
    {
        return wood >= woodCost &&
               stone >= stoneCost &&
               food >= foodCost;
    }

    public bool SpendResources(int woodCost, int stoneCost, int foodCost = 0)
    {
        if (!CanAfford(woodCost, stoneCost, foodCost))
        {
            Debug.Log("Not enough resources!");
            return false;
        }

        wood -= woodCost;
        stone -= stoneCost;
        food -= foodCost;

        return true;
    }
}