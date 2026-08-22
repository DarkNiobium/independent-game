using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text populationText;

    private void Update()
    {
        if (ResourceManager.Instance == null)
            return;

        woodText.text = "WOOD: " + ResourceManager.Instance.wood;
        stoneText.text = "STONE: " + ResourceManager.Instance.stone;
        foodText.text = "FOOD: " + ResourceManager.Instance.food;

        if (PopulationManager.Instance != null)
        {
            populationText.text =
                "POPULATION: " +
                PopulationManager.Instance.GetPopulation();
        }
    }
}