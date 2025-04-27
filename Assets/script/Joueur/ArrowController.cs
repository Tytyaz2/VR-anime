using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    public Transform player;        // Le joueur
    public Image arrowImage;         // L'Image de la flèche
    public float offset = 90f;       // Correction d'angle selon ton sprite (optionnel)

    void Update()
    {
        // Chercher tous les ennemis
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            arrowImage.enabled = false; // Si aucun ennemi, cacher la flèche
            return;
        }

        // Trouver l'ennemi le plus proche
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            arrowImage.enabled = true;

            // Calculer la direction entre le joueur et l'ennemi
            Vector3 dir = closestEnemy.position - player.position;
            float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            // Faire tourner la flèche
            arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, -angle + offset);
        }
    }
}
