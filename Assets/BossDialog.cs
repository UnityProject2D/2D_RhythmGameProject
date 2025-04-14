using MoreMountains.Feedbacks;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RhythmEvents;
using FronkonGames.Glitches.Interferences;

public class BossDialog : MonoBehaviour
{
    public TutorialSequenceSO BossSequenceSO;

    public MMFeedbacks MMFeedback;
    private MMF_TMPTextReveal _textReveal;
    public TextMeshProUGUI Text;
    public int curBasicStep = -1;
    public int curLoopStep = -1;
    private TutorialStepSo _curBossStepSo;
    public static TutorialManager Instance { get; private set; }
    Interferences interferences;

    public MMF_Player DialogMMF;
    private void Start()
    {
        interferences = Interferences.Instance;
        _curBossStepSo = BossSequenceSO.Steps[0];
    }
    private void Init()
    {
        _textReveal = MMFeedback.FeedbacksList.OfType<MMF_TMPTextReveal>().FirstOrDefault();
        
        interferences.SetActive(!interferences.isActive);
        TriggerNextStep();

        if (!flag)
        {
            flag = true;

            StartCoroutine(PlayOverDriveSequence());
        }
    }

    private void OnEnable()
    {
        TutorialEventSystem.OnTutorialTextEvent += HandleEvent;
        RhythmEvents.OnMarkerHit += HandleEvent;
    }

    private void OnDisable()
    {
        TutorialEventSystem.OnTutorialTextEvent -= HandleEvent;
        OnMarkerHit -= HandleEvent;
        interferences.SetActive(false);
    }

    private void DisableText()
    {
        DialogMMF.PlayFeedbacks();
    }


    public void FeedbackSetting(TutorialStepSo tutorialStepSo)
    {
        if (_textReveal == null) return;
        Debug.Log($"BossDialog: {tutorialStepSo.Text}");
    }
    bool flag = false;
    public void HandleEvent(string Trigger = "")
    {
        if(_curBossStepSo.TextNextConditionType == TextNextConditionType.OnEvent)
        {
            if (Trigger == "Next")
                TriggerNextStep();
            else if(Trigger == "Start")
            {
                Init();
                DialogMMF.gameObject.SetActive(true);
                DialogMMF.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            }
            else if(Trigger == "Exit")
            {
                DisableText();
            }
        }
    }

    public void TriggerNextStep()
    {
        if (NextStepSo())
        {
            StartNextStepAfterDelay();
        }
    }
    public bool NextStepSo()
    {
        if (curBasicStep < BossSequenceSO.Steps.Count - 1)
        {
            curBasicStep++;

            _curBossStepSo = BossSequenceSO.Steps[curBasicStep];
            return true;
        }
        return false;
    }

    
    public void SetTextMeshRroUGUI(TextMeshProUGUI[] TextMeshProGUIs)
    {
        foreach (TextMeshProUGUI textMeshPro in TextMeshProGUIs)
        {
            Color color = textMeshPro.color;
            textMeshPro.color = new Color(color.r, color.g, color.b, 0.0f);
        }
    }
    public void StartNextStepAfterDelay()
    {
        FeedbackSetting(_curBossStepSo);
        StartCoroutine(PlayAfterDelay(_curBossStepSo.DelayTime));
    }

    private IEnumerator PlayAfterDelay(float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);
        Text.text = _curBossStepSo.Text;
        MMFeedback.ResetFeedbacks();
        MMFeedback.PlayFeedbacks();
    }

    public void TextEndEvent()
    { // 현재 텍스트 렌더링 끝
        TutorialEventSystem.OnTextEvents("TextRenderEnd");
    }

    [Header("UI Elements")]
    public TMP_Text[] systemTexts; // 순차 출력용

    [Header("Settings")]
    public float duration;

    IEnumerator PlayOverDriveSequence()
    {
        StartCoroutine(ShowSystemLogs());
        yield return new WaitForSeconds(duration);

        foreach (var txt in systemTexts)
            StartCoroutine(BlinkOut(txt));

    }



    IEnumerator BlinkOut(TMP_Text txt)
    {
        for (int i = 0; i < 10; i++)
        {
            txt.enabled = !txt.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        txt.enabled = false;
    }

    IEnumerator ShowSystemLogs()
    {
        foreach (var txt in systemTexts)
        {
            txt.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
        for (int i = 0; i < systemTexts.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                systemTexts[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, systemTexts[j].GetComponent<RectTransform>().anchoredPosition.y + 30f);
            }
            systemTexts[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(Random.value);
        }

    }
}