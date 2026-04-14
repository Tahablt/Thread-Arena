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
        int count = DataManager.Instance.tumEsyalar.Count;

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, count);
            rastgeleItemler.Add(DataManager.Instance.tumEsyalar[randomIndex]);

            if (i < buttons.Length)
            {
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = rastgeleItemler[i].itemName;
            }
        }
    }

    public void OnClick_SelectButton(int index)
    {
        if (saveData != null && index < rastgeleItemler.Count)
        {
            ItemData selected = rastgeleItemler[index];

            // 1. Kayýt et
            saveData.AddItem(selected.id);

            // 2. ETKÝYÝ TETÝKLE
            ApplyItemEffect(selected.id);

            Debug.Log("Seçilen eþya: " + selected.itemName);
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

            case "kilic_01": // Kýlýç ID'n
                // Eðer karakterinde bir 'damage' deðiþkeni varsa
                // FindObjectOfType<PlayerCombat>().damage += 5; 
                Debug.Log("Kýlýç hasarý arttýrýldý!");
                break;

            case "zone_01": // Zone ID'n
                // Player'ýn altýndaki zone objesini bul ve aktif et
                GameObject zone = GameObject.Find("DamageZone");
                if (zone != null) zone.SetActive(true);
                Debug.Log("Çevre hasar alaný aktif edildi!");
                break;

            default:
                Debug.Log("Bu ID için henüz özellik yazýlmadý: " + itemId);
                break;
        }
    }
}