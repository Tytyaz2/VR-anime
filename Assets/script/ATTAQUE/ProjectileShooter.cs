using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootForce = 5f;
    public float projectileLifetime = 1f;
    public AudioClip shootingSound;  // Le son de tir
    private XRHandSubsystem handSubsystem;

    private void Start()
    {
        handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
    }

    public void ShootProjectile(string handType)
    {
        if (projectilePrefab == null || handSubsystem == null) return;

        XRHand hand = handType == "right" ? handSubsystem.rightHand : handSubsystem.leftHand;
        if (hand == null || !hand.isTracked) return;

        if (hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose))
        {
            Vector3 shootDirection = Vector3.Lerp(palmPose.forward, -palmPose.up, 0.5f);
            GameObject projectile = Instantiate(projectilePrefab, palmPose.position, palmPose.rotation);

            // Ajouter un AudioSource et jouer le son de tir
            AudioSource audioSource = projectile.AddComponent<AudioSource>();
            if (shootingSound != null)
            {
                audioSource.clip = shootingSound;
                audioSource.loop = false; // Le son ne doit pas boucler
                audioSource.volume = 0.2f;  // Réduire le volume de 5 fois

                // Démarrer l'audio, mais à partir de la position 0.5 seconde du clip
                audioSource.time = 0.5f;
                audioSource.Play();
            }

            // Ajout de la physique pour la course du projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = shootDirection * shootForce;
                Destroy(projectile, projectileLifetime);
            }
        }
    }
}
