using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class ContinuousProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootForce = 5f;
    public float projectileLifetime = 1f;
    public AudioClip shootingSound;
    public float fireRate = 0.2f; // Temps entre chaque tir

    private XRHandSubsystem handSubsystem;
    private bool isShooting = false;
    private float nextFireTime = 0f;
    private string currentHandType = "right"; // main actuelle (right ou left)

    private void Start()
    {
        handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
    }

    private void Update()
    {
        if (!isShooting || handSubsystem == null) return;

        XRHand hand = currentHandType == "right" ? handSubsystem.rightHand : handSubsystem.leftHand;
        if (hand == null || !hand.isTracked) return;

        if (Time.time >= nextFireTime)
        {
            ShootProjectileNow(hand);
            nextFireTime = Time.time + fireRate;
        }
    }

    public void ShootProjectileContinuous(string hand)
    {
        isShooting = true;
        nextFireTime = Time.time; // reset timer
        currentHandType = hand; // mémoriser quelle main on utilise
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    private void ShootProjectileNow(XRHand hand)
    {
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexPose))
        {
            Vector3 shootDirection = indexPose.forward;

            GameObject projectile = Instantiate(projectilePrefab, indexPose.position, indexPose.rotation);

            AttackData attackData = projectile.GetComponent<AttackData>();

            if (attackData != null && attackData.isUnlocked)
            {
                AudioSource audioSource = projectile.AddComponent<AudioSource>();
                if (shootingSound != null)
                {
                    audioSource.clip = shootingSound;
                    audioSource.loop = false;
                    audioSource.volume = 0.1f;
                    audioSource.time = 0.5f;
                    audioSource.Play();
                }
            }

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = shootDirection * shootForce;
                Destroy(projectile, projectileLifetime);
            }
        }
    }
}
