using UnityEngine;

public enum Rarities // Eþyalarýn nadirlik dereceleri
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "YeniItem", menuName = "Envanter/Item")]
public class ItemData : ScriptableObject
{
    public string id;           // Örn: "kýlýç_01"
    public string itemName;     // Örn: "Çelik Kýlýç"
    public string description;  // Örn: "Düþmanlara %10 daha fazla vurur."
    public Rarities rarity;     // Nadirlik derecesi
    public int price;           // Fiyatý
    public Sprite icon;         // Butonda görünecek resim
}