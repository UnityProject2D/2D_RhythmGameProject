using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class TutorialManager : MonoBehaviour
{
    public TutorialSequenceSO TutorialSequenceSO;

    public MMFeedbacks MMFeedback;
    private MMF_TMPTextReveal _textReveal;
    public TextMeshProUGUI Text;

    private int curStep = -1;
    private TutorialStepSo _curTutorialStepSo;

    public static TutorialManager Instance { get; private set; }
    private void Awake(){
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        RhythmInputHandler.Instance.OnInputPerformed += HandleInput;

        _textReveal = MMFeedback.FeedbacksList.OfType<MMF_TMPTextReveal>().FirstOrDefault();
        TriggerNextStep();
    }
    private void OnEnable(){
        TutorialEventSystem.OnTutorialTextEvent += HandleEvent;
    }

    private void OnDisable(){
        TutorialEventSystem.OnTutorialTextEvent -= HandleEvent;
    }


    public void FeedbackSetting(TutorialStepSo tutorialStepSo){
        if (_textReveal == null) return;
        _textReveal.RevealDuration = tutorialStepSo.Duration;
    }

    public void HandleEvent(string Trigger = ""){
        switch (_curTutorialStepSo.TextNextConditionType){
            case TextNextConditionType.OnTimeElapsedOrInput:
                TriggerNextStep();
                break;
            case TextNextConditionType.OnEvent: // 트리거랑 같을 때
                if(_curTutorialStepSo.TriggerKeyType.ToString() == Trigger){
                    TriggerNextStep();
                }
                break;
            case TextNextConditionType.TextNextConditionType_End:
                break;
        }
    }

    public void TriggerNextStep(){
        if(NextStepSo()) StartNextStepAfterDelay();
    }

    public bool NextStepSo(){
        if (++curStep >= TutorialSequenceSO.Steps.Count) return false;
        _curTutorialStepSo = TutorialSequenceSO.Steps[curStep];
        return true;
    }
    public void StartNextStepAfterDelay(){
        FeedbackSetting(_curTutorialStepSo);
        StartCoroutine(PlayAfterDelay(_curTutorialStepSo.DelayTime));
    }

    private IEnumerator PlayAfterDelay(float delay = 0.0f){
        yield return new WaitForSeconds(delay);
        Text.text = _curTutorialStepSo.Text;
        MMFeedback.ResetFeedbacks();
        MMFeedback.PlayFeedbacks();
    }

    // 키 입력 들어옴.
    private void HandleInput(string key){
        TutorialEventSystem.OnTextEvents(key); // 키 관련 이벤트
    }

    public void TextEndEvent(){ // 현재 텍스트 렌더링 끝
        TutorialEventSystem.OnTextEvents();
    }
}