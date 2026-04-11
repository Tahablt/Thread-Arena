using UnityEngine;
using System.Collections;

// YENÝ VE KUSURSUZ SÝSTEM: Mob türleri için açýlýr menü (Ýstediðin kadar ekleyebilirsin)
public enum EnemyType { Slime, Turtle }

public class Enemy : MonoBehaviour
{
    [Header("Mob Ayarlarý")]
    public EnemyType myType; // YAZI YAZMAK YOK! Inspector'dan seçeceksin.

    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float damageToPlayer = 10f;
    private float currentHealth;

    private bool isDead = false;
    private bool isStunned = false;

    private Transform player;
    private WaveManager waveManager;
    private Animator anim;
    private float defaultY;

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
        isStunned = false;

        if (anim != null) { anim.Rebind(); anim.Update(0f); }
        defaultY = transform.position.y;
    }

    private void Update()
    {
        if (player == null || isDead || isStunned) return;

        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        float distance = Vector3.Distance(transform.position, targetPos);

        Vector3 direction = targetPos - transform.position;
        if (direction != Vector3.zero) transform.rotation = Quaternion.LookRotation(direction);

        if (distance > 1.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (anim != null) anim.SetBool("isMoving", true);
        }
        else
        {
            if (anim != null) anim.SetBool("isMoving", false);
        }

        transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (anim != null) anim.SetTrigger("Hit");
        StartCoroutine(HitStun());

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

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
        Invoke("ReturnToPool", 2f);
    }

    private void ReturnToPool()
    {
        // Havuza dönerken artýk yazý deðil, kendi menü seçimini (Slime veya Turtle) yolluyor
        if (EnemyPool.Instance != null) EnemyPool.Instance.ReturnEnemy(this.gameObject, myType);
        else Destroy(gameObject);
    }
}