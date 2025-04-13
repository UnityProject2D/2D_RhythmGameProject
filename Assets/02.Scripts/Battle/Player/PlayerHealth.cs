using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float PlayerMaxHealth { get; private set; }
    public float Damage;
    private float _playerCurrentHealth;
    public bool IsTutorial;

    public float PlayerCurrentHealth => _playerCurrentHealth;

    public event Action<float> OnPlayerHealthChanged;
    public event Action OnPlayerDied;

    public MMF_Player OnMissFeedback;
    public MMF_Player OnHealFeedback;


    private void Awake()
    {
        PlayerMaxHealth = 10;
        _playerCurrentHealth = PlayerMaxHealth;
    }
    private void OnEnable()
    {
        RhythmEvents.OnInputJudged += HandleJudge;
    }

    private void Start()
    {
        _playerCurrentHealth = GameManager.Instance.PlayerHealth;
        OnPlayerHealthChanged?.Invoke(_playerCurrentHealth);
    }
    private void OnDisable()
    {
        RhythmEvents.OnInputJudged -= HandleJudge;
        GameManager.Instance.PlayerHealth = _playerCurrentHealth;
    }


    /// <summary>
    /// 판정에 따라 처리
    /// </summary>
    /// <param name="result"></param>
    private void HandleJudge(JudgedContext result)
    {
        float finalDamage = 0;
        switch (result.Result)
        {
            case JudgementResult.Perfect:
                HandlePerfect(result);
                break;
            case JudgementResult.Bad:
                finalDamage = HandleBad(result);
                break;
            case JudgementResult.Miss:
                finalDamage = HandleMiss(result);
                break;
        }

        if (PlayerState.Instance.EmergencyResponseCoreEnabled)
        {
            finalDamage = ApplyEmergencyResponseCore(finalDamage);
        }

        ApplyDamage(finalDamage);
    }

    #region Handle Judgement
    /// <summary>
    /// Perfect일 때 아이템에 따라 처리
    /// </summary>
    /// <param name="result"></param>
    private void HandlePerfect(JudgedContext result)
    {
        // 리커버리 코어
        if (PlayerState.Instance.RecoveryCoreEnabled)
        {
            if (result.RandomValue > 0.8f)
            {
#if UNITY_EDITOR
                Debug.Log("리커버리 코어");
#endif
                RecoveryHealth(PlayerState.Instance.RecoveryCoreAmount);
                //TODO: 이펙트 추가
            }

        }
    }

    /// <summary>
    /// Bad 일 때 아이템에 따라 처리
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private float HandleBad(JudgedContext result)
    {
        // 오버드라이브
        if (PlayerState.Instance.OverDriveUsed)
        {
#if UNITY_EDITOR
            Debug.Log("오버드라이브: bad 일때 데미지");
#endif
            OnMissFeedback?.PlayFeedbacks();
            return Damage;
        }
        return 0;
    }

    /// <summary>
    /// 미스일 때 아이템에 따라 분기
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private float HandleMiss(JudgedContext result)
    {
        if (PlayerState.Instance.ForcedEvasionEnabled && result.RandomValue > 0.8f)
        {
#if UNITY_EDITOR
            Debug.Log("강제 회피 프로토콜");
#endif
            //TODO: 이펙트 추가
        }
        else if (PlayerState.Instance.CalibrationChipsetEnabled)
        {
            if (!PlayerState.Instance.ItemEffectHandler.ApplyEffect(ItemID.CalibrationChipset, 0))
            {
                OnMissFeedback?.PlayFeedbacks(); // 쿨타임 중이면 데미지
                return Damage;
            }
#if UNITY_EDITOR
            Debug.Log("자동 교정 유닛");
#endif
            //TODO: 이펙트 추가
        }
        else
        {
            OnMissFeedback?.PlayFeedbacks();
            return Damage;
        }
        return 0;
    }

    #endregion

    private float ApplyEmergencyResponseCore(float finalDamage)
    {
        if (_playerCurrentHealth < PlayerMaxHealth * PlayerState.Instance.EmergencyResponseCoreThreshold)
        {
            return finalDamage - finalDamage * PlayerState.Instance.EmergencyResponseCoreReduce;
        }
        return finalDamage;
    }

    #region Modify Health
    /// <summary>
    /// 플레이어 체력 - finalDamage
    /// </summary>
    /// <param name="finalDamage">최종뎀</param>
    /// 

    private void ApplyDamage(float finalDamage)
    {
        if (IsTutorial) return;
        Debug.Log($"[HP] ApplyDamage: {finalDamage}");
        _playerCurrentHealth = Mathf.Max(0, _playerCurrentHealth - finalDamage);

#if UNITY_EDITOR
        Debug.Log($"[HP] {_playerCurrentHealth}/{PlayerMaxHealth} (-{finalDamage})");
#endif

        OnPlayerHealthChanged?.Invoke(_playerCurrentHealth);

        if (_playerCurrentHealth == 0)
        {
            // 긴급 회피 프로토콜
            if (PlayerState.Instance.EmergencyEvasionEnabled)
            {
                PlayerState.Instance.EmergencyEvasionEnabled = false;

                RecoveryHealth(PlayerState.Instance.EmergencyEvasionRecoveryAmount);
                //TODO: 이펙트 추가
            }
            else
            {
                OnPlayerDied?.Invoke();
            }
        }
        
    }

    /// <summary>
    /// amount 만큼 체력을 회복합니다.
    /// </summary>
    /// <param name="amount">체력 회복량</param>
    private void RecoveryHealth(float amount)
    {
        _playerCurrentHealth = Mathf.Min(PlayerMaxHealth, _playerCurrentHealth + amount);
        OnHealFeedback?.PlayFeedbacks();
    }

    #endregion


}
