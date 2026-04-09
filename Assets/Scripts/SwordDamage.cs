using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public float kilicHasari = 25f; // 4 vuruþta 100 caný bitirir

    // Kýlýç (Trigger) baþka bir objenin içine girdiðinde çalýþýr
    private void OnTriggerEnter(Collider other)
    {
        // Eðer çarptýðýmýz þeyin Tag'i "Enemy" ise
        if (other.CompareTag("Enemy"))
        {
            // O objenin içindeki EnemyHealth kodunu çek
            EnemyHealth zombiCani = other.GetComponent<EnemyHealth>();

            // Eðer kod varsa hasarý yapýþtýr!
            if (zombiCani != null)
            {
                zombiCani.HasarAl(kilicHasari);
            }
        }
    }
}