using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootForce = 5f;
    public float projectileLifetime = 1f;  // Durée de vie des projectiles (en secondes)

    private XRHandSubsystem handSubsystem;
    void Start()
    {
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader != null)
        {
            handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>(); // Obtenir le sous-système
        }
    }

    // Cette fonction sera appelée par un événement Unity lié au Hand Pose
    public void ActivateProjectileShooter(string hand)
    {
        if (handSubsystem == null) return; // Vérifier si le sous-système des mains est disponible

        if (hand == "left" && handSubsystem.leftHand.isTracked )
        {
            ShootProjectile(handSubsystem.leftHand);

        }
        else if (hand == "right" && handSubsystem.rightHand.isTracked )
        {
            ShootProjectile(handSubsystem.rightHand);

        }
    }

    private void ShootProjectile(XRHand hand)
    {
        if (projectilePrefab == null || hand == null) return;

        if (hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose))
        {
            Vector3 shootDirection = Vector3.Lerp(palmPose.forward, -palmPose.up, 0.5f);
            GameObject projectile = Instantiate(projectilePrefab, palmPose.position, palmPose.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = shootDirection * shootForce;
                Destroy(projectile, projectileLifetime); // Destroye le projectile après un délai

            }

        }
    }
}
