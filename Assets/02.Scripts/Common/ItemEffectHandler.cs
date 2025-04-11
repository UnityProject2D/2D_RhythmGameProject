using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemEffectHandler : MonoBehaviour
{
    private Dictionary<ItemID, Coroutine> _activeEffects = new();
    private Dictionary<ItemID, bool> _cooldownStates = new();
    private Dictionary<ItemID, float> _cooldownDurations = new()
    {
        { ItemID.CalibrationChipset, 20f }
    };
    public static ItemEffectHandler Instance { get; private set; }
    [SerializeField] private GameObject overDriveEffectPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject); 
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= DestroyOnRestart;
    }

    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameTitle")
        {
            Destroy(gameObject);
        }
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
                Instantiate(overDriveEffectPrefab);
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
            case ItemID.CombatRhythmCatcher:
                if (_activeEffects.ContainsKey(id))
                {
                    StopCoroutine(_activeEffects[id]);
                    break;
                }
                _activeEffects[id] = StartCoroutine(ApplyDurationItem(id, value, duration));
                return true;
            case ItemID.ProbabilityAmplifier:
                if (PlayerState.Instance.ProbabilityAmplifierUsed)
                {
                    Debug.LogWarning($"[ItemEffectHandler] {id} 효과가 이미 적용되었습니다.");
                    return false;
                }
                PlayerState.Instance.SetItemEnabled(id, true);
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