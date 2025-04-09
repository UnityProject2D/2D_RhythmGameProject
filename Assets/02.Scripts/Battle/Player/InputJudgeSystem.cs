using DG.Tweening;
using TMPro;
using UnityEngine;


public class InputJudgeSystem : MonoBehaviour
{
    private Animator _animator;
    public TextMeshProUGUI scoreText; // 점수 UI

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += OnScoreChg; // 점수 변경 이벤트 구독
    }

    private void OnEnable()
    {
        RhythmEvents.OnInputJudged += OnInputJdg; // 리듬 입력 판정 이벤트 구독
    }

    private void OnDisable()
    {
        RhythmEvents.OnInputJudged -= OnInputJdg; // 리듬 입력 판정 이벤트 구독 해제
        ScoreManager.Instance.OnScoreChanged -= OnScoreChg; // 점수 변경 이벤트 구독 해제
    }

    // 입력에 대한 판정 결과를 처리하는 메서드
    private void OnInputJdg(JudgedContext result)
    {
        switch (result.Result)
        {
            case JudgementResult.Perfect:
                _animator.SetTrigger("Perfect");
                break;
            case JudgementResult.Good:
                _animator.SetTrigger("Good");
                break;
            case JudgementResult.Bad:
                _animator.SetTrigger("Bad");
                break;
            case JudgementResult.Miss:
                _animator.SetTrigger("Miss");
                break;
        }
    }
    private void OnScoreChg(int score)
    {
        scoreText.text = $"{score}"; // 점수 UI 업데이트
        if (score % 500 == 0)
        {
            scoreText.transform.DOKill(); // 중복 트윙 방지
            scoreText.transform.DOScale(1.2f, 0.2f)
                .SetEase(Ease.OutBack).OnComplete(() =>
                scoreText.transform.DOScale(1f, 0.2f)); // 다시 작아짐
        }
    }
}
