using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject levelUpPanel;
    public Button[] buttons;

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
        Time.timeScale = 0;
        levelUpPanel.SetActive(true);

        rastgeleItemler = new List<ItemData>();
        
        // Kopyasını alıyoruz ki orijinal listeyi bozmadan içinden çıkarma yapabilelim
        List<ItemData> availableItems = new List<ItemData>(DataManager.Instance.tumEsyalar);

        for (int i = 0; i < buttons.Length; i++)
        {
            // Eğer eşya kalmadıysa geri kalan butonları gizle
            if (availableItems.Count == 0)
            {
                buttons[i].gameObject.SetActive(false);
                continue;
            }

            buttons[i].gameObject.SetActive(true);

            // Rastgele ama FARKLI eşyalar seç (aynı ekranda aynı eşya 2 kere yan yana durmasın)
            int randomIndex = Random.Range(0, availableItems.Count);
            ItemData selected = availableItems[randomIndex];
            rastgeleItemler.Add(selected);
            availableItems.RemoveAt(randomIndex); // Bir kez çıkanı seçeneklerden kaldır

            // UI YAZILARINI GÜNCELLE
            TextMeshProUGUI[] texts = buttons[i].GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) texts[0].text = selected.itemName; // İlk Text her zaman isimdir
            if (texts.Length > 1) texts[1].text = selected.description; // Varsa ikinci Text'e açıklamayı bas

            // UI İKONUNU GÜNCELLE
            Image[] images = buttons[i].GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                // Butonun kendi arka planı değil de, içindeki (çocuğu olan) boş Image objesini bul
                if (img.gameObject != buttons[i].gameObject)
                {
                    if (selected.icon != null)
                    {
                        img.sprite = selected.icon;
                        img.preserveAspect = true; // RESMİN SÜNMESİNİ VE YAMULMASINI KESİNLİKLE ENGELLER!
                        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f); // Görünür yap
                    }
                    else
                    {
                        // Eğer bu eşyanın ikonu Unity'den atanmamışsa, eski kılıç resmi filan kalmasın diye görünmez yap
                        img.sprite = null;
                        img.color = new Color(img.color.r, img.color.g, img.color.b, 0f); // Görünmez yap!
                    }
                    break;
                }
            }
        }
    }

    public void OnClick_SelectButton(int index)
    {
        if (saveData != null && index < rastgeleItemler.Count)
        {
            ItemData selected = rastgeleItemler[index];

            // 1. Kayıt et
            saveData.AddItem(selected.id);

            // 2. ETKİYİ TETİKLE
            ApplyItemEffect(selected.id);

            Debug.Log("Seçilen eşya: " + selected.itemName);
        }

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