using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 0f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 1f;

    [Header("References")]
    [SerializeField] private Transform player;
    private Rigidbody rb;

    void Start()
    {
        // Initialisation des composants
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Trouver le joueur une seule fois
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            float distance = direction.magnitude;

            // Rotation progressive vers le joueur
            if (distance > stoppingDistance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                // Mouvement physique
                Vector3 movement = transform.forward * speed;
                rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
            }
            else
            {
                // Arr�t lorsque proche du joueur
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    // Debug visuel
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}