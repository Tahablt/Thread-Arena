using System.Collections;
using UnityEngine;

// Her bir dalganýn ayarlarýný Inspector'da görebilmek için oluþturduðumuz sýnýf
[System.Serializable]
public class Wave
{
    public string waveName; // Örn: "Dalga 1"
    public int enemyCount; // Bu dalgada kaç düþman çýkacak
    public float spawnRate; // Saniyede kaç düþman doðacak (örn: 2 yazarsan saniyede 2 tane çýkar)
}

public class WaveManager : MonoBehaviour
{
    public Wave[] waves; // Tüm dalgalarýn listesi
    public Transform[] spawnPoints; // Düþmanlarýn doðacaðý noktalar

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0; // Sahnede hayatta olan düþman sayýsý

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        // Dalgalar arasý 2 saniye nefes alma payý (Ýstersen silebilirsin)
        yield return new WaitForSeconds(2f);

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("Tüm dalgalar bitti! Oyuncu Kazandý!");
            yield break; // Sistemi durdur
        }

        Wave currentWave = waves[currentWaveIndex];
        Debug.Log("==== " + currentWave.waveName + " Baþlýyor! ====");

        for (int i = 0; i < currentWave.enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f / currentWave.spawnRate);
        }
    }

    void SpawnEnemy()
    {
        // Güvenlik 1: Havuz sahnede yoksa çökmesin
        if (EnemyPool.Instance == null)
        {
            Debug.LogError("HATA: Sahnede EnemyPool bulunamadý! Zombi doðamýyor.");
            return;
        }

        GameObject enemy = EnemyPool.Instance.GetEnemy();

        // Güvenlik 2: Spawn point (doðma noktasý) listesi boþ býrakýldýysa çökmesin
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy.transform.position = randomSpawnPoint.position;
            enemy.transform.rotation = randomSpawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("DÝKKAT: WaveManager içine Spawn Point eklememiþsin! Zombiler merkeze atýlýyor.");
        }

        enemiesAlive++;
    }

    // Bir düþman öldüðünde Enemy scriptinden bu fonksiyon çaðýrýlýr
    public void OnEnemyDefeated()
    {
        enemiesAlive--;

        // ÖNEMLÝ GÜNCELLEME: Çift vuruþ bug'ý yüzünden sayý -1'e düþerse sistem takýlmasýn diye "==" yerine "<=" yaptýk!
        if (enemiesAlive <= 0)
        {
            // Sayýyý sýfýrla ki eksilerde kalmasýn (yeni dalga temiz baþlasýn)
            enemiesAlive = 0;

            currentWaveIndex++;
            StartCoroutine(StartWave());
        }
    }
}