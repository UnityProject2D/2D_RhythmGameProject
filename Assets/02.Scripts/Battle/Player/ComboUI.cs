using DG.Tweening;
using TMPro;
using UnityEngine;


public class ComboUI : MonoBehaviour
{
    private Animator _animator;
    public TextMeshProUGUI comboText; // 콤보 UI


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ScoreManager.Instance.OnComboChanged += OnComboChg; // 점수 변경 이벤트 구독
        ScoreManager.Instance.OnComboBreaked += OnComboBrk; // 점수 변경 이벤트 구독
    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnComboChanged -= OnComboChg; // 점수 변경 이벤트 구독 해제
        ScoreManager.Instance.OnComboBreaked -= OnComboBrk; // 점수 변경 이벤트 구독 해제
    }

    private void OnComboChg(int combo)
    {
        if (combo > 0)
        {
            _animator.SetTrigger("Combo"); // 애니메이션 트리거 설정
                                           //comboText.text = $"Combo: {combo}"; // 콤보 UI 업데이트

            if (combo >= 10)
            {
                comboText.transform.DOKill();
                comboText.color = Color.yellow; // 깜빡이기 전 색상

                // 깜빡임 + 살짝 커지기
                Sequence seq = DOTween.Sequence();
                seq.Append(comboText.DOFade(0f, 0.1f))
                   .Append(comboText.DOFade(1f, 0.1f))
                   .Append(comboText.transform.DOScale(1.2f, 0.15f))
                   .Append(comboText.transform.DOScale(1f, 0.15f));
            }
        }
    }

    private void OnComboBrk()
    {
    }
}
