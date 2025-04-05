using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/ShopItem")]
public class ShopItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int price;
    public CurrencyType currencyType;

    public ItemCategorySO category;

    public UnityEvent OnPurchase; // 구매 시 효과
}