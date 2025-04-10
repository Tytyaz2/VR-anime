using UnityEngine;
using TMPro;

public class BossController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string defeatTrigger = "IsCloseToPlayer"; // Paramètre dans l'Animator

    [Header("Détection")]
    [SerializeField] private float defeatDistance = 5f;

    [Header("Game Over")]
    [SerializeField] private TMP_Text defeatTextUI;
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
        defeatTriggered = true;

        

        // 2. Lancer l'animation de défaite
        bossAnimator.SetBool(defeatTrigger, defeatTriggered);

        // 3. Programmer l'affichage du message après l'animation
        Invoke("ShowDefeatMessage", defeatAnimationLength);
    }

    void ShowDefeatMessage()
    {
        if (bossHealth == null || !bossHealth.isDead)
        {
            Debug.Log("Le boss vous a tué");
            waveSpawner?.StopSpawning();

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
        }
    }
    // À appeler si le boss meurt pendant l'animation
    public void OnBossDeath()
    {
        CancelInvoke("ShowDefeatMessage");
        defeatTriggered = false;
    }
}