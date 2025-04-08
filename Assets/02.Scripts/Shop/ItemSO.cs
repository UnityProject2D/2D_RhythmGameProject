using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// 상점에서 구매할 수 있는 아이템을 나타내는 스크립터블 오브젝트입니다.
/// </summary>
[CreateAssetMenu(menuName = "Item/Item Entity")]
public class ItemSO : ScriptableObject
{
    /// <summary>
    /// 상점 아이템의 이름입니다.
    /// </summary>
    public string itemName;

    /// <summary>
    /// UI에서 상점 아이템을 나타내는 아이콘입니다.
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// 상점 아이템의 효과나 목적에 대한 설명입니다.
    /// </summary>
    public string description;

    public string EffectDescription;

    /// <summary>
    /// 이 아이템이 상점에서 속하는 카테고리입니다.
    /// </summary>
    public ItemCategorySO category;

    public bool isConsumable = false;
}