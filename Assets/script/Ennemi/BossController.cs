using UnityEngine;
using TMPro;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string defeatTrigger = "IsCloseToPlayer"; // Paramètre dans l'Animator

    [Header("Détection")]
    [SerializeField] private float defeatDistance = 5f;

    [Header("Game Over")]
    //[SerializeField] private TMP_Text defeatTextUI;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private float defeatAnimationLength = 12.1f; // Durée de l'animation

    [Header("Explosion Settings")]
    public GameObject explosionPrefab;

    private Transform player;
    private WaveSpawnerEvolutif waveSpawner;
    private bool defeatTriggered;
    private BossHealth bossHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        bossHealth = GetComponent<BossHealth>();
        waveSpawner = FindAnyObjectByType<WaveSpawnerEvolutif>();
        // Stopper les vagues immédiatement
        waveSpawner?.StopSpawning();
        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    void Update()
    {
        if (defeatTriggered || player == null || (bossHealth != null && bossHealth.isDead)) return;

        if (Vector3.Distance(transform.position, player.position) < defeatDistance)
        {

            TriggerDefeatSequence();
        }
    }

    void TriggerDefeatSequence()
    {
        if (defeatTriggered) return; // ⛔ Empêche les appels multiples

        defeatTriggered = true;

        SoundManager.Instance.PlayBossDanceMusic();

        // Lancer l'animation de défaite
        bossAnimator.SetBool(defeatTrigger, true);

        // Afficher le message après la durée de l’anim
        Invoke(nameof(ShowDefeatMessage), defeatAnimationLength);

    }
    void ShowDefeatMessage()
    {
        if (bossHealth == null || !bossHealth.isDead)
        {
            Debug.Log("Le boss vous a tué");
            if (player != null && player.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(9999);
            }

            if (explosionPrefab != null)
            {
                // Position : 2m devant le boss et 1.5m plus haut
                Vector3 explosionPos = transform.position
                                    + transform.forward * 0.2f
                                    + Vector3.up * 3f;

                // Orientation : vers l'avant du boss (ou vers le joueur si préféré)
                Quaternion explosionRot = transform.rotation;

                GameObject explosion = Instantiate(
                    explosionPrefab,
                    explosionPos,
                    explosionRot
                );
                Destroy(explosion, 2f);

            }
            StartCoroutine(WaitAndReturnToMenu());
        }
    }

    private IEnumerator WaitAndReturnToMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Main");
    }

    // À appeler si le boss meurt pendant l'animation
    public void OnBossDeath()
    {
        CancelInvoke("ShowDefeatMessage");
        

        // Ajout du score quand le boss meurt
        ScoreManager.Instance.AddScore(40);

        SoundManager.Instance.StopAllCoroutines();
        SoundManager.Instance.PlayBackgroundMusic();


        // Relance les vagues après la mort du boss
        waveSpawner?.StartSpawning();  // Relance les vagues une fois que le boss est mort
        
    }
}
