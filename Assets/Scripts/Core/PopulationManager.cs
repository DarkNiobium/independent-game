using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    [SerializeField] private int population = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPopulation(int amount)
    {
        population += amount;

        Debug.Log("Population: " + population);
    }

    public int GetPopulation()
    {
        return population;
    }
}