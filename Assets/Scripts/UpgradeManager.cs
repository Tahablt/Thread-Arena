using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject levelUpPanel;
    public UpgradeCard[] cards;

    [Header("Veri")]
    public PlayerSaveData saveData;
    public PlayerHealth playerHealth;
    public Character playerCharacter;

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
        List<ItemData> availableItems = new List<ItemData>(DataManager.Instance.tumEsyalar);

        for (int i = 0; i < cards.Length; i++)
        {

            int randomIndex = Random.Range(0, availableItems.Count);
            ItemData selected = availableItems[randomIndex];
            rastgeleItemler.Add(selected);
            availableItems.RemoveAt(randomIndex);

            cards[i].Set(selected, () => OnClick_SelectButton(selected));
        }
    }

    public void OnClick_SelectButton(ItemData data)
    {
        saveData.AddItem(data.id);
        ApplyItemEffect(data.itemType);

        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void ApplyItemEffect(ItemTypes type)
    {
        if (playerCharacter == null)
        {
            playerCharacter = FindFirstObjectByType<Character>();
        }


        switch (type)
        {
            case ItemTypes.Health:
                if (playerHealth != null) playerHealth.Heal(50);
                break;

            case ItemTypes.Sword:
                if (playerCharacter != null) playerCharacter.IncreaseDamage(5f);
                break;

            case ItemTypes.Zone: // Eğer ScriptableObject'te ID "zone_01" ise buraya da girer
                // Karakterin altındaki AuraWeapon scriptini (Kapalı olsa bile) bulur
                AuraWeapon aura = playerCharacter.GetComponentInChildren<AuraWeapon>(true);

                if (aura != null)
                {
                    if (!aura.gameObject.activeSelf)
                    {
                        // İlk alımda objeyi aç
                        aura.gameObject.SetActive(true);
                        Debug.Log("Aura Silahı İlk Kez Aktif Edildi!");
                    }
                    else
                    {
                        // Sonraki alımlarda geliştir
                        aura.IncreaseRange(0.5f);
                        aura.damage += 2f;
                        Debug.Log("Aura Menzili ve Hasarı Artırıldı!");
                    }
                }
                else
                {
                    Debug.LogError("HATA: Karakterin altında AuraWeapon scripti (DamageZone) bulunamadı!");
                }
                break;

            default:
                Debug.Log("Bu ID için henüz özellik yazılmadı: ");
                break;
        }
    }
}