using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{

    [SerializeField]
    private int _combo;
    [SerializeField]
    private double _score;
    [SerializeField]
    private double _totalScore;
    public int Combo => _combo;
    public int Score => (int)_score;
    public double TotalScore => _totalScore;

    public event Action<int> OnComboChanged;
    public event Action OnComboBreaked;
    public event Action<int> OnScoreChanged;

    public static ScoreManager Instance;

    [SerializeField]
    private int _baseScore;

    private int _perfectStreak;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        RhythmEvents.OnInputJudged += OnJudged;
        RhythmEvents.OnMusicStopped += OnStageCleared;
        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= DestroyOnRestart;
        RhythmEvents.OnInputJudged -= OnJudged;
        RhythmEvents.OnMusicStopped -= OnStageCleared;
    }

    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameTitle")
        {
            Destroy(gameObject);
        }
    }

    private void OnJudged(JudgedContext judgementResult)
    {
        float perfectBonus = 1f;
        switch (judgementResult.Result)
        {
            case JudgementResult.Perfect:
                _combo++;
                if (PlayerState.Instance.HyperScoreKernalEnabled)
                {
                    perfectBonus = 1.2f;
                }
                _perfectStreak++;
                if (_perfectStreak >= 3 && PlayerState.Instance.AutoComboSystemEnabled)
                {
                    _combo++;
                    _perfectStreak = 0;
                }
                break;
            case JudgementResult.Good:
                _combo++;
                _perfectStreak = 0;
                break;

            case JudgementResult.Bad:
                _perfectStreak = 0;
                if (PlayerState.Instance.PreciseCalibrationUnitEnabled)
                {
                    _combo++;
                }
                else if (PlayerState.Instance.ComboProtectorUsed)
                {
                    PlayerState.Instance.ComboProtectorUsed = false;
                }
                else
                {
                    _combo = 0;
                    OnComboBreaked?.Invoke();
                }
                break;

            case JudgementResult.Miss:
                _perfectStreak = 0;
                if (PlayerState.Instance.ForcedEvasionEnabled && judgementResult.RandomValue > 0.8)
                {
                    _combo++;
                }
                else if (PlayerState.Instance.ComboProtectorUsed)
                {
                    PlayerState.Instance.ComboProtectorUsed = false;
                }
                else
                {
                    _combo = 0;
                    OnComboBreaked?.Invoke();
                }
                break;
            default:
                _perfectStreak = 0;
                break;
        }

        float comboMultiplier = 1f + (_combo * 0.01f);

        _score +=
            (PlayerState.Instance.OverDriveUsed ? 2 : 1) *// 오버드라이브 2배
            _baseScore *// 기본 점수
            ((int)JudgementResult.Count - (int)judgementResult.Result) *// 판정 보정
            (PlayerState.Instance.PreciseCalibrationUnitEnabled ? 0.8f : 1f) *// 보정칩 핸디캡
            comboMultiplier *// 콤보 계수
            perfectBonus * // 하이퍼 스코어 커널 계수
            (judgementResult.Result == JudgementResult.Miss ? 0 : 1); // Miss 면 곱하기 0

        OnComboChanged?.Invoke(_combo);
        Debug.Log($"SCORE = {(int)_score}");
        OnScoreChanged?.Invoke((int)_score);

    }

    private void OnStageCleared()
    {
        _totalScore += _score;
    }
}
