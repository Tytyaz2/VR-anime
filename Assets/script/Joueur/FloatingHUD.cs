using UnityEngine;
using TMPro;

public class FloatingHUD : MonoBehaviour
{
    public Transform vrCamera;
    public Vector3 offset = new Vector3(0f, -0.2f, 1.5f); // Texte 20cm plus bas
    public PlayerHealth playerHealth; // Assigne ton script PlayerHealth dans l'Inspector
    public TextMeshProUGUI healthText; // Assigne le composant TextMeshPro

    void Update()
    {
        // Fait suivre le texte devant le joueur
        transform.position = vrCamera.position + vrCamera.forward * offset.z;
        transform.rotation = vrCamera.rotation;

        // Affiche les PV en temps réel
        healthText.text = $"PV: {playerHealth.currentHealth} / {playerHealth.maxHealth}";
    }
}