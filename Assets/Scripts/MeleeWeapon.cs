using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Kýlýç Ayarlarý")]
    public float damage = 25f;
    public bool isAttacking = false;

    // Çoklu hasar vermeyi engellemek için geçici liste
    private System.Collections.Generic.HashSet<IDamageable> hitThisSwing = new System.Collections.Generic.HashSet<IDamageable>();

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        // TryGetComponent, GetComponent'ten daha hýzlýdýr ve GC allocation yapmaz (Mobil için harika)
        if (other.TryGetComponent(out IDamageable hitTarget))
        {
            // Eðer bu savuruþta bu hedefe zaten vurduysak, tekrar vurma (Multi-hit bug'ý engeller)
            if (!hitThisSwing.Contains(hitTarget))
            {
                hitTarget.TakeDamage(damage);
                hitThisSwing.Add(hitTarget);
                Debug.Log(other.gameObject.name + " objesine hasar verildi!");
            }
        }
    }

    public void PerformAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        hitThisSwing.Clear(); // Yeni savuruþta vurulanlar listesini temizle

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }
}