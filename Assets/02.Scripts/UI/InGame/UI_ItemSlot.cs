using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
public class UI_ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 요소")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _useButton;
    [SerializeField] private ItemCategorySO equip;
    [SerializeField] private int _shortCutIndex;

    public GameObject[] OrbitEffects;

    [SerializeField] private bool _isEquipmentSlot;

    private ItemSO currentItem;


    public void Setup(ItemSO item)
    {
        currentItem = item;

        _icon.sprite = item.icon;
        var isEquipment = item.category == equip;

        if (_isEquipmentSlot || !item.isConsumable)
        {
            _nameText.text = "";
            _useButton.interactable = false;
            foreach (var effect in OrbitEffects)
            {
                effect.SetActive(true);
            }
        }
        else
        {
            _nameText.text = _shortCutIndex.ToString();

            _useButton.interactable = true;
            foreach (var effect in OrbitEffects)
            {
                effect.SetActive(false);
            }
        }
        _useButton.onClick.RemoveAllListeners();
        _useButton.onClick.AddListener(UseItem);
    }

    private void UseItem()
    {

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
