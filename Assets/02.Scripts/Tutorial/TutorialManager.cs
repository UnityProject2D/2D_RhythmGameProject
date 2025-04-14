using Cysharp.Threading.Tasks;
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
    public TextMeshProUGUI PressSpaceText;

    private bool _isPaused;
    public int curBasicStep = -1;
    public int curLoopStep = -1;
    private TutorialStepSo _curTutorialStepSo;

    public ParticleSystem[] CompleteParticles;
    public SFXSound SFXSound;

    private Bus masterBus;

    //public Button SinkEndButton;
    //public GameObject SinkPanel;

    public GameObject RhythmJudgePanel;

    public GameObject[] TutorialUI;
    public DoorController DoorController;
    public GameObject[] Lights;
    public static TutorialManager Instance { get; private set; }
    private void Awake(){
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);

        masterBus = RuntimeManager.GetBus("bus:/BGM");
    }
    private void Init()
    {
        _textReveal = MMFeedback.FeedbacksList.OfType<MMF_TMPTextReveal>().FirstOrDefault();
        _curTutorialStepSo = TutorialSequenceSO.Steps[0];
        TriggerNextStep();
        HandleInput().Forget();
    }

    private async UniTaskVoid HandleInput()
    {
        while(curBasicStep < TutorialSequenceSO.Steps.Count)
        {
            ButtonClick();
            await UniTask.Yield();
        }
        
    }
    private void OnEnable(){
        TutorialEventSystem.OnTutorialTextEvent += HandleEvent;
        RhythmEvents.OnMarkerHit += HandleEvent;
        OnMusicReady += Init;
        OnNotePreview += OnNoteReceived;
        OnNote += DisableText;
    }

    private void OnDisable(){
        TutorialEventSystem.OnTutorialTextEvent -= HandleEvent;
        OnNotePreview -= OnNoteReceived;
        OnNote -= DisableText; 
        OnMusicReady -= Init;
        OnMarkerHit -= HandleEvent;
    }

    private void DisableText(NoteData note)
    {
        foreach(var pressKey in PressKey)
        {

            Color color = pressKey.color;
            pressKey.color = new Color(color.r, color.g, color.b, 0.0f);
        }
    }


    public void FeedbackSetting(TutorialStepSo tutorialStepSo){
        if (_textReveal == null) return;
        _textReveal.RevealDuration = tutorialStepSo.Duration;
        if(tutorialStepSo.TextNextConditionType == TextNextConditionType.OnInput || tutorialStepSo.TextNextConditionType == TextNextConditionType.OnTimeElapsedOrInput)
        {
            PressSpaceText.gameObject.SetActive(true); 
        }
        else
        {
            PressSpaceText.gameObject.SetActive(false);
        }
    }

    public void HandleEvent(string Trigger = ""){
        switch (_curTutorialStepSo.TextNextConditionType){
            case TextNextConditionType.OnTimeElapsedOrInput:
                if(Trigger == "TextRenderEnd" || Trigger == "MouseClick")
                {
                    
                    TriggerNextStep();
                }
                    
                break;
            case TextNextConditionType.OnEvent: // OnMarkerHit 이벤트 받을 때
                if(Trigger != "MouseClick" && Trigger!="TextRenderEnd")
                    TriggerNextStep();
                break;
            case TextNextConditionType.OnButtonClick: // 버튼 클릭시 사라지도록
                if (Trigger == "SinkButton")
                    TriggerNextStep();
                break;
            case TextNextConditionType.OnInput:
                if (Trigger != "SinkButton" && Trigger != "TextRenderEnd")
                    TriggerNextStep();
                if (_curTutorialStepSo.MusicState == MusicState.Play)
                {
                    RhythmManager.Instance.Play();
                }
                break;
        }
    }

    public void TriggerNextStep(){
        if (NextStepSo()){
            StartNextStepAfterDelay();
        }
        else
        {
            StartCoroutine(NextStage());
        }
    }

    public IEnumerator NextStage()
    {

        foreach (var light in Lights)
        {
            light.SetActive(false);

            RuntimeManager.PlayOneShot("event:/SFX/LightOff");
            yield return new WaitForSeconds(0.5f);
        }
        Lights[^1].SetActive(true);
        DoorController.Open();

        yield return new WaitForSeconds(1f);

        GameSceneManager.Instance.ChangeScene("VFXTest");
    }
    public void OnOffTutorialUI(bool Active)
    {
        foreach (GameObject ui in TutorialUI)
        {
            ui.SetActive(Active);
        }
        
    }
    public bool NextStepSo()
    {
        switch (_curTutorialStepSo.NextSquence)
        {
            case NextSquence.Basic:
                {
                    return TutorialBasic();
                }

            case NextSquence.Loop:
                {
                    return TutorialLoop();
                }
        }
        return true;
    }

    public bool TutorialBasic()
    {
        if (curBasicStep < TutorialSequenceSO.Steps.Count-1)
        {
            curBasicStep++;

            _curTutorialStepSo = TutorialSequenceSO.Steps[curBasicStep];
            return true;
        }

        OnOffTutorialUI(false);
        return false;
    }

    public bool TutorialLoop()
    {
        if (++curLoopStep >= TutorialSequenceSO.Loops.Count)
        {
            curLoopStep = 0;
        }

        _curTutorialStepSo = TutorialSequenceSO.Loops[curLoopStep];

        return true;
    }
    public void SetTextMeshRroUGUI(TextMeshProUGUI[] TextMeshProGUIs)
    {
        foreach (TextMeshProUGUI textMeshPro in TextMeshProGUIs)
        {
            Color color = textMeshPro.color;
            textMeshPro.color = new Color(color.r, color.g, color.b, 0.0f);
        }
    }
    public void StartNextStepAfterDelay(){
        FeedbackSetting(_curTutorialStepSo);
        StartCoroutine(PlayAfterDelay(_curTutorialStepSo.DelayTime));
    }

    private IEnumerator PlayAfterDelay(float delay = 0.0f){
        yield return new WaitForSeconds(delay);

        // 음악 셋팅
        //MusicSetting();

        if (_curTutorialStepSo.TextNextConditionType == TextNextConditionType.OnButtonClick){ // 버튼 클릭 이벤트가 있을 경우
            //OnOffSinkUI(true);
            // 저지 키기
            RhythmJudgePanel.SetActive(true);
        }

        Text.text = _curTutorialStepSo.Text;
        MMFeedback.ResetFeedbacks();
        MMFeedback.PlayFeedbacks();
    }



    public void TextEndEvent(){ // 현재 텍스트 렌더링 끝
        TutorialEventSystem.OnTextEvents("TextRenderEnd");
    }

    private void OnNoteReceived(NoteData beatNote){
        PressKey[0].text = beatNote.expectedKey;
        foreach (TextMeshProUGUI pressKey in PressKey){
            Color color = pressKey.color;
            pressKey.color = new Color(color.r, color.g, color.b, 1.0f);
        }
    }

    public void ButtonClick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            HandleEvent("MouseClick");
    }
}