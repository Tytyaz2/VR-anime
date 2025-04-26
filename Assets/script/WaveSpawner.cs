using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Spawning Settings")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 30f;
    public int enemiesPerWave = 15;
    public float spawnRate = 1f;
    public float initialCountdown = 2f;
    public int bossWaveNumber = 3;

    [Header("UI Message Settings")]
    public TMP_Text waveMessageText;
    public float messageDisplayTime = 5f;
    public string[] techniqueMessages = {
        "Kikoha - paume vers ennemis, main ouverte (Prochaine vague : {0}s)",
        "Death Touch - paume vers sol, index pointé (Prochaine vague: {0}s)",
        "Makankosapo - paume ciel, index+majeur (Prochaine vague: {0}s)"
    };
    public string bossWarningMessage = "BOSS! Technique spéciale! (Temps restant: {0}s)";
    public string finalWaveMessage = "Entraînement terminé! (Temps total: {0}s)";

    private float countdown;
    private int waveNumber = 0;
    private Transform player;
    private List<string> remainingTechniques = new List<string>();
    private bool techniqueShownAtStart = false;
    private float totalTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null) Debug.LogError("Player not found!");

        countdown = initialCountdown;
        InitializeTechniques();
    }

    void InitializeTechniques()
    {
        remainingTechniques.Clear();
        remainingTechniques.AddRange(techniqueMessages);

        // Mélanger l'ordre des techniques
        for (int i = 0; i < remainingTechniques.Count; i++)
        {
            int randomIndex = Random.Range(i, remainingTechniques.Count);
            string temp = remainingTechniques[i];
            remainingTechniques[i] = remainingTechniques[randomIndex];
            remainingTechniques[randomIndex] = temp;
        }
    }

    void Update()
    {
        totalTime += Time.deltaTime;

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        else
        {
            countdown -= Time.deltaTime;

            if (!techniqueShownAtStart && countdown < initialCountdown - 0.5f)
            {
                techniqueShownAtStart = true;
                ShowNextTechniqueWithTime();
            }
        }
    }

    void ShowNextTechniqueWithTime()
    {
        if (remainingTechniques.Count > 0)
        {
            string message = string.Format(remainingTechniques[0], timeBetweenWaves);
            remainingTechniques.RemoveAt(0);
            StartCoroutine(ShowTimedMessage(message));
        }
    }

    IEnumerator ShowTimedMessage(string initialMessage)
    {
        float remainingTime = messageDisplayTime;
        string baseMessage = initialMessage.Split('(')[0];

        while (remainingTime > 0)
        {
            waveMessageText.text = $"{baseMessage}(Prochaine vague: {Mathf.CeilToInt(remainingTime)}s)";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        waveMessageText.text = "";
    }

    IEnumerator SpawnWave()
    {
        waveNumber++;

        if (waveNumber == bossWaveNumber)
        {
            string bossMsg = string.Format(bossWarningMessage, timeBetweenWaves);
            yield return ShowTimedMessage(bossMsg);
            SpawnBoss();
        }
        else if (waveNumber >= 4)
        {
            string finalMsg = string.Format(finalWaveMessage, Mathf.RoundToInt(totalTime));
            yield return ShowTimedMessage(finalMsg);
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnRate);
            }
            yield return new WaitForSeconds(8f);

            if (waveNumber == 1 || waveNumber == 2)
            {
                ShowNextTechniqueWithTime();
            }
        }
    }

    void SpawnBoss()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || bossPrefab == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPos = spawnPoint.position + Random.insideUnitSphere * 6f;
        spawnPos.y = 0;

        Instantiate(bossPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPos = spawnPoint.position + Random.insideUnitSphere * 6f;
        spawnPos.y = 0;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        if (player != null)
        {
            Vector3 dir = player.position - enemy.transform.position;
            dir.y = 0;
            enemy.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}