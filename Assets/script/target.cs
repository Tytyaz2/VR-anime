using UnityEngine;

public class Target : MonoBehaviour
{
    public TargetSpawner spawner; // Référence au spawner pour le score

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si l'objet entrant est un projectile
        if (other.CompareTag("Projectile"))
        {
            //spawner.AddScore(10); // Ajoute 10 points au score
            Destroy(gameObject);  // Détruit la cible
           // Destroy(other.gameObject); // Détruit le projectile après impact
        }
    }
}

