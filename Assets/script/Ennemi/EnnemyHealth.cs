using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100;
    [SerializeField] public float currentHealth;
    public bool invincible = false;

    [Header("Health Bar")]
    public HealthBarUI healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);
    private HealthBarUI healthBar;

    [Header("Death Settings")]
    public UnityEvent OnDeath;
    public float deathDelay = 0.1f;
    public bool destroyOnDeath = true;

    [Header("Effects")]
    public GameObject hitEffectPrefab;
    public GameObject deathEffectPrefab;

    [Header("Audio Settings")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    [Range(0, 1)] public float hitSoundVolume = 0.7f;
    [Range(0, 1)] public float deathSoundVolume = 1f;
    [Range(0, 0.5f)] public float hitPitchRandomization = 0.1f;
    public float sound3DBlend = 1f;

    [Header("Colliders")]
    public Collider physicsCollider;
    public Collider hurtbox;

    private AudioSource audioSource;
    private bool isDead = false;

    private PlayerStats playerStats;

    private BossController bossController;

    void Awake()
    {
        currentHealth = maxHealth;
        ConfigureAudioSource();
    }

    void Start()
    {

        // Configuration des colliders
        if (physicsCollider == null) physicsCollider = GetComponent<Collider>();
        if (hurtbox == null) CreateHurtbox();

        physicsCollider.isTrigger = false;
        hurtbox.isTrigger = true;

        // Configuration Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (healthBarPrefab != null)
        {
            healthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBar.Initialize(transform);
            healthBar.UpdateHealth(1f);
        }
    }

    void ConfigureAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = sound3DBlend;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.UpdateHealth((float)currentHealth / maxHealth);
    }

    void CreateHurtbox()
    {
        GameObject hurtboxObj = new GameObject("Hurtbox");
        hurtboxObj.transform.SetParent(transform);
        hurtboxObj.transform.localPosition = Vector3.zero;
        hurtbox = hurtboxObj.AddComponent<BoxCollider>();
        hurtbox.isTrigger = true;
        ((BoxCollider)hurtbox).size = Vector3.one * 1.2f;
    }

    public void TakeDamage(int damage, Vector3 hitDirection = default)
    {
        if (isDead || invincible) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        
        if (currentHealth > 0f)
            PlayHitSound(hitDirection);

        if (hitDirection != default && TryGetComponent<Rigidbody>(out var rb))
            rb.AddForce(hitDirection.normalized * 5f, ForceMode.Impulse);

        if (currentHealth <= 0)
        {
            //Ajout du score quand un petit monstre meurt
            ScoreManager.Instance.AddScore(10);
            Die();
        }
    }

    void PlayHitSound(Vector3 hitDirection)
    {
        if (hitSound != null)
        {
            Vector3 soundPosition = transform.position;
            if (hitDirection != default)
            {
                soundPosition += hitDirection.normalized * 0.5f;
            }

            GameObject soundObj = new GameObject("TempAudio");
            soundObj.transform.position = soundPosition;
            AudioSource tempSource = soundObj.AddComponent<AudioSource>();

            // Configuration audio
            tempSource.pitch = Random.Range(1f - hitPitchRandomization, 1f + hitPitchRandomization);
            tempSource.spatialBlend = sound3DBlend;
            tempSource.volume = hitSoundVolume;

            // Joue seulement le début du son (0.5s max)
            float playDuration = Mathf.Min(0.5f, hitSound.length);
            tempSource.clip = hitSound;
            tempSource.time = 0f; // Commence au début
            tempSource.Play();

            // Coupe le son après 0.5s et détruit l'objet
            Destroy(soundObj, playDuration);
        }
    }

    void Die()
    {
        isDead = true;

        if (physicsCollider != null) physicsCollider.enabled = false;
        if (hurtbox != null) hurtbox.enabled = false;



        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        PlayDeathSound();

        if (healthBar != null)
            Destroy(healthBar.gameObject);

       if (CompareTag("Boss"))
               {
                   bossController = GetComponent<BossController>();
                   bossController.OnBossDeath();

               }

       if (destroyOnDeath)
            Destroy(gameObject, deathDelay);
        else
            gameObject.SetActive(false);
    }

    void HitPlayer()
    {
        isDead = true;

        if (physicsCollider != null) physicsCollider.enabled = false;
        if (hurtbox != null) hurtbox.enabled = false;

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);


        if (healthBar != null)
            Destroy(healthBar.gameObject);

        if (destroyOnDeath)
            Destroy(gameObject, deathDelay);
        else
            gameObject.SetActive(false);
    }

    void PlayDeathSound()
    {
        if (deathSound != null && hitSound != null)
        {
            GameObject soundObj = new GameObject("TempAudio");
            soundObj.transform.position = transform.position;
            AudioSource tempSource = soundObj.AddComponent<AudioSource>();
            float playDuration = Mathf.Min(0.5f, hitSound.length);
            tempSource.spatialBlend = sound3DBlend;
            tempSource.volume = deathSoundVolume;
           
            tempSource.PlayOneShot(deathSound);

            Destroy(soundObj, playDuration);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.TryGetComponent<AttackData>(out var attack))
        {
            TakeDamage(attack.damage, transform.position - other.transform.position);
            if (attack.destroyOnImpact)
                Destroy(other.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(10); // tu peux changer le "10" si besoin
            }
         // 👇 Fait disparaître l'ennemi directement après contact
             if (!isDead)
            {
               HitPlayer();
            }
        }
    }
}