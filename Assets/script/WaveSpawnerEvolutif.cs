using System.Collections;
using TMPro;
using UnityEngine;

public class WaveSpawnerEvolutif : MonoBehaviour
{
    [Header("Base Settings")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;
    public float baseTimeBetweenWaves = 10f;
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

    private int waveNumber = 0;
    private int bossTier = 0;
    private float currentSpawnRate;
    private int currentEnemiesPerWave;
    private bool isSpawning = true;

    [Header("UI Boss Text")]
    private TextMeshProUGUI bossTextUI;

    void Start()
    {
        currentEnemiesPerWave = baseEnemiesPerWave;
        currentSpawnRate = baseSpawnRate;

        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        // Attente initiale
        yield return new WaitForSeconds(initialCountdown);

        while (isSpawning)
        {
            yield return StartCoroutine(SpawnWave());

            // Tu peux ajuster ici dynamiquement le temps entre les vagues si tu veux :
            float adjustedTimeBetweenWaves = Mathf.Max(2f, baseTimeBetweenWaves + currentEnemiesPerWave * 0.2f);

            Debug.Log($"Vague {waveNumber} terminée. Prochaine dans {adjustedTimeBetweenWaves} secondes.");

            yield return new WaitForSeconds(adjustedTimeBetweenWaves);
        }
    }

    IEnumerator SpawnWave()
    {
        if (!isSpawning) yield break;

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
                if (!isSpawning) yield break;

                SpawnEnemy();
                yield return new WaitForSeconds(currentSpawnRate);
            }
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        ApplyScaling(enemy, bossTier);
    }

    void SpawnBoss()
    {
        SoundManager.Instance.PlayAlarmMusic();

        // Trouve le texte du boss et l'affiche
        bossTextUI = GameObject.Find("BossText")?.GetComponent<TextMeshProUGUI>();

        if (bossTextUI != null)
        {
            bossTextUI.gameObject.SetActive(true);
            bossTextUI.text = "! LE BOSS ARRIVE !";
        }
        else
        {
            Debug.LogWarning("BossText pas trouvé !");
        }

        // Lance une coroutine pour attendre 3 secondes avant d'instancier le boss
        StartCoroutine(WaitAndSpawnBoss());
    }

    // Coroutine qui attend 3 secondes avant d'instancier le boss
    IEnumerator WaitAndSpawnBoss()
    {
        yield return new WaitForSeconds(3f); // Attends 3 secondes

        // Si aucun point de spawn ou prefab de boss n'est assigné, on arrête la fonction
        if (spawnPoints.Length == 0 || bossPrefab == null) yield break;

        // Sélectionne un point de spawn aléatoire
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Crée le boss à cet endroit
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

        // Applique les changements de statistiques en fonction du niveau du boss
        ApplyScaling(boss, bossTier);

    }

    void ApplyScaling(GameObject enemy, int tier)
    {
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.maxHealth *= Mathf.Pow(healthMultiplierPerBoss, tier);
            health.currentHealth = health.maxHealth;
        }

        // Tu peux aussi ajuster la vitesse ou d’autres stats ici
    }

    void UpgradeEnemyStats()
    {
        currentEnemiesPerWave += extraEnemiesPerBoss;
        currentSpawnRate = Mathf.Max(0.2f, currentSpawnRate - spawnRateReduction);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
        Debug.Log("Spawn des vagues arrêté.");
    }
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;

            // Trouve le texte du boss et l'affiche
            bossTextUI = GameObject.Find("BossText")?.GetComponent<TextMeshProUGUI>();

            if (bossTextUI != null)
            {
                bossTextUI.gameObject.SetActive(false);
            }
            // Ne réinitialise pas les variables, on reprend là où on était
            StartCoroutine(WaveLoop());  // Relance la boucle des vagues sans réinitialiser les variables
            Debug.Log("Spawn des vagues redémarré.");
        }


    }
}
