using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections;

public class LaserShooter : MonoBehaviour
{
    public GameObject laserPrefab;  // Ton prefab de laser, doit contenir un LineRenderer
    public float laserDistanceFromHand = 0.5f;  // Distance à laquelle le laser doit commencer devant la main
    private XRHandSubsystem handSubsystem;
    public float laserRange = 10f;
    private bool canFireLaser = true;  // Permet de vérifier si le cooldown est terminé

    private void Start()
    {
        handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
    }

    public void ShootProjectile(string handType)
    {
        if (laserPrefab == null || handSubsystem == null || !canFireLaser) return;

        XRHand hand = handType == "right" ? handSubsystem.rightHand : handSubsystem.leftHand;
        if (hand == null || !hand.isTracked) return;

        // Désactive la possibilité de tirer jusqu'à la fin du cooldown
        canFireLaser = false;

        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexTipPose))
        {

            // Crée le laser au bout de l'index dans la direction de l'index
            Vector3 laserPosition = indexTipPose.position + indexTipPose.forward * laserDistanceFromHand;
            GameObject laserInstance = Instantiate(laserPrefab, laserPosition, indexTipPose.rotation);
            
            // Destroye le laser après 1 seconde
            Destroy(laserInstance, 1f);
        }

        // Permet à nouveau de tirer le laser après un délai
        StartCoroutine(RestoreCooldown());
    }

    // Coroutine pour réactiver le tir après un délai
    private IEnumerator RestoreCooldown()
    {
        yield return new WaitForSeconds(1f);  // Attente du cooldown
        canFireLaser = true;
    }
}