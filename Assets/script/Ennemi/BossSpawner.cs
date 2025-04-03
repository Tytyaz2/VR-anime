using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bossPrefab;
    public Transform spawnPoint;
    public float spawnHeight = 0.5f; // Évite le clipping avec le sol

    void Start()
    {
        SpawnBoss();
    }

    public void SpawnBoss()
    {
        if (bossPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("Boss prefab or spawn point not assigned!");
            return;
        }

        // Instantiation simple sans modification physique
        Instantiate(bossPrefab,
                   spawnPoint.position + Vector3.up * spawnHeight,
                   spawnPoint.rotation);
    }
}