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

    public GameObject[] OrbitEffects;

    [SerializeField] private bool _isEquipmentSlot;

    public ItemSO CurrentItem;

    private Sprite _originalSprite;
    private void Start()
    {
        _originalSprite = _icon.sprite;
    }
    public void Init(ItemEffectHandler handler)
    {
        _itemEffectHandler = handler;
        if(PlayerState.Instance == null)
        {
            Debug.LogError("PlayerState Instance is null");
            return;
        }
        PlayerState.Instance.OnItemUsed += OnStatusChanged;
    }

    public void OnStatusChanged(ItemUseStatus itemUseStatus)
    {
        Debug.Log($"{gameObject}: {itemUseStatus.itemID}: {itemUseStatus.flag}");
        if (CurrentItem == null || itemUseStatus.itemID != CurrentItem.itemID)
        {
            Debug.Log($"{gameObject}: currentItem == null or different");
            return;
        }
        if (itemUseStatus.itemID == CurrentItem.itemID && CurrentItem.isConsumable)
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
        CurrentItem = null;
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
        yield return new WaitForSeconds(CurrentItem.EffectDuration);
        ClearSlot();
    }

    public void Setup(ItemSO item)
    {
        if (CurrentItem != null)
        {
            PlayerState.Instance.SetItemEnabled(CurrentItem.itemID, false);
        }
        CurrentItem = item;

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
        if (_itemEffectHandler == null || CurrentItem == null || _isEquipmentSlot || !CurrentItem.isConsumable)
        {
            Debug.LogWarning($"[UI_ItemSlot]: {_itemEffectHandler == null} || {CurrentItem == null}|| {_isEquipmentSlot} || {!CurrentItem.isConsumable}");
            return;
        }
        var id = CurrentItem.itemID;
        float value = CurrentItem.EffectValue;
        float duration = CurrentItem.EffectDuration;

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
        if (CurrentItem != null)
            TooltipUI.Instance.Show(CurrentItem);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }

    public bool HasItem()
    {
        return CurrentItem != null;
    }
}