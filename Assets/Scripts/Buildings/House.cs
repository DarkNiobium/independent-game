using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private int houseCapacity = 5;

    [Header("Worker Spawn")]
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private Vector3 workerSpawnOffset = new Vector3(2f, 0f, 0f);

    private void Start()
    {
        if (PopulationManager.Instance != null)
        {
            PopulationManager.Instance.AddPopulation(houseCapacity);

            Debug.Log("House added population: " + houseCapacity);
        }
        else
        {
            Debug.LogError("PopulationManager not found!");
        }

        SpawnWorker();
    }

    private void SpawnWorker()
    {
        if (workerPrefab == null)
        {
            Debug.LogError("Worker Prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition =
            transform.position + workerSpawnOffset;

        Instantiate(
            workerPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Debug.Log("New worker spawned!");
    }
}