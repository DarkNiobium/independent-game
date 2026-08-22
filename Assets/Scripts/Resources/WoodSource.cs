using UnityEngine;

public class WoodSource : MonoBehaviour
{
    [SerializeField] private int woodAmount = 100;

    public int CollectWood(int requestedAmount)
    {
        if (woodAmount <= 0)
            return 0;

        int amount = Mathf.Min(requestedAmount, woodAmount);

        woodAmount -= amount;

        Debug.Log(
            "Wood collected: " + amount +
            " | Remaining: " + woodAmount
        );

        return amount;
    }

    public bool HasWood()
    {
        return woodAmount > 0;
    }
}