using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
public class ShopSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 요소")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _buyButton;

    public event Action OnPurchaseSuccess;

    private ShopItemSO currentItem;

    public void Setup(ShopItemSO item)
    {
        currentItem = item;

        _icon.sprite = item.icon;
        _nameText.text = item.itemName;

        //_priceText.text = item.price.ToString();

        bool canBuy = CurrencyManager.Instance.CanAfford(item.currencyType, item.price);
        _buyButton.interactable = canBuy;
        _nameText.color = canBuy ? Color.white : Color.gray;

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        var price = currentItem.price;
        var type = currentItem.currencyType;

        if (!CurrencyManager.Instance.TrySpend(type, price))
        {
            Debug.Log("구매 실패: 재화 부족");
            return;
        }
        Debug.Log($"{currentItem.itemName} 구매");

        currentItem.OnPurchase?.Invoke();
        OnPurchaseSuccess?.Invoke();

        _buyButton.interactable = false;

        TooltipUI.Instance.Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(currentItem);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}
