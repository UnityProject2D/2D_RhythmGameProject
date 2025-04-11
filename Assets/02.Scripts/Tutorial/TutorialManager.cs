using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public TutorialSequenceSO TutorialSequenceSO;

    public MMFeedbacks MMFeedback;
    private MMF_TMPTextReveal textReveal;
    public TextMeshProUGUI Text;

    private int curStep = -1;
    private TutorialStepSo _curTutorialStepSo;

    private Action _nextStep;
    public static TutorialManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);

        RhythmInputHandler.Instance.OnInputPerformed += HandleInput;
        textReveal = MMFeedback.FeedbacksList.OfType<MMF_TMPTextReveal>().FirstOrDefault();
        _nextStep += StartNextStepAfterDelay;
        _nextStep.Invoke();
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

    public void StartNextStep()
    {
        NextStep();
        FeedbackSetting(_curTutorialStepSo);
        StartCoroutine(PlayAfterDelay());
    }

    private IEnumerator PlayAfterDelay(float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);

        Text.text = _curTutorialStepSo.Text;
        MMFeedback.ResetFeedbacks();
        MMFeedback.PlayFeedbacks();
    }

    private void HandleInput(string key)
    {
        if(key == "W")
            StartNextStep();
    }
}


