using UnityEngine;

public class AttackData : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 10;
    public bool destroyOnImpact = true;
    public float knockbackForce = 3f;

    [Header("Effects")]
    public GameObject impactEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                Vector3 hitDirection = transform.position - other.transform.position;
                enemy.TakeDamage(damage, -hitDirection);
            }

            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            if (destroyOnImpact)
                Destroy(gameObject);
        }
    }
}