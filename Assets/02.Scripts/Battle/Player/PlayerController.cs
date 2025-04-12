using UnityEngine;
using static RhythmInputHandler; // 플레이어 입력 처리

public enum RhythmAction
{
    None = 0, // 기본값
    Jump,
    Slide,
    Roll,
    BackFlip,
    Hit,
    Die
}

public class PlayerController : MonoBehaviour
{
    public PlayerHealth PlayerHealth;

    private Animator _animator;
    private bool _isDead;
    private float _prevHealth = 0;
    private bool _inputEnabled = true;

    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
    }
    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(this);
            Debug.Log(this);
        }
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (Instance != null)
        {
            Instance.OnInputPerformed += OnInputPerf;
        }
        PlayerHealth.OnPlayerHealthChanged += OnPlayerHealthChanged; // 플레이어 체력 변경 이벤트 구독
        PlayerHealth.OnPlayerDied += HandleDie;
        _prevHealth = PlayerHealth.PlayerCurrentHealth;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(this);

            Debug.Log(this);
        }
    }
    private void OnDestroy()
    {
        if (Instance != null)
        {
            Instance.OnInputPerformed -= OnInputPerf;
        }
        PlayerHealth.OnPlayerHealthChanged -= OnPlayerHealthChanged; // 플레이어 체력 변경 이벤트 구독 해제
    }

    private void OnInputPerf(string key)
    {
        if (!_inputEnabled || _isDead) return;
        RhythmAction direction = RhythmAction.None;
        switch (key)
        {
            case "W": direction = RhythmAction.Jump; break;
            case "S": direction = RhythmAction.Slide; break;
            case "A": direction = RhythmAction.Roll; break;
            case "D": direction = RhythmAction.BackFlip; break;
        }

        _animator.SetInteger("Direction", (int)direction);
        _animator.SetTrigger("Actioned");

    }


    private void OnPlayerHealthChanged(float changed)
    {
        if(changed < _prevHealth)
        {
            _animator.SetTrigger("Hit");
        }

        _prevHealth = changed;
    }

    private void HandleDie()
    {
        if (!_isDead)
        {
            _isDead = true;
            _animator.SetTrigger("Die");
        }
    }
}