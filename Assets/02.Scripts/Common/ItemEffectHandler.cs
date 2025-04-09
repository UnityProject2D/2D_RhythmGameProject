using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ItemEffectHandler : MonoBehaviour
{
    private Dictionary<ItemID, Coroutine> _activeEffects = new();
    private Dictionary<ItemID, bool> _cooldownStates = new();
    private Dictionary<ItemID, float> _cooldownDurations = new()
    {
        { ItemID.CalibrationChipset, 20f }
    };
    public static ItemEffectHandler Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    public bool ApplyEffect(ItemID id, float value, float duration = 0f)
    {
        switch (id)
        {
            case ItemID.OverDrive:
                if (_activeEffects.ContainsKey(id))
                {
                    StopCoroutine(_activeEffects[id]);
                }
                _activeEffects[id] = StartCoroutine(ApplyDurationItem(id, value, duration));
                return true;

            case ItemID.CalibrationChipset:
                if (_cooldownStates.TryGetValue(id, out var isCooling) && isCooling)
                    return false;
                StartCoroutine(CooldownRoutine(id, _cooldownDurations[id]));
                return true;
            case ItemID.ComboProtector:
                if (_activeEffects.ContainsKey(id))
                {
                    StopCoroutine(_activeEffects[id]);
                    break;
                }
                _activeEffects[id] = StartCoroutine(ApplyDurationItem(id, value, duration));
                return true;
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

    private IEnumerator ApplyDurationItem(ItemID id,float multiplier, float duration)
    {
        PlayerState.Instance.SetItemEnabled(id, true);
        yield return new WaitForSeconds(duration);
        PlayerState.Instance.SetItemEnabled(id, false);
    }

    private IEnumerator CooldownRoutine(ItemID id, float cooldown)
    {
        PlayerState.Instance.SetItemEnabled(id, false);
        yield return new WaitForSeconds(cooldown);
        PlayerState.Instance.SetItemEnabled(id, true);
    }
}