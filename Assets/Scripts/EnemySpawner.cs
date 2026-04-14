using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Ayarlarý")]
    public float slimeSpawnRate = 2f;  // 2 saniyede bir slime
    public float rammusSpawnRate = 8f; // 8 saniyede bir rammus
    public float spawnRadius = 10f;    // Oyuncunun ne kadar uzaðýnda doðacaklar?

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Belirlenen sürelerde sürekli spawn fonksiyonlarýný çaðýrýr
        InvokeRepeating("SpawnSlime", 1f, slimeSpawnRate);
        InvokeRepeating("SpawnRammus", 5f, rammusSpawnRate);
    }

    void SpawnSlime()
    {
        SpawnEnemy(EnemyType.Slime);
    }

    void SpawnRammus()
    {
        SpawnEnemy(EnemyType.Turtle);
    }

    void SpawnEnemy(EnemyType type)
    {
        // Senin yazdýðýn EnemyPool'dan düþmaný istiyoruz
        GameObject enemy = EnemyPool.Instance.GetEnemy(type);

        if (enemy != null)
        {
            // Oyuncunun etrafýnda rastgele bir konum belirle
            enemy.transform.position = GetRandomSpawnPosition();
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Oyuncunun etrafýnda rastgele bir çember üzerinde nokta seçer
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        return player.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }
}