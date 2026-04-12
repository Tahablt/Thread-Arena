using UnityEngine;

public enum EnemyType { Slime, Turtle }

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Mob Ayarlarý")]
    public EnemyType myType;
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    [Tooltip("Mobun oyuncudan duracaðý mesafe")]
    public float stopDistance = 1.2f;

    [Header("Saldýrý Ayarlarý")]
    public float damageToPlayer = 10f;
    public float attackInterval = 1f;
    private float lastAttackTime;

    [Header("XP Ayarlarý")]
    public GameObject xpPrefab; // Inspector'dan XP prefabini buraya sürükle

    private float currentHealth;
    private bool isDead = false;
    private Transform player;
    private Animator anim;
    private WaveManager waveManager;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        waveManager = FindFirstObjectByType<WaveManager>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (anim != null) anim.SetBool("isMoving", true);
    }

    private void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            if (anim != null) anim.SetBool("isMoving", true);
        }
        else
        {
            if (anim != null) anim.SetBool("isMoving", false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackInterval)
            {
                PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damageToPlayer);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

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
        if (isDead) return; // Çift tetiklenmeyi engelle
        isDead = true;

        // 1. ÖNCE XP DOÐUR (Havuzlamadan veya yok etmeden önce!)
        if (xpPrefab != null)
        {
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("DÝKKAT: Enemy scriptinde XP Prefab takýlý deðil!");
        }

        // 2. SONRA DÝÐER ÝÞLEMLER
        if (waveManager != null) waveManager.OnEnemyDefeated();

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
            Destroy(gameObject);
        }
    }
}