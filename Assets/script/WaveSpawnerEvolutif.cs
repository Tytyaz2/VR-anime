using System.Collections;
using UnityEngine;

public class WaveSpawnerEvolutif : MonoBehaviour
{
    [Header("Base Settings")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;
    public float baseTimeBetweenWaves = 30f;
    public int baseEnemiesPerWave = 15;
    public float baseSpawnRate = 1f;
    public float initialCountdown = 2f;
    public int bossWaveInterval = 3;

    [Header("Difficulty Scaling")]
    public float healthMultiplierPerBoss = 1.5f;
    public float damageMultiplierPerBoss = 1.3f;
    public float speedMultiplierPerBoss = 1.1f;
    public int extraEnemiesPerBoss = 3;
    public float spawnRateReduction = 0.1f;

    private float countdown;
    private int waveNumber = 0;
    private int bossTier = 0;
    private float currentTimeBetweenWaves;
    private int currentEnemiesPerWave;
    private float currentSpawnRate;
    private bool isSpawning = true; // Nouvelle variable pour contrôler le spawn

    void Start()
    {
        currentTimeBetweenWaves = baseTimeBetweenWaves;
        currentEnemiesPerWave = baseEnemiesPerWave;
        currentSpawnRate = baseSpawnRate;
        countdown = initialCountdown;
    }

    void Update()
    {
        if (!isSpawning) return; // Ne rien faire si le spawn est arrêté

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = currentTimeBetweenWaves;
        }
        else
        {
            countdown -= Time.deltaTime;
        }
    }

    IEnumerator SpawnWave()
    {
        if (!isSpawning) yield break; // Sortir si le spawn est arrêté

        waveNumber++;
        bool isBossWave = waveNumber % bossWaveInterval == 0;

        if (isBossWave)
        {
            bossTier++;
            SpawnBoss();
            UpgradeEnemyStats();
        }
        else
        {
            for (int i = 0; i < currentEnemiesPerWave; i++)
            {
                if (!isSpawning) yield break; // Sortir si le spawn est arrêté pendant la vague
                SpawnEnemy();
                yield return new WaitForSeconds(currentSpawnRate);
            }
        }
    }

    // Fonction pour arrêter le spawn
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines(); // Arrête toutes les coroutines en cours (comme SpawnWave)
        Debug.Log("Spawn des vagues arrêté");
    }

    void UpgradeEnemyStats()
    {
        currentEnemiesPerWave += extraEnemiesPerBoss;
        currentSpawnRate = Mathf.Max(0.2f, currentSpawnRate - spawnRateReduction);
    }

    void SpawnBoss()
    {
        if (spawnPoints.Length == 0 || bossPrefab == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        ApplyScaling(boss, bossTier);
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        ApplyScaling(enemy, bossTier);
    }

    void ApplyScaling(GameObject enemy, int tier)
    {
        // Santé
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.maxHealth *= Mathf.Pow(healthMultiplierPerBoss, tier);
            health.currentHealth = health.maxHealth;
        }
    }
}