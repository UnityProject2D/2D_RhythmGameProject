using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 체력바를 지연 애니메이션으로 표시하는 클래스입니다.
/// 초기화는 인스펙터에서 설정한 값으로 진행되며,
/// CurrentHealth 프로퍼티를 통해 체력을 설정할 수 있습니다.
/// </summary>
public class DelayedHealthBar : MonoBehaviour
{
    [Header("체력 설정")]
    [Tooltip("최대 체력")]
    [SerializeField] private float _maxHealth = 100f;
    [Tooltip("현재 체력")]
    [SerializeField] private float _currentHealth = 100f;

    [Header("색상 설정")]
    [Tooltip("표면 색상")]
    [SerializeField] private Color _frontColor = Color.green;
    [Tooltip("지연 색상")]
    [SerializeField] private Color _backColor = Color.red;

    [Header("지연 설정")]
    [Tooltip("지연 여부")]
    [SerializeField] private bool _isDelayed = true;
    [Tooltip("지연 시간")]
    [SerializeField] private float _delayTime = 1f;
    [Tooltip("초기화 시간")]
    [SerializeField] private float _initializationTime = 2f;

    private Slider _backSlider;
    private Slider _frontSlider;

    private bool _isInitialized = false;

    // 트윈 참조를 저장하기 위한 변수
    private Tween _backSliderTween;

    public float MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            if (_backSlider != null && _frontSlider != null)
            {
                _backSlider.maxValue = value;
                _frontSlider.maxValue = value;
            }
            if (!_isInitialized)
            {
                InitializeHealthBar();
            }
        }
    }

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Max(0f, value);
            DelayedSetValues(_currentHealth);
        }
    }

    private void Awake()
    {
        // 슬라이더 컴포넌트 할당
        Slider[] sliders = GetComponentsInChildren<Slider>();
        if (sliders.Length >= 2)
        {
            _backSlider = sliders[0];
            _frontSlider = sliders[1];
        }
        else
        {
            Debug.LogError("슬라이더 컴포넌트 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        if (PlayerHealth.Instance != null)
        {
            _maxHealth = PlayerHealth.Instance.PlayerMaxHealth;
            PlayerHealth.Instance.OnPlayerHealthChanged += HandleHealthChanged; // 플레이어 체력 변경 이벤트 구독
        } // 플레이어가 죽은 경우 이벤트 구독하지 않음


        // MaxHealth 프로퍼티를 통해 초기화
        MaxHealth = _maxHealth;
        
        // 슬라이더 색상 설정
        SetColors();
    }

    private void Update()
    {
        if (_isInitialized && _currentHealth != _frontSlider.value)
        {
            // 현재 체력이 슬라이더의 값과 다를 경우 애니메이션 진행
            // 일반적인 경우 필요 없음
            DelayedSetValues(_currentHealth);
        }
    }

    private void SetColors()
    {
        // 슬라이더 색상 설정
        _backSlider.fillRect.GetComponent<Image>().color = _backColor;
        _frontSlider.fillRect.GetComponent<Image>().color = _frontColor;
    }


    private void InitializeHealthBar()
    {
        // 0부터 시작해서 최대값까지 애니메이션을 진행
        _backSlider.value = 0f;
        _frontSlider.value = 0f;

        // 지연 적용 설정에 따라 초기화 방식 결정
        if (_isDelayed)
        {
            // 애니메이션으로 초기화 (지연 효과 적용)
            _frontSlider.DOValue(_maxHealth, _initializationTime).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                _backSlider.value = _maxHealth;
                _isInitialized = true;
                DelayedSetValues(_currentHealth);
            });
        }
        else
        {
            // 애니메이션 없이 즉시 초기화
            _frontSlider.value = _maxHealth;
            _backSlider.value = _maxHealth;
            _isInitialized = true;
        }
    }

    private void DelayedSetValues(float value)
    {
        if (!_isInitialized) return;

        // 프론트 슬라이더의 값은 바로 변경
        _frontSlider.value = value;

        // 지연 적용이 활성화되어 있을 때만 애니메이션 사용
        if (_isDelayed)
        {
            // 이전 애니메이션을 중지
            if (_backSliderTween != null && _backSliderTween.IsPlaying())
            {
                _backSliderTween.Kill();
            }

            // 애니메이션으로 백 슬라이더 값 변경
            _backSliderTween = _backSlider.DOValue(value, _delayTime).SetEase(Ease.OutCubic);
        }
        else
        {
            // 애니메이션 없이 즉시 값 변경
            _backSlider.value = value;
        }
    }

    private void OnDestroy()
    {
        // 클린업: 스크립트 파괴 시 트윈 정리
        if (_backSliderTween != null)
        {
            _backSliderTween.Kill();
        }
        if (PlayerHealth.Instance != null) // 플레이어가 죽은 경우 이벤트 구독하지 않음
            PlayerHealth.Instance.OnPlayerHealthChanged -= HandleHealthChanged; // 플레이어 체력 변경 이벤트 구독 해제
    }

    private void HandleHealthChanged(float currentHealth)
    {
        _currentHealth = currentHealth;


    }
}
