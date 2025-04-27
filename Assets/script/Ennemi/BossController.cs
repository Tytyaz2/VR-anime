using UnityEngine;
using TMPro;

public class BossController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string defeatTrigger = "IsCloseToPlayer"; // Param�tre dans l'Animator

    [Header("D�tection")]
    [SerializeField] private float defeatDistance = 5f;

    [Header("Game Over")]
    //[SerializeField] private TMP_Text defeatTextUI;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private float defeatAnimationLength = 12.1f; // Dur�e de l'animation

    [Header("Explosion Settings")]
    public GameObject explosionPrefab;

    [Header("UI Boss Text")]
    private TextMeshProUGUI bossTextUI;

    private Transform player;
    private WaveSpawnerEvolutif waveSpawner;
    private bool defeatTriggered;
    private BossHealth bossHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        bossHealth = GetComponent<BossHealth>();
        waveSpawner = FindAnyObjectByType<WaveSpawnerEvolutif>();
        //audioSource = GetComponent<AudioSource>();

        if (defeatPanel != null)
            defeatPanel.SetActive(false);

        SoundManager.Instance.PlayAlarmMusic();

        bossTextUI = GameObject.Find("BossText")?.GetComponent<TextMeshProUGUI>();

        if (bossTextUI != null)
        {
            bossTextUI.gameObject.SetActive(true);
            bossTextUI.text = "! BOSS ARRIVÉ !";
        }
        else
        {
            Debug.LogWarning("BossText pas trouvé !");
        }
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

        SoundManager.Instance.PlayBossDanceMusic();

        // 2. Lancer l'animation de d�faite
        bossAnimator.SetBool(defeatTrigger, defeatTriggered);

        // 3. Programmer l'affichage du message apr�s l'animation
        Invoke("ShowDefeatMessage", defeatAnimationLength);

    }

    void ShowDefeatMessage()
    {
        if (bossHealth == null || !bossHealth.isDead)
        {
            Debug.Log("Le boss vous a tu�");
            if (player != null && player.TryGetComponent<PlayerHealth>(out var playerHealth))
                    {
                        playerHealth.TakeDamage(9999);
                    }

            waveSpawner?.StopSpawning();

            if (explosionPrefab != null)
            {
                // Position : 2m devant le boss et 1.5m plus haut
                Vector3 explosionPos = transform.position
                                    + transform.forward * 0.2f
                                    + Vector3.up * 3f;

                // Orientation : vers l'avant du boss (ou vers le joueur si pr�f�r�)
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
    // � appeler si le boss meurt pendant l'animation
    public void OnBossDeath()
    {
        CancelInvoke("ShowDefeatMessage");
        defeatTriggered = false;

        //Ajout du score quand le boss meurt
        ScoreManager.Instance.AddScore(50);

        SoundManager.Instance.PlayBackgroundMusic();

        if (bossTextUI != null)
        {
            bossTextUI.gameObject.SetActive(false);
        }
    }
}