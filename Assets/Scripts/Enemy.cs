using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Düþman Ayarlarý")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;  // Düþmanýn hareket hýzý
    private float currentHealth;

    private bool isDead = false; // ÖNEMLÝ: Çift vuruþ bug'ýný engelleyecek kilit

    private Transform player;
    private WaveManager waveManager;

    private void Awake()
    {
        // Sahnedeki oyuncuyu "Player" tag'i ile buluyoruz.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Sahnede 'Player' tagine sahip bir obje bulunamadý!");
        }

        // Dalga yöneticisini buluyoruz
        waveManager = FindFirstObjectByType<WaveManager>();
    }

    // Bu kod düþman havuzdan her çekildiðinde (görünür olduðunda) otomatik çalýþýr
    private void OnEnable()
    {
        currentHealth = maxHealth; // Caný fulle ki ölü doðmasýn
        isDead = false; // Havuzdan çýkýnca ölüm kilidini sýfýrla ki tekrar hasar alabilsin
    }

    private void Update()
    {
        // Oyuncu sahnede varsa ona doðru hareket et
        if (player != null && !isDead)
        {
            // Dümdüz oyuncuya doðru ilerler
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            // 3D OYUN ÝÇÝN YÜZÜNÜ OYUNCUYA DÖNME KODU
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Zombi yukarý/aþaðý eðilmesin, sadece saða sola dönsün diye Y'yi sýfýrlýyoruz

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    // Karakterin mermisi veya kýlýcý bu fonksiyonu tetikleyecek
    public void TakeDamage(float damage)
    {
        // Eðer zombi zaten öldüyse (ama daha havuza gidemeden kýlýç bir daha çarptýysa) hasarý umursama
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            isDead = true; // Zombiyi ölü olarak iþaretle ki bir daha hasar yemesin
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("1 - Düþman öldü! Havuza gitmeye çalýþýyor...");

        if (waveManager != null)
        {
            waveManager.OnEnemyDefeated();
        }
        else
        {
            Debug.LogWarning("DÝKKAT: WaveManager bulunamadý!");
        }

        // Havuz sahnede var mý diye kontrol ediyoruz
        if (EnemyPool.Instance != null)
        {
            Debug.Log("2 - Havuz bulundu! Düþman baþarýyla havuza geri gönderiliyor.");
            EnemyPool.Instance.ReturnEnemy(this.gameObject);
        }
        else
        {
            Debug.LogError("3 - KRÝTÝK HATA: Sahnede EnemyPool scripti bulunamadý! Düþman havuza gidemediði için kalýcý olarak siliniyor (Destroy).");
            Destroy(gameObject);
        }
    }
}