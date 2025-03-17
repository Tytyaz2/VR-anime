using UnityEngine;

public class HandProjectile : MonoBehaviour
{
    public GameObject projectilePrefab; // Préfabriqué du projectile
    public float projectileSpeed = 10f; // Vitesse du projectile
    public float forwardThreshold = 0.5f; // Seuil de mouvement vers l'avant

    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movementDirection = currentPosition - previousPosition;

        // Vérifier si la main se déplace vers l'avant
        if (movementDirection.z > forwardThreshold)
        {
            LaunchProjectile();
        }

        previousPosition = currentPosition;
    }

    void LaunchProjectile()
    {
        // Instancier le projectile à la position de la main
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Définir la vitesse du projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * projectileSpeed;
        }
    }
}