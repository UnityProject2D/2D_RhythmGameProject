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
    private Animator _animator;

    public bool IsDead;
    public bool IsAlive = true;

    private float _prevHealth = 0;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Instance.OnInputPerformed += OnInputPerf;
        PlayerHealth.Instance.OnPlayerHealthChanged += OnPlayerHealthChanged; // 플레이어 체력 변경 이벤트 구독
        _prevHealth = PlayerHealth.Instance.PlayerCurrentHealth;
    }

    private void OnDisable()
    {
        Instance.OnInputPerformed -= OnInputPerf;
        PlayerHealth.Instance.OnPlayerHealthChanged -= OnPlayerHealthChanged; // 플레이어 체력 변경 이벤트 구독
    }

    private void OnInputPerf(string key)
    {
        if (IsDead) return;
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
}