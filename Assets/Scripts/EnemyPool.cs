using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance; // Her yerden kolayca ulaþabilmek için Singleton yapýyoruz

    [Header("Pool Ayarlarý")]
    public GameObject enemyPrefab; // Zombi/Mob prefabýn
    public int poolSize = 50; // Baþlangýçta kaç tane üretileceði

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        // Oyun baþlarken havuzu düþmanlarla doldur
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false); // Sahnede görünmesinler
            pool.Enqueue(obj); // Sýraya ekle
        }
    }

    // Havuzdan düþman çaðýrmak için bu fonksiyonu kullanacaðýz
    public GameObject GetEnemy()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Eðer havuzda hiç düþman kalmadýysa (hepsi sahnedeyse) acil durum olarak yeni üret
            GameObject obj = Instantiate(enemyPrefab);
            return obj;
        }
    }

    // Düþman öldüðünde yok etmeyip bu fonksiyonla havuza geri atacaðýz
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        pool.Enqueue(enemy);
    }
}