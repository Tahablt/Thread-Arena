using UnityEngine;
using System.Collections; // IEnumerator için bunu ekledik

public class MeleeWeapon : MonoBehaviour
{
    [Header("Kýlýç Ayarlarý")]
    public float damage = 25f;
    public bool isAttacking = false; // KÝLÝT BURASI: Sadece saldýrýrken true olacak!

    // Kýlýç bir þeye çarptýðýnda
    private void OnTriggerEnter(Collider other)
    {
        // 1. KURAL: Eðer saldýrmýyorsak (isAttacking false ise) alt tarafý hiç okuma, iptal et!
        if (!isAttacking) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy hitEnemy = other.GetComponent<Enemy>();

            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(damage);
                Debug.Log(other.gameObject.name + " objesine hasar verildi!");
            }
        }
    }

    // Ekrandaki "Fire" butonuna basýnca bu fonksiyonu çalýþtýracaðýz
    public void PerformAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // Saldýrý süresini ayarlayan minik zamanlayýcý
    IEnumerator AttackRoutine()
    {
        isAttacking = true; // Hasar kilidini aç

        // Kýlýcýn hasar verme süresi (Örn: yarým saniye boyunca çarptýklarýna hasar versin)
        yield return new WaitForSeconds(0.5f);

        isAttacking = false; // Süre bitince hasar kilidini tekrar kapat
    }
}