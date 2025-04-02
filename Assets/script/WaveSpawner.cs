using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;
    public int enemiesPerWave = 5;
    public float spawnRate = 0.5f;

    private float countdown = 2f;
    private int waveNumber = 0;
    public Transform player;

    void Start()
    {
        // Initialisation du joueur dans Start()
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Vérification pour éviter les erreurs
        if (player == null)
        {
            Debug.LogError("Aucun objet avec le tag 'Player' trouvé dans la scène!");
        }
    }

    void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        waveNumber++;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnRate);
        }

        // Augmentez la difficulté si vous voulez
        enemiesPerWave += 2;
    }

    void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Aucun point de spawn défini!");
            return;
        }

        // Choisir un point de spawn aléatoire
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Calculer une position aléatoire dans une sphère de 3m autour du spawn point
        Vector3 randomOffset = Random.insideUnitSphere * 6f;
        randomOffset.y = 0; // Conserver la même hauteur (optionnel)

        // Instancier l'ennemi avec la position aléatoire
        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position + randomOffset,
            Quaternion.identity
        );

        // Faire regarder l'ennemi vers le joueur
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - enemy.transform.position;
            directionToPlayer.y = 0; // Garder la rotation seulement sur l'axe Y
            enemy.transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }
    }
}