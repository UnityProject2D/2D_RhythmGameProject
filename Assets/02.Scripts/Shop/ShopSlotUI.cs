using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using FMODUnity;
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

        _icon.sprite = item.itemSO.icon;
        _nameText.text = item.itemSO.itemName;

        //_priceText.text = item.price.ToString();

        bool canBuy = CurrencyManager.Instance.CanAfford(item.currencyType, item.price);
        _buyButton.interactable = canBuy;
        _nameText.color = canBuy ? Color.white : Color.gray;

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        // 슬롯 확인
        bool isEquip = currentItem.itemSO.category.categoryName == "장비 아이템";
        bool isConsumable = currentItem.itemSO.isConsumable;

        if (isEquip && UI_GameSlots.Instance.slots[0].HasItem())
        {
            Debug.Log("장비 슬롯이 이미 사용 중입니다.");

            RuntimeManager.PlayOneShot("event:/SFX/Failed");
            return;
        }
        else if (isConsumable && UI_GameSlots.Instance.slots[1].HasItem())
        {
            Debug.Log("소비 슬롯이 이미 사용 중입니다.");
            RuntimeManager.PlayOneShot("event:/SFX/Failed");
            return;
        }

        // 구매 처리
        if (!CurrencyManager.Instance.TrySpend(currentItem.currencyType, currentItem.price))
        {
            Debug.Log("구매 실패: 재화 부족");
            RuntimeManager.PlayOneShot("event:/SFX/Failed");    
            return;
        }

        RuntimeManager.PlayOneShot("event:/SFX/Upgrade");
        Debug.Log($"{currentItem.itemSO.itemName} 구매 완료");
        currentItem.OnPurchase?.Invoke();
        OnPurchaseSuccess?.Invoke();
        _buyButton.interactable = false;

        // 슬롯에 추가
        UI_GameSlots.Instance.SetSlot(currentItem.itemSO);

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
