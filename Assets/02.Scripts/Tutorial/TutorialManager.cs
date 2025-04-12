using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using static RhythmEvents;

public class TutorialManager : MonoBehaviour
{
    public TutorialSequenceSO TutorialSequenceSO;

    public MMFeedbacks MMFeedback;
    private MMF_TMPTextReveal _textReveal;
    public TextMeshProUGUI Text;

    public TextMeshProUGUI[] PressKey;

    private bool _isPaused = false;
    private int curStep = 5;//-1;
    private TutorialStepSo _curTutorialStepSo;

    public ParticleSystem[] CompleteParticles;
    public SFXSound SFXSound;

    private Bus masterBus;
    public static TutorialManager Instance { get; private set; }
    private void Awake(){
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);

        masterBus = RuntimeManager.GetBus("bus:/");
    }
    private void Start()
    {
        RhythmInputHandler.Instance.OnInputPerformed += HandleInput;

        _textReveal = MMFeedback.FeedbacksList.OfType<MMF_TMPTextReveal>().FirstOrDefault();
        TriggerNextStep();
    }
    private void OnEnable(){
        TutorialEventSystem.OnTutorialTextEvent += HandleEvent;
        // RhythmEvents.OnInputJudged += OnJudged; // 판정 이벤트 구독
        // OnNotePreview += OnNotePreviewReceived;
        OnNote += OnNoteReceived;

    }

    private void OnDisable(){
        TutorialEventSystem.OnTutorialTextEvent -= HandleEvent;
        // RhythmEvents.OnInputJudged -= OnJudged; // 판정 이벤트 구독
        // OnNotePreview -= OnNotePreviewReceived;
        OnNote -= OnNoteReceived;
    }

    private void SetPause(bool isPaused){
        if (_isPaused == isPaused) // 같을 경우 제외
            return;
        _isPaused = isPaused;
        masterBus.setPaused(_isPaused);
    }


    public void FeedbackSetting(TutorialStepSo tutorialStepSo){
        if (_textReveal == null) return;
        _textReveal.RevealDuration = tutorialStepSo.Duration;
        PressKey[0].text = tutorialStepSo.TriggerKeyType.ToString();
    }

    public void HandleEvent(string Trigger = ""){
        switch (_curTutorialStepSo.TextNextConditionType){
            case TextNextConditionType.OnTimeElapsedOrInput:
                TriggerNextStep();
                break;
            case TextNextConditionType.OnEvent: // 트리거랑 같을 때

                // 해당 트리거 이벤트 성공 여부
                if(_curTutorialStepSo.TriggerKeyType.ToString() == Trigger && _isPaused)
                {
                    foreach (TextMeshProUGUI pressKey in PressKey){
                        pressKey.gameObject.SetActive(false);
                    }

                    SetPause(false);
                    SFXSound.Play();
                    // RuntimeManager.PlayOneShot("event:/SFX/Upgrade");
                    Time.timeScale = 1.0f;
                    TriggerNextStep();
                    foreach (ParticleSystem particle in CompleteParticles){
                        particle.Play();
                    }

                }
                break;
        }
    }

    public void TriggerNextStep(){
        if (NextStepSo()){
            StartNextStepAfterDelay();
        }
    }

    public bool NextStepSo()
    {
        if (++curStep >= TutorialSequenceSO.Steps.Count){
            return false;
        }
        _curTutorialStepSo = TutorialSequenceSO.Steps[curStep];
        return true;
    }
    public void StartNextStepAfterDelay(){
        FeedbackSetting(_curTutorialStepSo);
        StartCoroutine(PlayAfterDelay(_curTutorialStepSo.DelayTime));
    }

    private IEnumerator PlayAfterDelay(float delay = 0.0f){
        yield return new WaitForSeconds(delay);

        // 음악 셋팅
        MusicSetting();

        Text.text = _curTutorialStepSo.Text;
        MMFeedback.ResetFeedbacks();
        MMFeedback.PlayFeedbacks();
    }

    public void MusicSetting()
    {
        if (_curTutorialStepSo.MusicState == MusicState.Stop ||
            _curTutorialStepSo.MusicState == MusicState.NonPlay) { 
            SetPause(true);
        }
        else{
            SetPause(false);
        }
    }

    // 키 입력 들어옴.
    private void HandleInput(string key){
        TutorialEventSystem.OnTextEvents(key); // 키 관련 이벤트
    }

    public void TextEndEvent(){ // 현재 텍스트 렌더링 끝
        TutorialEventSystem.OnTextEvents();
    }

    private void OnNoteReceived(NoteData beatNote){
        foreach (TextMeshProUGUI pressKey in PressKey){
            pressKey.gameObject.SetActive(true);
        }
        SetPause(true);
        Time.timeScale = 0.0f;
    }
    
}