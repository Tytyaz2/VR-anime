using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AttackData : MonoBehaviour
{
    [Header("Base Settings")]
    public string attackName = "Basic Attack";
    public bool isUnlocked = false;
    public int unlockLevel = 1;

    [Header("Damage Settings")]
    public int damage = 10;
    public float knockbackForce = 3f;
    public bool destroyOnImpact = true;
    public float destroyDelay = 0.1f;


    // Références aux composants
    private Rigidbody rb;
    private Collider attackCollider;
    private Renderer attackRenderer;
    private ParticleSystem attackParticles;
    private AudioSource audioSource;

    // État interne
    private bool isInitialized = false;
    private Vector3 initialScale;

    private PlayerStats playerStats;

    void Awake()
    {
        InitializeComponents();
        initialScale = transform.localScale;
        isInitialized = true;

        if (!isUnlocked)
        {
            SetAttackActive(false, true);
        }
    }

    void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        attackCollider = GetComponent<Collider>();
        attackRenderer = GetComponent<Renderer>();
        attackParticles = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        // Configuration initiale de l'AudioSource
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // Son 3D
        }
    }

    public void UnlockAttack()
    {
        isUnlocked = true;
        SetAttackActive(true, false);

    }

    public void LockAttack()
    {
        isUnlocked = false;
        SetAttackActive(false, true);
    }

    private void SetAttackActive(bool active, bool immediate)
    {
        if (!isInitialized) return;

        // Désactivation physique
        attackCollider.enabled = active;
        rb.isKinematic = !active;
        rb.detectCollisions = active;

        // Contrôle visuel
        if (attackRenderer != null)
            attackRenderer.enabled = active;

        // Effets particules
        if (attackParticles != null)
        {
            if (active) attackParticles.Play();
            else attackParticles.Stop(true, immediate ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
        }

        // Contrôle audio
        if (audioSource != null)
        {
            audioSource.enabled = active;
            if (!active && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // Ajustement d'échelle
        transform.localScale = active ? initialScale : Vector3.one * 0.001f;
    }


}