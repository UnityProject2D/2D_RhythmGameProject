using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour
{
    [Header("패널 전체 페이드용")]
    public CanvasGroup panelCanvasGroup;

    [Header("제목 관련")]
    public TextMeshProUGUI TitleText;
    public Image TitleBG;

    [Header("항목 텍스트")]
    public TextMeshProUGUI IndexPerfect;
    public TextMeshProUGUI IndexGood;
    public TextMeshProUGUI IndexMaxCombo;
    public TextMeshProUGUI IndexScore;

    [Header("결과 텍스트")]
    public TextMeshProUGUI ResultPerfect;
    public TextMeshProUGUI ResultGood;
    public TextMeshProUGUI ResultMaxCombo;
    public TextMeshProUGUI ResultScore;

    [Header("배경 이미지")]
    public Image PerfectBG;
    public Image GoodBG;
    public Image MaxComboBG;
    public Image ScoreBG;

    // targetBGColor: Hex #18A174, 투명도 0x39 (57/255)
    private Color targetBGColor = new Color(24f / 255f, 161f / 255f, 116f / 255f, 57f / 255f);

    private void Start()
    {
        // 패널 투명하게 초기화
        panelCanvasGroup.alpha = 0f;
        // Title 및 그룹 텍스트와 BG 초기 상태 설정 (알파 0, 스케일 0.8)
        InitUI(TitleText, TitleBG);
        InitUI(IndexPerfect, null);
        InitUI(ResultPerfect, PerfectBG);
        InitUI(IndexGood, null);
        InitUI(ResultGood, GoodBG);
        InitUI(IndexMaxCombo, null);
        InitUI(ResultMaxCombo, MaxComboBG);
        InitUI(IndexScore, null);
        InitUI(ResultScore, ScoreBG);

        ScoreManager.Instance.OnScoreChanged += UpdateScore;
    }

    private void InitUI(TextMeshProUGUI txt, Image bg)
    {
        txt.alpha = 0f;
        txt.transform.localScale = Vector3.one * 0.8f;
        if (bg != null)
        {
            bg.color = new Color(targetBGColor.r, targetBGColor.g, targetBGColor.b, 0f);
            bg.transform.localScale = Vector3.one * 0.8f;
        }
    }

    private void UpdateScore(int score)
    {
        ResultScore.text = score.ToString();
    }

    // 헬퍼 메서드: 한 그룹의 왼쪽(인덱스) 텍스트, 오른쪽(결과) 텍스트, 그리고 BG 애니메이션을 Sequence에 추가
    private void AppendGroup(Sequence seq, RectTransform leftRect, TextMeshProUGUI leftText,
                             RectTransform rightRect, TextMeshProUGUI rightText, Image bg,
                             float offset = 130f, float dur = 0.4f, float interval = 0.5f)
    {
        float origLeft = leftRect.anchoredPosition.x;
        float origRight = rightRect.anchoredPosition.x;
        // 오프셋 적용
        leftRect.anchoredPosition = new Vector2(origLeft + offset, leftRect.anchoredPosition.y);
        rightRect.anchoredPosition = new Vector2(origRight - offset, rightRect.anchoredPosition.y);

        seq.Append(leftRect.DOAnchorPosX(origLeft, dur).SetEase(Ease.OutElastic));
        seq.Join(leftText.DOFade(1f, dur));
        seq.Join(leftText.transform.DOScale(1f, dur).SetEase(Ease.OutBack));
        seq.Join(rightRect.DOAnchorPosX(origRight, dur).SetEase(Ease.OutElastic));
        seq.Join(rightText.DOFade(1f, dur));
        seq.Join(rightText.transform.DOScale(1f, dur).SetEase(Ease.OutBack));
        if (bg != null)
        {
            seq.Join(bg.DOColor(targetBGColor, dur));
            seq.Join(bg.transform.DOScale(1f, dur).SetEase(Ease.OutBack));
        }
        seq.AppendCallback(() =>
        {
            leftText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1f);
            rightText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1f);
        });
        seq.AppendInterval(interval);
    }

    public void PlayOpenEffect()
    {
        gameObject.SetActive(true);
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            Sequence seq = DOTween.Sequence();

            // Title 그룹 애니메이션
            seq.Append(TitleText.DOFade(1f, 0.4f));
            seq.Join(TitleText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            if (TitleBG)
            {
                seq.Join(TitleBG.DOColor(targetBGColor, 0.4f));
                seq.Join(TitleBG.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            }
            seq.AppendInterval(0.5f);

            // 각 그룹 애니메이션 호출 (Perfect, Good, MaxCombo, Score)
            AppendGroup(seq, IndexPerfect.GetComponent<RectTransform>(), IndexPerfect,
                        ResultPerfect.GetComponent<RectTransform>(), ResultPerfect, PerfectBG);
            AppendGroup(seq, IndexGood.GetComponent<RectTransform>(), IndexGood,
                        ResultGood.GetComponent<RectTransform>(), ResultGood, GoodBG);
            AppendGroup(seq, IndexMaxCombo.GetComponent<RectTransform>(), IndexMaxCombo,
                        ResultMaxCombo.GetComponent<RectTransform>(), ResultMaxCombo, MaxComboBG);
            AppendGroup(seq, IndexScore.GetComponent<RectTransform>(), IndexScore,
                        ResultScore.GetComponent<RectTransform>(), ResultScore, ScoreBG);
        });
    }
}
