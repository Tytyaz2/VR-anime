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
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Colliders")]
    public Collider physicsCollider;
    public Collider hurtbox;

    private AudioSource audioSource;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
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
            healthBar.UpdateHealth(1f); // 100% au départ
        }
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

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (hitDirection != default && TryGetComponent<Rigidbody>(out var rb))
            rb.AddForce(hitDirection.normalized * 5f, ForceMode.Impulse);

        if (currentHealth <= 0)
            Die();
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
    }

    void Die()
    {
        isDead = true;
        OnDeath.Invoke();

        if (physicsCollider != null) physicsCollider.enabled = false;
        if (hurtbox != null) hurtbox.enabled = false;

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Correction ici - Destruction de la barre de vie
        if (healthBar != null)
            Destroy(healthBar.gameObject); // On détruit l'objet gameObject associé

        if (destroyOnDeath)
            Destroy(gameObject, deathDelay);
        else
            gameObject.SetActive(false);
        if (healthBar != null)
            Destroy(healthBar.gameObject);
    }
}