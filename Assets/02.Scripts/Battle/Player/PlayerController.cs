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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Instance.OnInputPerformed += OnInputPerf;
        PlayerHealth.OnPlayerHealthChanged += OnPlayerHealthChanged; // 플레이어 체력 변경 이벤트 구독
        PlayerHealth.OnPlayerDied += HandleDie;
        _prevHealth = PlayerHealth.PlayerCurrentHealth;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(this);
        }
    }
    private void OnDisable()
    {
        Instance.OnInputPerformed -= OnInputPerf;
        PlayerHealth.OnPlayerHealthChanged -= OnPlayerHealthChanged; // 플레이어 체력 변경 이벤트 구독 해제
    }

    private void OnInputPerf(string key)
    {
        if (_isDead) return;
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
            _prevHealth = changed;
            _animator.SetTrigger("Hit");
        }
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