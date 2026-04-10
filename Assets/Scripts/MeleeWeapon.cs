using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Kýlýç Ayarlarý")]
    public float damage = 25f; // Kýlýcýn vuracaðý hasar

    // KILIÇ BÝR ÞEYE ÇARPTIÐINDA (3D FÝZÝK)
    private void OnTriggerEnter(Collider other)
    {
        // Çarptýðýmýz þeyin Tag'i "Enemy" ise
        if (other.CompareTag("Enemy"))
        {
            // Düþmanýn içindeki Enemy scriptini bul
            Enemy hitEnemy = other.GetComponent<Enemy>();

            // Bulduysak hasarý vur
            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(damage);
                Debug.Log(other.gameObject.name + " objesine 3D kýlýç girdi!");
            }
        }
    }
}