using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maxCan = 100f;
    private float mevcutCan;
    private Animator animator;
    private NavMeshAgent agent;
    private bool oluMu = false;
    void Start()
    {
        mevcutCan = maxCan;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    public void HasarAl(float hasarMiktari)
    {
        if (oluMu) return;

        mevcutCan -= hasarMiktari;
        Debug.Log(gameObject.name + "Vuruldu! Kalan Can: " + mevcutCan);

        if (mevcutCan <= 0)
        {
            Ol();
        }
    }
    void Ol()
    {
        oluMu = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if(animator != null)
        {
            animator.SetTrigger("Dead");
        }

        Collider zombiCollider = GetComponent<Collider>();
        if(zombiCollider != null)
        {
            zombiCollider.enabled = false;
        }

        EnemyAI yerdeVurma = GetComponent<EnemyAI>();
        if(yerdeVurma != null)
        {
            yerdeVurma.enabled = false;
        }

        Destroy(gameObject, 1f);
    }
}