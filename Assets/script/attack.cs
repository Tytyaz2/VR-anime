using UnityEngine;

public class attack : MonoBehaviour
{
    public GameObject Attaque; // l'attaque a instancier
    public Transform firepoint; //spawn du projectile
    public float bulletForce = 20f; // vitesse du projectile

    public void shoot() {
    GameObject projectile = Instantiate(Attaque, firepoint.position , firepoint.rotation);
    Rigidbody rb = projectile.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.AddForce(firepoint.forward * bulletForce,ForceMode.Impulse);
        }
    }
}
