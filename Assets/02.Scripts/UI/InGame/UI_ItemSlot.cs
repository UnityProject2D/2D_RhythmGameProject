using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections;
public class UI_ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 요소")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _useButton;
    [SerializeField] private int _shortCutIndex;
    private ItemEffectHandler _itemEffectHandler;

    private Sprite _originalSprite;
    private void Start()
    {
        _originalSprite = _icon.sprite;
    }
    public void Init(ItemEffectHandler handler)
    {
        _itemEffectHandler = handler;
        PlayerState.Instance.OnItemUsed += OnStatusChanged;
    }
    public GameObject[] OrbitEffects;

    [SerializeField] private bool _isEquipmentSlot;

    private ItemSO currentItem;

    public void OnStatusChanged(ItemUseStatus itemUseStatus)
    {
        Debug.Log($"{gameObject}: {itemUseStatus.itemID}: {itemUseStatus.flag}");
        if (currentItem == null || itemUseStatus.itemID != currentItem.itemID)
        {
            Debug.Log($"{gameObject}: currentItem == null or different");
            return;
        }
        if (itemUseStatus.itemID == currentItem.itemID && currentItem.isConsumable)
        {
            if (itemUseStatus.flag)
            {

                Debug.Log($"{gameObject}: Started Coroutine");
                StartCoroutine(ActiveForDuration());
            }
            else
            {

                Debug.Log($"{gameObject}: Clear Slot");
                ClearSlot();
            }
        }
        else if (!_isEquipmentSlot)
        {
            if (itemUseStatus.flag == false) ClearSlot();
        }
    }
    private void ClearSlot()
    {
        currentItem = null;
        _nameText.text = "";
        _icon.sprite = _originalSprite;
        foreach (var effect in OrbitEffects)
        {
            effect.SetActive(false);
        }

        _useButton.onClick.RemoveAllListeners();
    }
    private IEnumerator ActiveForDuration()
    {
        foreach (var effect in OrbitEffects)
        {
            effect.SetActive(true);
        }
        _useButton.interactable = false;
        yield return new WaitForSeconds(currentItem.EffectDuration);
        ClearSlot();
    }

    public void Setup(ItemSO item)
    {
        if (currentItem != null)
        {
            PlayerState.Instance.SetItemEnabled(currentItem.itemID, false);
        }
        currentItem = item;

        _icon.sprite = item.icon;
        _isEquipmentSlot = item.category.categoryName == "장비 아이템" ? true : false;

        if (_isEquipmentSlot || !item.isConsumable)
        {
            PlayerState.Instance.SetItemEnabled(item.itemID, true);
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
        if (_itemEffectHandler == null || currentItem == null || _isEquipmentSlot || !currentItem.isConsumable)
        {
            Debug.LogWarning($"[UI_ItemSlot]: {_itemEffectHandler == null} || {currentItem == null}|| {_isEquipmentSlot} || {!currentItem.isConsumable}");
            return;
        }
        var id = currentItem.itemID;
        float value = currentItem.EffectValue;
        float duration = currentItem.EffectDuration;

        bool success = _itemEffectHandler.ApplyEffect(id, value, duration);
        // 성공 여부에 따라 처리
        if (success)
        {
            Debug.Log($"{id} 아이템 효과 적용됨");

        }
        else
        {
            Debug.Log($"{id} 아이템 효과 적용 실패 또는 쿨타임");
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
            TooltipUI.Instance.Show(currentItem);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}