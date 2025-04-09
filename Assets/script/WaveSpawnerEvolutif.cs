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

    void Start()
    {
        currentTimeBetweenWaves = baseTimeBetweenWaves;
        currentEnemiesPerWave = baseEnemiesPerWave;
        currentSpawnRate = baseSpawnRate;
        countdown = initialCountdown;
    }

    void Update()
    {
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
                SpawnEnemy();
                yield return new WaitForSeconds(currentSpawnRate);
            }
        }
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