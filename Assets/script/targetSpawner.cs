using UnityEngine;
using TMPro;
using System.Collections;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;  // Prefab de la cible
    public Transform player;         // Référence au joueur
    public float minDistance = 3f;   // Distance minimale de spawn
    public float maxDistance = 5f;   // Distance maximale de spawn
    public float minHeight = 1f;     // Hauteur minimale
    public float maxHeight = 3f;     // Hauteur maximale
    public TextMeshProUGUI scoreText; // Référence au texte du score

    private GameObject currentTarget = null; // Stocke la cible actuelle
    private int score = 0; // Score du joueur

    // Pour gérer la musique dans la peau
    public AudioSource audioSource;  // Référence à l'AudioSource
    public AudioClip scoreIncreaseSound; // Référence au clip audio du score

    void Start()
    {
        StartCoroutine(SpawnTargetWhenDestroyed());
    }

    IEnumerator SpawnTargetWhenDestroyed()
    {
        while (true)
        {
            // Attendre tant qu'une cible est encore présente
            while (currentTarget != null)
            {
                yield return null;
            }

            // Spawn une nouvelle cible
            SpawnTarget();

            // Attendre un petit délai pour éviter le spawn instantané
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SpawnTarget()
    {
        if (player == null || targetPrefab == null) return;

        // Définir une distance aléatoire autour du joueur (cylindrique)
        float randomDistance = Random.Range(minDistance, maxDistance);

        // Choisir un angle aléatoire autour du joueur
        float angle = Random.Range(0f, 360f);
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)); // Direction horizontale

        // Calculer la position en appliquant la distance
        Vector3 randomPosition = player.position + randomDirection * randomDistance;

        // Définir une hauteur aléatoire
        float randomHeight = Random.Range(minHeight, maxHeight);
        randomPosition.y += randomHeight;

        // Raycast pour ajuster la hauteur au sol (optionnel, si tu veux toujours coller au sol)
        if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
        {
            randomPosition = hit.point + Vector3.up * randomHeight; // Ajoute la hauteur variable
        }

        // Créer une rotation pour orienter le haut du cylindre vers le joueur
        Quaternion lookRotation = Quaternion.LookRotation(player.position - randomPosition) * Quaternion.Euler(90f, 0f, 0f);

        // Instancier la cible avec la bonne orientation
        currentTarget = Instantiate(targetPrefab, randomPosition, lookRotation);

        // Ajouter le script de destruction au clic
        Target targetScript = currentTarget.AddComponent<Target>();
        targetScript.spawner = this; // Assure l'affectation de spawner
        Debug.Log("Spawner assigned: " + targetScript.spawner);  // Debug log pour vérifier
    }

    public void TargetDestroyed(Vector3 targetPosition)
    {
        currentTarget = null; // Libère la place pour une nouvelle cible

        // Joue le son de destruction à la position de la cible
    }

    public void AddScore(int amount)
    {

                audioSource.PlayOneShot(scoreIncreaseSound);

        score += amount;
        scoreText.text = "Score: " + score;  // Met à jour l'affichage du score
    }
}
