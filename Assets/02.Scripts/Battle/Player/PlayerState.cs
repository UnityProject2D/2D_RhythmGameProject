using System;
using UnityEngine;
public struct ItemUseStatus
{
    public ItemUseStatus(ItemID id, bool p)
    {
        itemID = id;
        flag = p;
    }
    public ItemID itemID;
    public bool flag;
}

[Flags]
public enum ItemFlag
{
    None = 0,
    // 영구 및 장비 아이템
    PerfectRecoveryCore = 1 << 0,
    EmergencyResponseCore = 1 << 1,
    AccessLevelCore = 1 << 2,
    RecoveryAlgorithmCore = 1 << 3,
    CalibrationChipset = 1 << 4,
    ForcedEvasion = 1 << 5,
    AutoComboSystem = 1 << 6,
    PreciseCalibrationUnit = 1 << 7,
    HyperScoreKernal = 1 << 8,
    DataCacheModule = 1 << 9,

    // 소비 아이템
    EmergencyEvasion = 1 << 10,
    OverDrive = 1 << 11,
    ProbabilityAmplifier = 1 << 12,
    ComboProtector = 1 << 13,
    HackingTool = 1 << 14,
}

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }
    public ItemEffectHandler ItemEffectHandler;
    public event Action<ItemUseStatus> OnItemUsed;

    [Header("아이템 관련")]
    [Space(10)]
    [Header("영구 아이템")]
    [Space(5)]
    [Header("리커버리 코어: Perfect시 일정 확률로 체력 n 회복")]
    public bool RecoveryCoreEnabled;
    public float RecoveryCoreAmount;
    [Header("위기 대응 코어: HP p% 미만일 때 데미지 감소 q%")]
    public bool EmergencyResponseCoreEnabled;
    [Tooltip("위기 대응 코어 체력 비율")]
    public float EmergencyResponseCoreThreshold;
    [Tooltip("위기 대응 코어 데미지 감소 비율")]
    public float EmergencyResponseCoreReduce;
    [Header("접속권한 레벨 코어: 상점 아이템 20% 할인")]
    public bool AccessLevelCoreEnabled;
    [Header("회복 알고리즘: n스테이지마다 체력 회복")]
    public bool RecoveryAlgorithmCoreEnabled;

    [Space(10)]
    [Header("장비 아이템")]
    [Space(5)]
    [Header("자동 교정 유닛: 체력 깎이는거 한번 방어")]
    public bool CalibrationChipsetEnabled;
    [Header("강제 회피 프로토콜: 미스해도 일정확률로 Good 판정")]
    public bool ForcedEvasionEnabled;
    [Header("자동 콤보 시스템: Perfect 3회마다 추가 콤보 누적")]
    public bool AutoComboSystemEnabled;
    [Header("정밀 보정 칩셋: Bad도 콤보 유지, 기본 점수 획득량 감소")]
    public bool PreciseCalibrationUnitEnabled;
    [Header("하이퍼 스코어 커널: Perfect시 20% 증가된 점수 획득")]
    public bool HyperScoreKernalEnabled;
    [Header("골드 획득기?: 스테이지 클리어시 돈 추가 획득")]
    public bool DataCacheModuleEnabled;

    [Space(10)]
    [Header("소비 아이템")]
    [Space(5)]
    //패턴 체계화: 적 공격 패턴이 조금 더 일관된 템포로 단순화
    [Header("긴급 회피 프로토콜: HP 0 될 경우 1회 생존 (자동 사용)")]
    public bool EmergencyEvasionEnabled;
    [Tooltip("긴급 회피 프로토콜 체력 회복량")]
    public float EmergencyEvasionRecoveryAmount;
    [Header("오버 드라이브: 퍼펙트시 2배 점수, Bad일때 체력 소모")]
    public bool OverDriveUsed;
    [Header("확률 증폭제: 다음 보상 확률 2배")]
    public bool ProbabilityAmplifierUsed;
    [Header("전투 리듬 캐쳐: 미스 판정 시에도 콤보 유지 (일회성)")]
    public bool ComboProtectorUsed;
    [Header("해킹 툴: 상점 아이템 랜덤 1개 무료")]
    public bool HackingToolUsed;
    public void SetItemEnabled(ItemID id, bool flag)
    {
        switch (id)
        {
            case ItemID.PerfectRecoveryCore:
                RecoveryCoreEnabled = flag;
                break;
            case ItemID.EmergencyResponseCore:
                EmergencyResponseCoreEnabled = flag;
                break;
            case ItemID.AccessLevelCore:
                AccessLevelCoreEnabled = flag;
                break;
            case ItemID.RecoveryAlgorithmCore:
                RecoveryAlgorithmCoreEnabled = flag;
                break;
            case ItemID.CalibrationChipset:
                CalibrationChipsetEnabled = flag;
                break;
            case ItemID.ForcedEvasion:
                ForcedEvasionEnabled = flag;
                break;
            case ItemID.AutoComboSystem:
                AutoComboSystemEnabled = flag;
                break;
            case ItemID.PreciseCalibrationUnit:
                PreciseCalibrationUnitEnabled = flag;
                break;
            case ItemID.DataCacheModule:
                DataCacheModuleEnabled = flag;
                break;
            case ItemID.EmergencyEvasion:
                EmergencyEvasionEnabled = flag;
                break;
            case ItemID.OverDrive:
                OverDriveUsed = flag;
                break;
            case ItemID.ProbabilityAmplifier:
                ProbabilityAmplifierUsed = flag;
                break;
            case ItemID.ComboProtector:
                ComboProtectorUsed = flag;
                break;
            case ItemID.HackingTool:
                HackingToolUsed = flag;
                break;
            case ItemID.HyperScoreKernal:
                HyperScoreKernalEnabled = flag;
                break;
            default:
                Debug.LogWarning($"[PlayerState] 알 수 없는 ItemID: {id}");
                return;
        }
        var itemStatus = new ItemUseStatus(id, flag);
        OnItemUsed?.Invoke(itemStatus);
    }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RestoreItemFlags(GameManager.Instance.SavedItemFlags);
    }

    private void RestoreItemFlags(ItemFlag flags)
    {
        RecoveryCoreEnabled = flags.HasFlag(ItemFlag.PerfectRecoveryCore);
        EmergencyResponseCoreEnabled = flags.HasFlag(ItemFlag.EmergencyResponseCore);
        AccessLevelCoreEnabled = flags.HasFlag(ItemFlag.AccessLevelCore);
        RecoveryAlgorithmCoreEnabled = flags.HasFlag(ItemFlag.RecoveryAlgorithmCore);
        CalibrationChipsetEnabled = flags.HasFlag(ItemFlag.CalibrationChipset);
        ForcedEvasionEnabled = flags.HasFlag(ItemFlag.ForcedEvasion);
        AutoComboSystemEnabled = flags.HasFlag(ItemFlag.AutoComboSystem);
        PreciseCalibrationUnitEnabled = flags.HasFlag(ItemFlag.PreciseCalibrationUnit);
        HyperScoreKernalEnabled = flags.HasFlag(ItemFlag.HyperScoreKernal);
        DataCacheModuleEnabled = flags.HasFlag(ItemFlag.DataCacheModule);

        EmergencyEvasionEnabled = flags.HasFlag(ItemFlag.EmergencyEvasion);
        OverDriveUsed = flags.HasFlag(ItemFlag.OverDrive);
        ProbabilityAmplifierUsed = flags.HasFlag(ItemFlag.ProbabilityAmplifier);
        ComboProtectorUsed = flags.HasFlag(ItemFlag.ComboProtector);
        HackingToolUsed = flags.HasFlag(ItemFlag.HackingTool);
    }

    private void OnDestroy()
    {
        GameManager.Instance.SavedItemFlags = CollectCurrentFlags();
    }

    private ItemFlag CollectCurrentFlags()
    {
        ItemFlag result = ItemFlag.None;

        if (RecoveryCoreEnabled) result |= ItemFlag.PerfectRecoveryCore;
        if (EmergencyResponseCoreEnabled) result |= ItemFlag.EmergencyResponseCore;
        if (AccessLevelCoreEnabled) result |= ItemFlag.AccessLevelCore;
        if (RecoveryAlgorithmCoreEnabled) result |= ItemFlag.RecoveryAlgorithmCore;
        if (CalibrationChipsetEnabled) result |= ItemFlag.CalibrationChipset;
        if (ForcedEvasionEnabled) result |= ItemFlag.ForcedEvasion;
        if (AutoComboSystemEnabled) result |= ItemFlag.AutoComboSystem;
        if (PreciseCalibrationUnitEnabled) result |= ItemFlag.PreciseCalibrationUnit;
        if (HyperScoreKernalEnabled) result |= ItemFlag.HyperScoreKernal;
        if (DataCacheModuleEnabled) result |= ItemFlag.DataCacheModule;

        if (EmergencyEvasionEnabled) result |= ItemFlag.EmergencyEvasion;
        if (OverDriveUsed) result |= ItemFlag.OverDrive;
        if (ProbabilityAmplifierUsed) result |= ItemFlag.ProbabilityAmplifier;
        if (ComboProtectorUsed) result |= ItemFlag.ComboProtector;
        if (HackingToolUsed) result |= ItemFlag.HackingTool;

        return result;
    }
}
