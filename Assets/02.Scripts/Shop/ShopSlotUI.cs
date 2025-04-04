using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _buyButton;

    private ShopItemSO currentItem;

    public void Setup(ShopItemSO item)
    {
        currentItem = item;

        _icon.sprite = item.icon;
        _nameText.text = item.itemName;
        //_priceText.text = item.price.ToString();
        //TODO. 화폐체크 후 버튼 interactive 변경, text color 변경, 아이템에 마우스 오버시 정보 표시

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        //TODO. 화폐 체크
        Debug.Log($"{currentItem.itemName} 구매");

        currentItem.OnPurchase?.Invoke();

        _buyButton.interactable = false;
    }
}
