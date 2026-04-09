using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxCan = 100f;
    private float mevcutCan;

    void Start()
    {
        mevcutCan = maxCan;
    }

    public void HasarAl(float hasarMiktari)
    {
        mevcutCan -= hasarMiktari;
        Debug.Log("Hasar yedik, Kalan can miktarý: " + mevcutCan);

        // Can 0'a eþit veya daha küçükse karakter ölür
        if (mevcutCan <= 0)
        {
            Ol();
        }
    }

    // Ol() fonksiyonu artýk baðýmsýz bir þekilde dýþarýda duruyor!
    void Ol()
    {
        Debug.Log("Oyuncu Öldü! GAME OVER !!!");
        // Ýleride buraya oyun sonu ekranýný açan kodlarý ekleyeceðiz
    }
}