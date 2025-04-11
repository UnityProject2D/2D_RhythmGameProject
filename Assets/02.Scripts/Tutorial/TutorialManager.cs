using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

public class TutorialManager : MonoBehaviour
{
    public TutorialSequenceSO TutorialSequenceSO;

    public MMFeedbacks MMFeedback;
    private MMF_TMPTextReveal textReveal;
    public TextMeshProUGUI Text;

    private int curStep = -1;
    private TutorialStepSo _curTutorialStepSo;
    public static TutorialManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);

        textReveal = MMFeedback.FeedbacksList.OfType<MMF_TMPTextReveal>().FirstOrDefault();
        StartNextStepAfterDelay();
    }

    public void FeedbackSetting(TutorialStepSo tutorialStepSo)
    {
        textReveal.RevealDuration = tutorialStepSo.Duration;
    }

    public void NextStep()
    {
        if (++curStep >= TutorialSequenceSO.Steps.Count)
            return;

        _curTutorialStepSo = TutorialSequenceSO.Steps[curStep];
        Debug.Log($"현재 nextStep: {curStep}");
    }

    public void StartNextStepAfterDelay()
    {
        NextStep();
        FeedbackSetting(_curTutorialStepSo);
        StartCoroutine(PlayAfterDelay(_curTutorialStepSo.DelayTime));
    }

    private IEnumerator PlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Text.text = _curTutorialStepSo.Text;
        MMFeedback.PlayFeedbacks();
    }
}
