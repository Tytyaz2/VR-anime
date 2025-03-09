using System.Collections;
using UnityEngine;
using TMPro; // Nécessaire pour le texte UI

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;  // Prefab de la cible
    public Transform player;         // Référence au joueur
    public float spawnRadius = 10f;  // Rayon de la sphère
    public float spawnInterval = 2f; // Temps entre chaque spawn
    public TextMeshProUGUI scoreText; // Référence au texte du score

    private int score = 0; // Score du joueur

    void Start()
    {
        StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        while (true)
        {
            SpawnTarget();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnTarget()
    {
        if (player == null || targetPrefab == null) return;

        // Générer une position aléatoire dans une sphère autour du joueur
        Vector3 randomPosition = player.position + Random.insideUnitSphere * spawnRadius;
        randomPosition.y = Mathf.Max(randomPosition.y, 1f); // Évite les spawns sous le sol

        // Instancier la cible
        GameObject newTarget = Instantiate(targetPrefab, randomPosition, Quaternion.identity);

        // Ajouter le script de destruction au clic
        Target targetScript = newTarget.AddComponent<Target>();
        targetScript.spawner = this; // Permet d’accéder au score
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }
}
