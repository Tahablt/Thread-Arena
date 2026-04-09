using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private float temasMesafesi = 1.5f; // Bize ne kadar yaklaþýnca hasar verecek
    [SerializeField] private float hasarAraligi = 0.5f;  // Saniyede kaç kere hasar verecek (0.5 saniye = hýzlý sömürür)
    [SerializeField] private float verilecekHasar = 5f;  // Her dokunuþta kaç can gidecek

    private Transform playerTarget;
    private PlayerHealth playerHealth; // Karakterimizin can kodu
    private NavMeshAgent agent;
    private Animator animator;
    private float sonHasarZamani = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // Oyuncuyu bul ve can kodunu hafýzaya al (Optimizasyon için)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 1. ZOMBÝ ARTIK HÝÇ DURMAYACAK, SÜREKLÝ ÜSTÜMÜZE KOÞACAK
        agent.SetDestination(playerTarget.position);

        if (animator != null)
        {
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude);
        }

        // 2. TEMAS (HASAR) KONTROLÜ
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= temasMesafesi)
        {
            // Zamanlayýcý dolduysa yapýþtýr hasarý!
            if (Time.time >= sonHasarZamani + hasarAraligi)
            {
                if (playerHealth != null)
                {
                    playerHealth.HasarAl(verilecekHasar);
                }
                sonHasarZamani = Time.time;
            }
        }
    }
}