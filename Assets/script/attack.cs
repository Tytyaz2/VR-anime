using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections;

public class LaserShooter2   : MonoBehaviour
{
    public GameObject laserPrefab;
    public float laserDistanceFromHand = 0.5f;
    public float laserRange = 10f;
    public float laserCooldown = 5f;

    private bool canFireLaserLeft = true;
    private bool canFireLaserRight = true;
    private XRHandSubsystem handSubsystem; // Stocke l'instance du sous-système des mains

    void Start()
    {
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader != null)
        {
            handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>(); // Obtenir le sous-système
        }
    }

    // Cette fonction sera appelée par un événement Unity lié au Hand Pose
    public void ActivateLaser(string hand)
    {
        if (handSubsystem == null) return; // Vérifier si le sous-système des mains est disponible

        if (hand == "left" && canFireLaserLeft && handSubsystem.leftHand.isTracked)
        {
            StartCoroutine(FireLaser(handSubsystem.leftHand, "left"));
        }
        else if (hand == "right" && canFireLaserRight && handSubsystem.rightHand.isTracked)
        {
            StartCoroutine(FireLaser(handSubsystem.rightHand, "right"));
        }
    }

    private IEnumerator FireLaser(XRHand hand, string handSide)
    {
        if (laserPrefab == null || hand == null) yield break;

        if (handSide == "left") canFireLaserLeft = false;
        if (handSide == "right") canFireLaserRight = false;

        if (hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose))
        {
            Vector3 laserPosition = palmPose.position + palmPose.forward * laserDistanceFromHand;
            GameObject laser = Instantiate(laserPrefab, laserPosition, palmPose.rotation);

            RaycastHit hit;
            if (Physics.Raycast(laserPosition, palmPose.forward, out hit, laserRange))
            {
                laser.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                laser.GetComponent<Renderer>().material.color = Color.green;
            }

            Destroy(laser, 1f);
        }

        yield return new WaitForSeconds(laserCooldown);

        if (handSide == "left") canFireLaserLeft = true;
        if (handSide == "right") canFireLaserRight = true;
    }
}
