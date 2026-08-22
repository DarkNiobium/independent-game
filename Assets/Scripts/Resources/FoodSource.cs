using UnityEngine;

public class FoodSource : MonoBehaviour
{
    [SerializeField] private int foodAmount = 1000;

    public int CollectFood(int requestedAmount)
    {
        if (foodAmount <= 0)
            return 0;

        int amount = Mathf.Min(requestedAmount, foodAmount);

        foodAmount -= amount;

        Debug.Log(
            "Food collected: " + amount +
            " | Remaining: " + foodAmount
        );

        return amount;
    }

    public bool HasFood()
    {
        return foodAmount > 0;
    }
}