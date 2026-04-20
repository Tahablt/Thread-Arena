using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject levelUpPanel;
    public UpgradeSlot[] slots;

    [Header("Veri")]
    public PlayerSaveData saveData;
    public PlayerHealth playerHealth; // Can iksiri için referans

    private List<ItemData> rastgeleItemler;

    private void Start()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
    }

    public void ShowUpgradeMenu()
    {
        // DataManager dan tüm itrm ları getir
        // Tüm Itm lardan rastgele 3 tane item çek. Bu itemların hepsi birbirinden farklı olmalı
        // Bu rastgele Itemları spawn et. (Buonlara atamasını yap)

        Time.timeScale = 0;
        levelUpPanel.SetActive(true);

        rastgeleItemler = new List<ItemData>();

        // Kopyasını alıyoruz ki orijinal listeyi bozmadan içinden çıkarma yapabilelim
        List<ItemData> availableItems = new List<ItemData>(DataManager.Instance.tumEsyalar);

        // rastgele 3 item ile liste oluşur
        for (int i = 0; i < slots.Length; i++)
        {
            // Eğer eşya kalmadıysa geri kalan butonları gizle
            if (availableItems.Count == 0)
            {
                slots[i].gameObject.SetActive(false);
                continue;
            }

            slots[i].gameObject.SetActive(true);

            // Rastgele ama FARKLI eşyalar seç (aynı ekranda aynı eşya 2 kere yan yana durmasın)
            int randomIndex = Random.Range(0, availableItems.Count);
            ItemData selected = availableItems[randomIndex];
            rastgeleItemler.Add(selected);
            availableItems.RemoveAt(randomIndex); // Bir kez çıkanı seçeneklerden kaldır
        }

        availableItems = null;


        for (int i = 0; i < slots.Length; i++)
        {

            var selected = rastgeleItemler[i];

            slots[i].Set(selected, () => OnClick_SelectButton(selected));
        }

    }

    public void OnClick_SelectButton(ItemData data)
    {

        // 1. Kayıt et
        saveData.AddItem(data.id);

        // 2. ETKİYİ TETİKLE
        ApplyItemEffect(data.id);


        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void ApplyItemEffect(string itemId)
    {
        switch (itemId)
        {
            case "can_iksiri": // ScriptableObject ID'n neyse onu yaz
                if (playerHealth != null) playerHealth.Heal(50);
                Debug.Log("Can yenilendi!");
                break;

            case "kilic_01": // Kılıç ID'n
                // Eğer karakterinde bir 'damage' değişkeni varsa
                // FindObjectOfType<PlayerCombat>().damage += 5; 
                Debug.Log("Kılıç hasarı arttırıldı!");
                break;

            case "zone_01": // Zone ID'n
                // Player'ın altındaki zone objesini bul ve aktif et
                GameObject zone = GameObject.Find("DamageZone");
                if (zone != null) zone.SetActive(true);
                Debug.Log("Çevre hasar alanı aktif edildi!");
                break;

            default:
                Debug.Log("Bu ID için henüz özellik yazılmadı: " + itemId);
                break;
        }
    }
}