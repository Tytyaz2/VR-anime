using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    public UnityEvent OnDeath;
    public int scoreValue = 1;

    public void Die()
    {
        // Notifie le WaveSpawner
        OnDeath.Invoke();

        Destroy(gameObject);
    }

}