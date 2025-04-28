using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHits = 30;
    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    [Header("Death Settings")]
    public float deathDelay = 0.5f;
    public bool destroyOnDeath = true;

    [Header("Effects")]
    public GameObject hitEffectPrefab;
    public GameObject deathEffectPrefab;
    public AudioClip hitSound;
    public AudioClip deathSound;

    private int currentHits = 0;
    public bool isDead = false;
    private AudioSource audioSource;
    private HashSet<Collider> processedHits = new HashSet<Collider>();
    private float lastHitTime;
    private const float HIT_COOLDOWN = 0.1f;
    private BossController bossController;

    private void Start()
    {
        // Configuration automatique des composants physiques
        ConfigurePhysics();
    }

    void ConfigurePhysics()
    {
        // 1. Configuration du Rigidbody pour les collisions physiques
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // 2. Configuration des Colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            // Le collider principal reste physique
            if (col.gameObject == this.gameObject)
            {
                col.isTrigger = false; // Collision physique pour le sol
            }
            // Ajoutez un collider enfant dédié aux attaques
            else
            {
                col.isTrigger = true; // Pour détecter les projectiles
            }
        }

        // 3. Création d'un collider enfant dédié aux attaques si absent
        if (transform.Find("AttackTrigger") == null)
        {
            GameObject triggerObj = new GameObject("AttackTrigger");
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;

            CapsuleCollider triggerCol = triggerObj.AddComponent<CapsuleCollider>();
            triggerCol.isTrigger = true;
            triggerCol.radius = 1.5f; // Ajustez selon la taille du boss
            triggerCol.height = 3f;
        }
    }
    

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < HIT_COOLDOWN) return;
        if (processedHits.Contains(other)) return;

        Debug.Log($"[BossHealth] Trigger with: {other.gameObject.name} (Tag: {other.tag})");

        if (other.CompareTag("Projectile") || other.CompareTag("Laser"))
        {
            processedHits.Add(other);
            lastHitTime = Time.time;

            Debug.Log("[BossHealth] Valid hit registered");
            TakeHit();

            if (other.CompareTag("Projectile"))
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20);
            }
        }
    }

    void Update()
    {
        // Nettoyage périodique des hits traités
        if (Time.frameCount % 60 == 0) // Toutes les secondes (à 60 FPS)
        {
            processedHits.RemoveWhere(col => col == null);
        }
    }

    void TakeHit()
    {
        currentHits++;
        Debug.Log($"[BossHealth] Hit count: {currentHits}/{maxHits}");

        OnHit.Invoke();

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHits >= maxHits)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        bossController = FindAnyObjectByType<BossController>();
        bossController.OnBossDeath();
        

        Debug.Log("[BossHealth] Boss defeated!");

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(50);
        }

        OnDeath.Invoke();

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, deathDelay);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}