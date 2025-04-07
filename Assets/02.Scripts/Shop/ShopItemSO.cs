using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 상점에서 구매할 수 있는 아이템을 나타내는 스크립터블 오브젝트입니다.
/// </summary>
[CreateAssetMenu(menuName = "Shop/ShopItem")]
public class ShopItemSO : ScriptableObject
{
    public ItemSO itemSO;

    /// <summary>
    /// 이 아이템을 구매하는 비용입니다.
    /// </summary>
    public int price;

    /// <summary>
    /// 이 아이템을 구매하는 데 필요한 통화 유형입니다.
    /// </summary>
    public CurrencyType currencyType;

    /// <summary>
    /// 이 아이템이 구매될 때 트리거되는 이벤트입니다.
    /// </summary>
    public UnityEvent OnPurchase;

}