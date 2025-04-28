using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 0f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 1f;

    [Header("Footstep Audio")]
    public AudioClip[] footstepSounds;
    [Tooltip("Intervalle entre les pas en secondes")]
    public float footstepInterval = 0.5f;
    [Range(0, 1)] public float spatialBlend = 0.8f;
    public float minDistance = 5f;  // Distance à laquelle le son est à volume maximum
    public float maxDistance = 30f; // Distance à laquelle le son n'est plus audible
    [Range(0.1f, 0.5f)] public float pitchVariation = 0.2f;

    [Header("Footstep Volume")]
    [Range(0f, 2f)] public float footstepVolume = 1.5f;


    private Transform player;
    private Rigidbody rb;
    private AudioSource audioSource;
    private float nextFootstepTime;
    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void ConfigureAudioSource()
    {
        audioSource.spatialBlend = spatialBlend; // 0=2D, 1=3D
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;
        isMoving = distance > stoppingDistance && speed > 0.1f;

        HandleMovement(direction);
        HandleFootsteps();
    }

    void HandleMovement(Vector3 direction)
    {
        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(transform.forward.x * speed, rb.linearVelocity.y, transform.forward.z * speed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void HandleFootsteps()
    {
        if (!isMoving || footstepSounds.Length == 0) return;

        if (Time.time >= nextFootstepTime)
        {
            PlayFootstepSound();
            nextFootstepTime = Time.time + footstepInterval * (1f + Random.Range(-0.1f, 0.1f)); // Légère variation aléatoire
        }
    }

    void PlayFootstepSound()
    {
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        audioSource.PlayOneShot(clip, 1.5f);
    }
}