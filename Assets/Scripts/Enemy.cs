using UnityEngine;

public enum EnemyType { Slime, Turtle }

public class Enemy : MonoBehaviour, MeleeWeapon.IDamageable
{
    [Header("Mob Ayarlarý")]
    public EnemyType myType;
    public float maxHealth = 100f;
    public float moveSpeed = 3f;

    private float currentHealth;
    private bool isDead = false;
    private Transform player;
    private Animator anim;
    private WaveManager waveManager;

    private void Awake()
    {
        // Player referansýný bir kere al, Update'te sürekli Find çaðýrma!
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        waveManager = FindFirstObjectByType<WaveManager>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (anim != null) anim.SetBool("isMoving", true); // Doðar doðmaz yürümeye baþlasýn
    }

    private void Update()
    {
        if (player == null || isDead) return;

        // Basit Yürüme Mantýðý
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Y ekseninde kayma yapmasýn

        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // IDamageable arayüzünden gelen zorunlu metot
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Öldüðünde waveManager'a bildir
        if (waveManager != null) waveManager.OnEnemyDefeated();

        // Object Pooling: Direkt havuzdan at, Destroy etme!
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.ReturnEnemy(this.gameObject, myType);
        }
        else
        {
            Destroy(gameObject); // Yedek güvenlik önlemi
        }
    }
}