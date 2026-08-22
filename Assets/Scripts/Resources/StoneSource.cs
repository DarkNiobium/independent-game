using UnityEngine;

public class StoneSource : MonoBehaviour
{
    [SerializeField] private int stoneAmount = 1000;

    public int CollectStone(int requestedAmount)
    {
        if (stoneAmount <= 0)
            return 0;

        int amount = Mathf.Min(requestedAmount, stoneAmount);

        stoneAmount -= amount;

        Debug.Log(
            "Stone collected: " + amount +
            " | Remaining: " + stoneAmount
        );

        return amount;
    }

    public bool HasStone()
    {
        return stoneAmount > 0;
    }
}