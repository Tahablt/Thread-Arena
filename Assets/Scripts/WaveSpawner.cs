using UnityEngine;
using System.Collections;
[System.Serializable] public class Wave
{
    public string dalgaAdi;
    public int dusmanSayisi;
    public float dogmaHizi;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Dalga Ayarlarý")]
    public Wave[] dalgalar;
    public float dalgalarArasiSure = 5f;

    [Header("Doðma Ayarlarý")]
    public float dogmaYaricapi = 15f;
    public Transform player;
    public GameObject enemyPrefab;

    private int aktifDalgaIndex = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(DalgalariBaslat());
    }
    IEnumerator DalgalariBaslat()
    {
        while (aktifDalgaIndex < dalgalar.Length)
        {
            Debug.Log("==== Dalga Baþladý: " + dalgalar[aktifDalgaIndex].dalgaAdi + "====");

            yield return StartCoroutine(DusmanlariUret(dalgalar[aktifDalgaIndex]));

            aktifDalgaIndex++;

            if (aktifDalgaIndex < dalgalar.Length)
            {
                Debug.Log("Dinlenme Süresi ");
                yield return new WaitForSeconds(dalgalarArasiSure);
            }
            else
            {
                Debug.Log("BÜTÜN DALGALAR BÝTTÝ !!!");
            }

        }
    }
    IEnumerator DusmanlariUret(Wave dalga)
    {
        for (int i = 0; i <= dalga.dusmanSayisi; i++)
        {
            TekBirDusmanUret();

            yield return new WaitForSeconds(dalga.dogmaHizi);
        }  
    }
    void TekBirDusmanUret() 
    {
        float rastgeleAci = Random.Range(0f, 360f);
        float xPozisyonu = player.position.x + dogmaYaricapi * Mathf.Cos(rastgeleAci * Mathf.Deg2Rad);
        float zPozisyonu = player.position.z + dogmaYaricapi * Mathf.Sin(rastgeleAci * Mathf.Deg2Rad);

        Vector3 dogmaNoktasi = new Vector3(xPozisyonu, player.position.y, zPozisyonu);
        Instantiate(enemyPrefab, dogmaNoktasi, Quaternion.identity);
    }
  }


