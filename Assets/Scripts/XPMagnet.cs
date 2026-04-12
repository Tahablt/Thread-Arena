using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    public float magnetSpeed = 5f;
    private Transform playerTransform;
    private bool isFollowing = false;

    private void Start()
    {
        // Oyuncuyu bul
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
    }

    private void Update()
    {
        if (isFollowing && playerTransform != null)
        {
            // Küreyi oyuncuya doðru hareket ettir
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, magnetSpeed * Time.deltaTime);
        }
    }

    // Mýknatýs alanýna girince takip etmeye baþla
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "MagnetArea") // Player'ýn içindeki o boþ objenin adý
        {
            isFollowing = true;
        }
    }
}