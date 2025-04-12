using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    [Header("상점 슬롯")]
    [SerializeField] private Transform _equipSlotParent;
    [SerializeField] private Transform _consumableSlotParent;
    [SerializeField] private GameObject _shopSlotPrefab;

    [Header("판매 상품 데이터")]
    [SerializeField] private List<ShopItemSO> _equipmentItems;
    [SerializeField] private List<ShopItemSO> _consumableItems;

    [Header("아이템 갯수")]
    [SerializeField]
    [Tooltip("장비 아이템")]
    private int _equipmentItemsCount;
    [SerializeField]
    [Tooltip("소비 아이템")]
    private int _consumableItemsCount;

    [Header("재화")]
    [SerializeField] private TMP_Text _quantumKeyText;
    [SerializeField] private TMP_Text _creditText;

    private List<ShopItemSO> _currentEquipmentItems = new();
    private List<ShopItemSO> _currentConsumableItems = new();


    private void Start()
    {
        GenerateShopItems();
        UpdateCurrency();
    }
    private void OnDestroy()
    {
        foreach (Transform child in _equipSlotParent)
        {
            child.GetComponent<ShopSlotUI>().OnPurchaseSuccess -= UpdateCurrency;

            Destroy(child.gameObject);
        }
        foreach (Transform child in _consumableSlotParent)
        {
            child.GetComponent<ShopSlotUI>().OnPurchaseSuccess -= UpdateCurrency;

            Destroy(child.gameObject);
        }
    }
    public void UpdateCurrency()
    {
        _quantumKeyText.text = CurrencyManager.Instance.Get(CurrencyType.QuantumKey).ToString();
        _creditText.text = CurrencyManager.Instance.Get(CurrencyType.Credit).ToString();
    }

    public void GenerateShopItems()
    {
        _currentEquipmentItems.Clear();
        _currentConsumableItems.Clear();

        var equipment = GetRandomItems(_equipmentItems, _equipmentItemsCount);
        _currentEquipmentItems.AddRange(equipment);

        var consumables = GetRandomItems(_consumableItems, _consumableItemsCount);
        _currentConsumableItems.AddRange(consumables);

        CreateSlots();
    }

    private void CreateSlots()
    {
        foreach (var item in _currentEquipmentItems)
        {
            var slot = Instantiate(_shopSlotPrefab, _equipSlotParent);
            var slotUI = slot.GetComponent<ShopSlotUI>();
            slotUI.Setup(item);
            slotUI.OnPurchaseSuccess += UpdateCurrency;
        }
        foreach (var item in _currentConsumableItems)
        {
            var slot = Instantiate(_shopSlotPrefab, _consumableSlotParent);
            var slotUI = slot.GetComponent<ShopSlotUI>();
            slotUI.Setup(item);
            slotUI.OnPurchaseSuccess += UpdateCurrency;
        }
    }

    private List<ShopItemSO> GetRandomItems(List<ShopItemSO> sourceList, int count)
    {
        var copy = new List<ShopItemSO>(sourceList);
        var result = new List<ShopItemSO>();

        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }

    
}
