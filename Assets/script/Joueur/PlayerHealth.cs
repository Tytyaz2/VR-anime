using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public AudioClip damageSound;

    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        SoundManager.Instance.PlayBackgroundMusic();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (damageSound != null) audioSource.PlayOneShot(damageSound, 0.1f); // Volume entre 0.0 et 1.0

        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead!");
        // Gère la mort du joueur (Game Over, respawn, etc.)
    }
}