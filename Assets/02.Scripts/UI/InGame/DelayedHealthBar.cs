using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 체력바를 지연 표시하는 클래스입니다.
/// 초기화는 인스펙터에서 설정한 값으로 진행되며,
/// CurrentHealth 프로퍼티를 통해 체력을 설정할 수 있습니다.
/// </summary>
public class DelayedHealthBar : MonoBehaviour
{
    [Header("체력 설정")]
    [Tooltip("최대 체력")]
    [SerializeField] private float _maxHealth = 20f;
    [Tooltip("현재 체력")]
    [SerializeField] private float _currentHealth = 20f;

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

    private float _previousHealth = 20f; // 이전 체력

    // 트윈 참조를 저장하기 위한 변수
    private Tween _delayedSliderTween;

    public PlayerHealth playerHealth;

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
            _previousHealth = _currentHealth; // 변경 전 체력 저장
            _currentHealth = Mathf.Max(0f, value);
            DelayedSetValues(_currentHealth, _previousHealth);
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
        _maxHealth = playerHealth.PlayerMaxHealth;
        _previousHealth = _currentHealth; // 최초의 이전 체력은 현재 체력
        playerHealth.OnPlayerHealthChanged += HandleHealthChanged;
        //if (GameManager.Instance.Player.Health != null)
        //{
        //    OnPlayerRegistered();
        //}
        //else
        //{
        //    GameManager.Instance.PlayerRegistered += OnPlayerRegistered; // 플레이어 등록 이벤트 구독
        //}
        // MaxHealth 프로퍼티를 통해 초기화하면 트위닝 적용됨
        MaxHealth = _maxHealth;
        //SetPlayer().Forget();
        // 슬라이더 색상 설정
        SetColors();
    }


    //private async UniTaskVoid SetPlayer()
    //{
    //    while (GameManager.Instance.Player.Health == null)
    //    {
    //        await UniTask.Yield();
    //    }
    //    var playerHealth = GameManager.Instance.Player.Health;
    //    _maxHealth = playerHealth.PlayerMaxHealth;
    //    playerHealth.OnPlayerHealthChanged += HandleHealthChanged;
    //    GameManager.Instance.PlayerRegistered -= OnPlayerRegistered;
    //}
    //private void OnPlayerRegistered()
    //{
    //    var playerHealth = GameManager.Instance.Player.Health;
    //    _maxHealth = playerHealth.PlayerMaxHealth;
    //    playerHealth.OnPlayerHealthChanged += HandleHealthChanged;
    //    GameManager.Instance.PlayerRegistered -= OnPlayerRegistered; // 플레이어 등록 이벤트 구독 해제
    //}
    private void Update()
    {
#if UNITY_EDITOR
        // 현재 체력이 이전 체력과 다를 경우
        // 일반적인 경우 필요 없음
        // 인스펙터에서 _currentHealth 바꾸면서 확인할 때 필요
        if (_isInitialized && _currentHealth != _previousHealth)
        {
            DelayedSetValues(_currentHealth, _previousHealth);
        }
#endif
    }

    private void SetColors()
    {
        // 슬라이더 색상 설정
        _backSlider.fillRect.GetComponent<Image>().color = _backColor;
        _frontSlider.fillRect.GetComponent<Image>().color = _frontColor;
    }


    private void InitializeHealthBar()
    {
        // 0부터 시작해서 최대 체력까지 트위닝 진행
        _backSlider.value = 0f;
        _frontSlider.value = 0f;

        // 지연 적용 설정에 따라 초기화 방식 결정
        if (_isDelayed)
        {
            // 트위닝으로 초기화 (지연 효과 적용)
            _frontSlider.DOValue(_maxHealth, _initializationTime).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                _backSlider.value = _maxHealth;
                _isInitialized = true;
                DelayedSetValues(_currentHealth, _previousHealth);
            });
        }
        else
        {
            // 트위닝 없이 즉시 초기화
            _frontSlider.value = _maxHealth;
            _backSlider.value = _maxHealth;
            _isInitialized = true;
        }
    }

    private void DelayedSetValues(float newValue, float oldValue)
    {
        if (!_isInitialized) return;

        bool isDecreasing = newValue < oldValue; // 체력이 감소하는지 확인

        // 즉시 적용해야 하는 슬라이더 참조
        Slider immediateSlider = isDecreasing ? _frontSlider : _backSlider;
        // 지연 적용해야 하는 슬라이더 참조
        Slider delayedSlider = isDecreasing ? _backSlider : _frontSlider;

        // 즉시 슬라이더 값 변경
        immediateSlider.value = newValue;

        // 지연 적용이 활성화되어 있을 때만 트위닝 사용
        if (_isDelayed)
        {
            // 이전 트위닝 중지
            if (_delayedSliderTween != null && _delayedSliderTween.IsPlaying())
            {
                _delayedSliderTween.Kill();
            }

            // 트위닝으로 지연 슬라이더 값 변경
            _delayedSliderTween = delayedSlider.DOValue(newValue, _delayTime).SetEase(Ease.OutCubic);
        }
        else
        {
            // 트위닝 없이 즉시 값 변경
            delayedSlider.value = newValue;
        }
    }

    private void OnDestroy()
    {
        // 클린업: 스크립트 파괴 시 트윈 정리
        if (_delayedSliderTween != null)
        {
            _delayedSliderTween.Kill();
        }
        if (GameManager.Instance.Player != null) // 플레이어가 죽은 경우 이벤트 구독하지 않음
            GameManager.Instance.Player.Health.OnPlayerHealthChanged -= HandleHealthChanged; // 플레이어 체력 변경 이벤트 구독 해제
    }

    private void HandleHealthChanged(float currentHealth)
    {
        // _currentHealth 대신 CurrentHealth 프로퍼티를 사용해야 정상적으로 지연 트위닝이 적용됨
        CurrentHealth = currentHealth;
    }
}
