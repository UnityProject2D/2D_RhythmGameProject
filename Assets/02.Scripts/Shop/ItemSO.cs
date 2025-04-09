using UnityEngine.Events;
using UnityEngine;

public enum ItemID
{
    PerfectRecoveryCore,
    EmergencyResponseCore,
    AccessLevelCore,
    RecoveryAlgorithmCore,

    CalibrationChipset,
    ForcedEvasion,
    CombatRhythmCatcher,
    PreciseCalibrationUnit,
    DataCacheModule,
    HyperScoreKernal,
    AutoComboSystem,

    EmergencyEvasion,
    OverDrive,
    ProbabilityAmplifier,
    ComboProtector,
    HackingTool,
    PatternStabilizer,

}

[CreateAssetMenu(menuName = "Item/Item Entity")]
public class ItemSO : ScriptableObject
{
    public ItemID itemID;
    public string itemName;
    public Sprite icon;
    public string description;

    public string EffectDescription;

    /// <summary>
    /// 이 아이템이 상점에서 속하는 카테고리입니다.
    /// </summary>
    public ItemCategorySO category;

    public bool isConsumable = false;
    /// <summary>
    /// 아이템의 적용할 수 있는 효과를 나타냅니다.
    /// </summary>
    /// 
    public float EffectValue;
    public float EffectDuration;
    public float value;


    public float EffectCooldown;
}