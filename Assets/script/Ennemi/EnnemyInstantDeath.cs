using UnityEngine;

public class EnemyInstantDeath : MonoBehaviour
{
    [Header("Death Settings")]
    public float deathDelay = 0.1f; // Petit d�lai pour les effets
    public bool destroyOnDeath = true;

    [Header("Effects")]
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;

    private bool isDead = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        // D�tection du joueur ou projectile
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Laser"))
        {
            Die();

            // D�truire le projectile s'il en est un
            if (collision.gameObject.CompareTag("Projectile"))
            {
                Destroy(collision.gameObject);
            }
        }
    }

    // Version alternative pour les triggers
    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") || other.CompareTag("Projectile"))
        {
            Die();

            // D�truire le projectile s'il en est un
            if (other.CompareTag("Projectile"))
            {
                Destroy(other.gameObject);
            }
        }
    }

    void Die()
    {
        isDead = true;

        // Effets de mort
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Destruction ou d�sactivation
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