using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TargetSpawner : MonoBehaviour
{
    public GameObject startGameButton;

    public TextMeshProUGUI messageText; // Texte pour les messages d’attente
    public string[] pauseMessages; // 3 messages différents
    private int targetsDestroyedCount = 0;
    private bool isFirstSpawn = true;

    public VideoPlayer videoPlayer;         // Le composant VideoPlayer
    public VideoClip[] pauseVideos;
    public GameObject nextSceneButton;


    public GameObject targetPrefab;  // Prefab de la cible
    public Transform player;         // Référence au joueur
    public float minDistance = 7f;   // Distance minimale de spawn
    public float maxDistance = 10f;   // Distance maximale de spawn
    public float minHeight = 2f;     // Hauteur minimale
    public float maxHeight = 5f;     // Hauteur maximale
    public TextMeshProUGUI scoreText; // Référence au texte du score
    private bool isPaused = false;

    private GameObject currentTarget = null; // Stocke la cible actuelle
    private int score = 0; // Score du joueur

    // Pour gérer la musique dans la peau
    public AudioSource audioSource;  // Référence à l'AudioSource
    public AudioClip scoreIncreaseSound; // Référence au clip audio du score

    void Start()
    {
        // Affiche le bouton de démarrage si assigné
        if (startGameButton != null)
            startGameButton.SetActive(true);
    }

    public void StartGame()
    {
        if (startGameButton != null)
            startGameButton.SetActive(false); // Cache le bouton

        StartCoroutine(SpawnTargetWhenDestroyed()); // Lance le jeu
    }


    IEnumerator SpawnTargetWhenDestroyed()
    {
        if (isFirstSpawn)
        {
            yield return ShowPauseMessage(0);
            isFirstSpawn = false;
        }

        while (true)
        {
            // Attendre que la cible soit détruite ET qu'on ne soit pas en pause
            while (currentTarget != null || isPaused)
            {
                yield return null;
            }

            targetsDestroyedCount++;

            if (targetsDestroyedCount % 5 == 0)
            {
                int msgIndex = Mathf.Clamp((targetsDestroyedCount / 5), 0, pauseMessages.Length - 1);
                yield return ShowPauseMessage(msgIndex);
            }

            SpawnTarget();

            yield return null;
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

    IEnumerator ShowPauseMessage(int messageIndex)
    {
        isPaused = true; // <<<<< PAUSE

        if (messageText != null && pauseMessages.Length > messageIndex)
        {
            messageText.text = pauseMessages[messageIndex];
            messageText.gameObject.SetActive(true);
        }

        if (videoPlayer != null && pauseVideos.Length > messageIndex)
        {
            videoPlayer.clip = pauseVideos[messageIndex];
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();
        }

        if (messageIndex == pauseMessages.Length - 1 && nextSceneButton != null)
        {
            nextSceneButton.SetActive(true);
        }

        yield return new WaitForSeconds(15f);

        // On cache les éléments après la pause
        if (messageText != null)
            messageText.gameObject.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }

        isPaused = false; // <<<<< REPRISE
    }


    public void LoadNextScene()
    {
        SceneManager.LoadScene("wave"); // Remplace par le nom exact de ta prochaine scène
    }


}


