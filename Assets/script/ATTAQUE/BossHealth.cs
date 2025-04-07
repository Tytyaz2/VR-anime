using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth = 10;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(int damage) {
        Debug.Log("Dégâts reçus : " + currentHealth); // Doit diminuer à chaque hit
        currentHealth -= damage;
        healthSlider.value = currentHealth; // Met à jour la barre

        if (currentHealth <= 0) {
            Debug.Log("BOSS MORT"); // Vérifie dans la Console
            Destroy(gameObject); // Ou active un effet de mort
        }
    }

    void Die()
    {
        // Effets de mort (explosion, son, etc.)
        Destroy(gameObject);
    }
}