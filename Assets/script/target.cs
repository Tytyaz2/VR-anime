using UnityEngine;

public class Target : MonoBehaviour
{
    public TargetSpawner spawner; // Référence au spawner pour le score

    void OnMouseDown()
    {
        spawner.AddScore(10); // Ajoute 10 points par cible
        Destroy(gameObject);  // Détruit la cible
    }
}
