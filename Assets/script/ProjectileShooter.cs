using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootForce = 5f;
    public float projectileLifetime = 1f;  // Durée de vie des projectiles (en secondes)
    private XRHandSubsystem handSubsystem;
    private bool hasShotRight = false;
    private bool hasShotLeft = false;

    void Start()
    {
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader != null)
        {
            handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();

            if (handSubsystem != null)
            {
                handSubsystem.updatedHands += OnHandUpdated;
            }
        }
    }

    private void OnDestroy()
    {
        if (handSubsystem != null)
        {
            handSubsystem.updatedHands -= OnHandUpdated;
        }
    }

    private void OnHandUpdated(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags successFlags, XRHandSubsystem.UpdateType updateType)
    {
        if (subsystem.rightHand.isTracked)
        {
            HandleHandAction(subsystem.rightHand, ref hasShotRight);
        }
        if (subsystem.leftHand.isTracked)
        {
            HandleHandAction(subsystem.leftHand, ref hasShotLeft);
        }
    }

    private void HandleHandAction(XRHand hand, ref bool hasShot)
    {
        if (IsHandOpen(hand) && !hasShot)
        {
            ShootProjectile(hand);
            hasShot = true;
        }
        else if (!IsHandOpen(hand))
        {
            hasShot = false;  // Réinitialise le tir quand la main revient à une position neutre
        }
    }

    private bool IsHandOpen(XRHand hand, float openThreshold = 0.12f)
    {
        if (hand == null) return false;

        XRHandJointID[] fingerTips = {
           XRHandJointID.IndexTip,
           XRHandJointID.MiddleTip,
           XRHandJointID.RingTip,
           XRHandJointID.LittleTip,
           XRHandJointID.ThumbTip
        };

        XRHandJointID[] fingerBases = {
           XRHandJointID.IndexMetacarpal,
           XRHandJointID.MiddleMetacarpal,
           XRHandJointID.RingMetacarpal,
           XRHandJointID.LittleMetacarpal,
           XRHandJointID.ThumbMetacarpal
        };

        float totalDistance = 0f;
        int validJoints = 0;

        for (int i = 0; i < fingerTips.Length; i++)
        {
            if (hand.GetJoint(fingerTips[i]).TryGetPose(out Pose tipPose) &&
                hand.GetJoint(fingerBases[i]).TryGetPose(out Pose basePose))
            {
                float distance = Vector3.Distance(tipPose.position, basePose.position);
                if (distance > openThreshold) // Si le doigt est suffisamment étendu
                {
                    totalDistance += distance;
                    validJoints++;
                }
            }
        }

        return validJoints >= 4; // Ajustez ce seuil selon vos besoins
    }

    private void ShootProjectile(XRHand hand)
    {
        if (projectilePrefab == null) return;

        if (hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose))
        {
            Vector3 shootDirection = Vector3.Lerp(palmPose.forward, -palmPose.up, 0.5f);
            GameObject projectile = Instantiate(projectilePrefab, palmPose.position, palmPose.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = shootDirection * shootForce;
                Destroy(projectile, projectileLifetime); // Destroye le projectile
            }
        }
    }
}
