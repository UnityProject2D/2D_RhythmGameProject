using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemEffectHandler : MonoBehaviour
{
    private Dictionary<ItemID, Coroutine> _activeEffects = new();
    private Dictionary<ItemID, bool> _cooldownStates = new();
    private Dictionary<ItemID, float> _cooldownDurations = new()
    {
        { ItemID.CalibrationChipset, 20f }
    };
    public bool ApplyEffect(ItemID id, float value, float duration = 0f)
    {
        switch (id)
        {
            case ItemID.OverDrive:
                if (_activeEffects.ContainsKey(id)) StopCoroutine(_activeEffects[id]);
                _activeEffects[id] = StartCoroutine(ApplyOverdrive(value, duration));
                break;

            case ItemID.CalibrationChipset:
                if (_cooldownStates.TryGetValue(id, out var isCooling) && isCooling)
                    return false;
                StartCoroutine(CooldownRoutine(id, _cooldownDurations[id]));
                return true;
            case ItemID.ComboProtector:
                if (_activeEffects.ContainsKey(id))
                {
                    StopCoroutine(_activeEffects[id]);
                }
                _activeEffects[id] = StartCoroutine(ApplyComboProtector(value, duration));
                break;
            default:
                Debug.LogWarning($"[PlayerState] 알 수 없는 ItemID: {id}");
                break;
        }

        return false;
    }
    public void ApplyEffect(ItemID id, bool flag)
    {
        PlayerState.Instance.SetItemEnabled(id, flag);
    }

    private IEnumerator ApplyOverdrive(float multiplier, float duration)
    {
        PlayerState.Instance.OverDriveUsed = true;
        yield return new WaitForSeconds(duration);
        PlayerState.Instance.OverDriveUsed = false;
    }
    private IEnumerator ApplyComboProtector(float multiplier, float duration)
    {
        PlayerState.Instance.ComboProtectorUsed = true;
        yield return new WaitForSeconds(duration);
        PlayerState.Instance.ComboProtectorUsed = false;
    }

    private IEnumerator CooldownRoutine(ItemID id, float cooldown)
    {
        PlayerState.Instance.SetItemEnabled(id, false);
        yield return new WaitForSeconds(cooldown);
        PlayerState.Instance.SetItemEnabled(id, true);
    }
}