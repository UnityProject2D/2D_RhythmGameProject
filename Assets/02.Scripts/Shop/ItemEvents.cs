using System;
using System.Collections.Generic;
using UnityEngine;

public static class ItemEvents
{
    public static event Action<bool> OnEnableRecoveryCore;
    public static event Action<bool> OnEnableEmergencyResponseCore;
    public static event Action<bool> OnEnableAccessLevelCore;
    public static event Action<bool> OnEnableRecoveryAlgorithmCore;
    public static event Action<bool> OnEnableCalibrationChipset;
    public static event Action<bool> OnEnableForcedEvasion;
    public static event Action<bool> OnEnableAutoComboSystem;
    public static event Action<bool> OnEnablePreciseCalibrationUnit;
    public static event Action<bool> OnEnableBonusChip;
    public static event Action<bool> OnEnableEmergencyEvasion;
    public static event Action<bool> OnOverDriveUsed;
    public static event Action<bool> OnProbabilityAmplifierUsed;
    public static event Action<bool> OnComboProtectorUsed;
    public static event Action<bool> OnHackingToolUsed;

    private static readonly Dictionary<ItemID, Action<bool>> _eventMap;

    static ItemEvents()
    {
        _eventMap = new Dictionary<ItemID, Action<bool>>
        {
            { ItemID.PerfectRecoveryCore, OnEnableRecoveryCore },
            { ItemID.EmergencyResponseCore, OnEnableEmergencyResponseCore },
            { ItemID.AccessLevelCore, OnEnableAccessLevelCore },
            { ItemID.RecoveryAlgorithmCore, OnEnableRecoveryAlgorithmCore },
            { ItemID.CalibrationChipset, OnEnableCalibrationChipset },
            { ItemID.ForcedEvasion, OnEnableForcedEvasion },
            { ItemID.AutoComboSystem, OnEnableAutoComboSystem },
            { ItemID.PreciseCalibrationUnit, OnEnablePreciseCalibrationUnit },
            { ItemID.DataCacheModule, OnEnableBonusChip },
            { ItemID.EmergencyEvasion, OnEnableEmergencyEvasion },
            { ItemID.OverDrive, OnOverDriveUsed },
            { ItemID.ProbabilityAmplifier, OnProbabilityAmplifierUsed },
            { ItemID.ComboProtector, OnComboProtectorUsed },
            { ItemID.HackingTool, OnHackingToolUsed },
        };
    }

    public static void ModifyItemUse(ItemID id, bool use)
    {
        if(_eventMap.TryGetValue(id, out var action))
        {
            action?.Invoke(use);
        }
        else
        {
            Debug.LogWarning($"[ItemEvents] 등록되지 않은 ItemID: {id}");
        }
    }

}
