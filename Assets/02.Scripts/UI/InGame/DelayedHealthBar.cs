using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DelayedHealthBar : MonoBehaviour
{
    private Slider BackSlider;
    private Slider FrontSlider;

    // 트윈 참조를 저장하기 위한 변수
    private Tween _backSliderTween;

    [SerializeField] private float _maxHealth = 100f;

    [SerializeField] private float _currentHealth = 100f;

    public float MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            if (BackSlider != null && FrontSlider != null)
            {
                BackSlider.maxValue = value;
                FrontSlider.maxValue = value;
                InitializeHealthBar(value);
            }
        }
    }

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;

            if (FrontSlider != null)
            {
                FrontSlider.value = value;
                DelayedSetValues(value);
            }
        }
    }

    private void Awake()
    {
        // 슬라이더 컴포넌트를 가져옵니다.
        BackSlider = transform.Find("BackSlider").GetComponent<Slider>();
        FrontSlider = transform.Find("FrontSlider").GetComponent<Slider>();

        // 슬라이더의 초기값을 설정합니다.
        BackSlider.value = 0f;
        FrontSlider.value = 0f;
    }

    private void Start()
    {
        // MaxHealth 프로퍼티를 통해 초기화
        MaxHealth = _maxHealth;
    }

    private void Update()
    {
        // 현재 표시되는 체력 값을 _currentHealth에 동기화
        _currentHealth = FrontSlider.value;
    }

    private void InitializeHealthBar(float value)
    {
        BackSlider.value = 0f;
        FrontSlider.value = 0f;
        FrontSlider.DOValue(value, 2f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            BackSlider.value = value;
            _currentHealth = value;
        });
    }

    private void DelayedSetValues(float value)
    {
        // 항상 이전 애니메이션을 중지하고 새로운 애니메이션 시작
        if (_backSliderTween != null && _backSliderTween.IsPlaying())
        {
            _backSliderTween.Kill();
        }

        // 새로운 애니메이션 시작
        _backSliderTween = BackSlider.DOValue(value, 1f).SetEase(Ease.OutCubic);
    }

    private void OnDestroy()
    {
        // 클린업: 스크립트 파괴 시 트윈 정리
        if (_backSliderTween != null)
        {
            _backSliderTween.Kill();
        }
    }
}
