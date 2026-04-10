using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Mob Ayarlarý")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float damageToPlayer = 10f;
    private float currentHealth;

    private bool isDead = false;
    private bool isStunned = false;

    private Transform player;
    private WaveManager waveManager;
    private Animator anim;

    // Mobun yere bastýðý orijinal yüksekliði (Üst üste binmeyi engeller)
    private float defaultY;

    private void Awake()
    {
        // Oyuncuyu, dalga yöneticisini ve animatörü bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        waveManager = FindFirstObjectByType<WaveManager>();
        anim = GetComponentInChildren<Animator>();
    }

    // Mob havuzdan her doðduðunda çalýþýr
    private void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        isStunned = false;

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        // Doðduðu anki yüksekliðini betona çivilemek için kaydet
        defaultY = transform.position.y;
    }

    private void Update()
    {
        // Öldüyse, sersemlediyse veya oyuncu yoksa kýmýldama
        if (player == null || isDead || isStunned) return;

        // YÜKSEKLÝK BUG'I ÇÖZÜMÜ: Oyuncunun kafasýna deðil, ayaklarýna (kendi hizasýna) git
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        float distance = Vector3.Distance(transform.position, targetPos);

        // Yönünü oyuncuya dön
        Vector3 direction = targetPos - transform.position;
        if (direction != Vector3.zero) transform.rotation = Quaternion.LookRotation(direction);

        // Mesafeye göre yürü veya dur
        if (distance > 1.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (anim != null) anim.SetBool("isMoving", true);
        }
        else
        {
            if (anim != null) anim.SetBool("isMoving", false);
            // Karakter dibindeyse burada oyuncuya hasar verme kodunu tetikleyebilirsin
        }

        // YIÐILMA BUG'I ÇÖZÜMÜ: Fizik motoru havaya itmeye çalýþsa bile zorla yere indir
        transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Darbe yeme animasyonu ve anlýk sersemleme
        if (anim != null) anim.SetTrigger("Hit");
        StartCoroutine(HitStun());

        // Caný bittiyse öl
        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    // Kýlýç yiyince yarým saniyelik duraksama efekti
    IEnumerator HitStun()
    {
        isStunned = true;
        yield return new WaitForSeconds(0.4f);
        isStunned = false;
    }

    private void Die()
    {
        if (anim != null) anim.SetTrigger("Die");
        if (waveManager != null) waveManager.OnEnemyDefeated();

        // Ölüm animasyonunu izlemek için 2 saniye yerde bekle, sonra havuza dön
        Invoke("ReturnToPool", 2f);
    }

    private void ReturnToPool()
    {
        if (EnemyPool.Instance != null) EnemyPool.Instance.ReturnEnemy(this.gameObject);
        else Destroy(gameObject);
    }
}