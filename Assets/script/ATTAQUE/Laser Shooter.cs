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
    public AudioClip laserClip;
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
            // Récupère le AttackData du prefab instancié
            AttackData attackData = laserInstance.GetComponent<AttackData>();

            // Ne joue le son que si l'attaque est débloquée
            if (attackData != null && attackData.isUnlocked)
            {
                AudioSource audioSource = laserInstance.AddComponent<AudioSource>();
                if (laserClip != null)
                {
                    audioSource.clip = laserClip;
                    audioSource.loop = false; // Le son ne doit pas boucler
                    audioSource.volume = 0.1f;  // Réduire le volume de 5 fois

                    // Démarrer l'audio, mais à partir de la position 0.5 seconde du clip
                    audioSource.time = 0.5f;
                    audioSource.Play();
                }
                // Destroye le laser après 1 seconde
                Destroy(laserInstance, 1f);
            }
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