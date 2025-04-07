using UnityEngine;
using TMPro; // Ajoute cette ligne

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public AudioClip damageSound;
    [Header("UI Settings")] // Nouvelle section
    public TextMeshProUGUI healthText; // Texte pour afficher les PV

    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        UpdateHealthUI(); // Initialise l'affichage
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (damageSound != null) audioSource.PlayOneShot(damageSound);

        UpdateHealthUI(); // Met à jour l'UI après chaque dégât

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void UpdateHealthUI() // Nouvelle méthode
    {
        if (healthText != null)
            healthText.text = $"PV: {currentHealth}/{maxHealth}";
    }

    void Die()
    {
        Debug.Log("Player is dead!");
        UpdateHealthUI(); // Dernière mise à jour
        // Gère la mort du joueur
    }
}