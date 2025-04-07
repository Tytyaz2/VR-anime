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

        if (collision.gameObject.CompareTag("Player"))
        {
            // Inflige des dégâts au joueur avant de mourir
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // Dégâts ajustables
            }
            Die();
        }
        else if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Laser") )
        {
        // Gestion différenciée ennemi normal vs boss
                if (gameObject.CompareTag("Boss"))
                {

                    Debug.Log("Objet touché : " + gameObject.tag); // Doit afficher "Boss"

                    // Comportement spécifique pour le boss
                    BossHealth bossHealth = GetComponent<BossHealth>();
                    if (bossHealth != null)
                    {
                        bossHealth.TakeDamage(1); // Réduit la barre de vie de 1
                        ScoreManager.Instance.AddScore(1); // Ajoute au score
                        Destroy(collision.gameObject); // Détruit le projectile

                        // Le boss ne meurt pas immédiatement, sa barre de vie gère ça
                    }
                }
                else
                {
                    // Comportement normal pour les ennemis standards
                    ScoreManager.Instance.AddScore(1);
                    Destroy(collision.gameObject);
                    Die();
                }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Projectile"))
        {
            // Tué par projectile → Score +1
            ScoreManager.Instance.AddScore(1); // <-- On ajoute le score ICI
            Destroy(other.gameObject);
            Die(); // Appel normal de Die()
        }
        else if (other.CompareTag("Player"))
        {
            // Tué en touchant le joueur → Pas de score
            Die(); // Pas d'appel à AddScore()
        }
    }

    // Supprime l'incrémentation de score dans Die() pour éviter les doublons
    void Die()
    {
        isDead = true;
        // EFFETS DE MORT (garder tout sauf AddScore)
        if (deathEffectPrefab != null) Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        if (deathSound != null) audioSource.PlayOneShot(deathSound);
        if (destroyOnDeath) Destroy(gameObject, deathDelay);
        else gameObject.SetActive(false);
    }
}