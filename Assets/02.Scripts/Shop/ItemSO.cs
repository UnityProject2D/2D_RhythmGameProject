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

    /// <summary>
    /// 이 아이템이 상점에서 속하는 카테고리입니다.
    /// </summary>
    public ItemCategorySO category;

    /// <summary>
    /// 아이템의 적용할 수 있는 효과를 나타냅니다.
    /// </summary>
    [System.Serializable]
    public struct ItemEffect
    {
        /// <summary>
        /// 아이템에 의해 적용될 수 있는 효과 유형입니다.
        /// </summary>
        public enum EffectType
        {
            /// <summary>체력을 증가시킵니다.</summary>
            Health,
            /// <summary>최대 체력을 증가시킵니다.</summary>
            MaxHealth,
            /// <summary>방어 성공 시 체력을 회복합니다.</summary>
            HealthOnBlock,
            /// <summary>리듬 입력 판정 범위를 증가시킵니다.</summary>
            RhythmInputRange,
            /// <summary>적 공격 속도를 감소시킵니다.</summary>
            EnemyAttackSpeed,
        }

        /// <summary>
        /// 이 아이템이 적용하는 효과 유형입니다.
        /// </summary>
        public EffectType effectType;

        /// <summary>
        /// 효과의 크기입니다.
        /// </summary>
        public float value;
    }

    /// <summary>
    /// 이 아이템이 구매될 때 적용되는 효과 배열입니다.
    /// </summary>
    public ItemEffect[] effects;
}