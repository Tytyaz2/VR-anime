using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections;

public class LaserShooter : MonoBehaviour
{
    public GameObject laserPrefab;
    public float laserDistanceFromHand = 0.5f;  // Distance à laquelle le laser doit commencer devant la main
    private XRHandSubsystem handSubsystem;
    public float laserRange = 10f;
    private bool canFireLaser = true;  // Permet de vérifier si le cooldown est terminé
    public float laserCooldown = 5f;  // Cooldown entre chaque utilisation du laser

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
            HandleLaserAction(subsystem.rightHand);
        }
        if (subsystem.leftHand.isTracked)
        {
            HandleLaserAction(subsystem.leftHand);
        }
    }

    private void HandleLaserAction(XRHand hand)
    {
        if (IsLaserGesture(hand) && canFireLaser)
        {
            StartCoroutine(FireLaser(hand));
        }
    }

    private bool IsLaserGesture(XRHand hand, float sensitivityThreshold = 0.12f)
    {
        if (hand == null) return false;

        bool indexOpen = IsFingerExtended(hand, XRHandJointID.IndexTip, XRHandJointID.IndexMetacarpal, sensitivityThreshold);
        bool middleOpen = IsFingerExtended(hand, XRHandJointID.MiddleTip, XRHandJointID.MiddleMetacarpal, sensitivityThreshold);
        bool ringClosed = !IsFingerExtended(hand, XRHandJointID.RingTip, XRHandJointID.RingMetacarpal, sensitivityThreshold * 0.8f);
        bool pinkyClosed = !IsFingerExtended(hand, XRHandJointID.LittleTip, XRHandJointID.LittleMetacarpal, sensitivityThreshold * 0.8f);
        bool thumbClosed = !IsFingerExtended(hand, XRHandJointID.ThumbTip, XRHandJointID.ThumbMetacarpal, sensitivityThreshold * 0.7f);

        return indexOpen && middleOpen && ringClosed && pinkyClosed && thumbClosed;
    }

    private bool IsFingerExtended(XRHand hand, XRHandJointID tip, XRHandJointID baseJoint, float threshold = 0.12f)
    {
        if (hand.GetJoint(tip).TryGetPose(out Pose tipPose) &&
            hand.GetJoint(baseJoint).TryGetPose(out Pose basePose))
        {
            return Vector3.Distance(tipPose.position, basePose.position) > threshold;
        }
        return false;
    }

    private IEnumerator FireLaser(XRHand hand)
    {
        if (laserPrefab == null) yield break;

        // Désactive la possibilité de tirer jusqu'à la fin du cooldown
        canFireLaser = false;

        if (hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose))
        {
            // Crée le laser à la position de la main mais décalé dans la direction de la main
            Vector3 laserPosition = palmPose.position + palmPose.forward * laserDistanceFromHand;  // Position décalée devant la main
            GameObject laser = Instantiate(laserPrefab, laserPosition, palmPose.rotation);

            // Utiliser un Raycast pour vérifier la collision du laser avec la cible
            RaycastHit hit;
            if (Physics.Raycast(laserPosition, palmPose.forward, out hit, laserRange))
            {
                // Le laser touche quelque chose, par exemple, vous pouvez changer la couleur du laser pour visualiser l'impact
                laser.GetComponent<Renderer>().material.color = Color.red;  // Change la couleur du laser pour indiquer un impact
                // Par exemple, si vous touchez un objet, vous pouvez aussi appliquer une logique supplémentaire ici (détruire l'objet, infliger des dégâts, etc.)
            }
            else
            {
                // Si rien n'est touché, le laser reste à sa couleur normale
                laser.GetComponent<Renderer>().material.color = Color.green;  // Laser normal
            }

            // Destroye le laser après 1 seconde
            Destroy(laser, 1f);  // Laser détruit après 1 seconde
        }

        // Attendre la fin du cooldown
        yield return new WaitForSeconds(laserCooldown);

        // Permet à nouveau de tirer le laser
        canFireLaser = true;
    }

}