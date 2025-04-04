using System.Security.Cryptography;
using UnityEngine;

public class RhythmJudge : MonoBehaviour
{
    public static RhythmJudge Instance { get; private set; }

    [SerializeField] private RhythmPatternSO[] pattern;
    private int currentNoteIndex = 0;
    private float beatDuration => 60f / RhythmManager.Instance.BPM;

    private int _currentNotes => RhythmManager.Instance.StageMusicIndex;


    [Header("판정 범위 (비율)")]
    [SerializeField] private float perfectRange = 0.15f;
    [SerializeField] private float goodRange = 0.30f;
    [SerializeField] private float badRange = 0.45f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!RhythmManager.Instance.IsPlaying) return;

        float now = RhythmManager.Instance.GetCurrentMusicTime();
        
        while (currentNoteIndex < pattern[_currentNotes].notes.Count)
        {
            float noteTime = pattern[_currentNotes].notes[currentNoteIndex].beat * beatDuration;

            if (now - noteTime > beatDuration * badRange)
            {
                Debug.Log($"Miss 판정 (입력 없음) | 노트 시간: {noteTime:F2}s, 현재: {now:F2}s");
                RhythmEvents.InvokeOnInputJudged(JudgementResult.Miss);
                currentNoteIndex++;
            }
            else break;
        }
    }
    private void OnEnable()
    {
        RhythmEvents.OnBeat += OnBeatReceived;
        RhythmInputHandler.Instance.OnInputPerformed += EvaluateInput;
        RhythmEvents.OnMusicStart += OnMusicStartReceived;
    }

    private void OnDisable()
    {
        RhythmEvents.OnBeat -= OnBeatReceived;
        RhythmInputHandler.Instance.OnInputPerformed -= EvaluateInput;
        RhythmEvents.OnMusicStart -= OnMusicStartReceived;
    }

    private void OnMusicStartReceived()
    {
        currentNoteIndex = 0;
    }

    private void OnBeatReceived(float beatTime)
    {
    }

    public void EvaluateInput(string key)
    {
        if (currentNoteIndex >= pattern[_currentNotes].notes.Count)
        {
            Debug.Log("모든 노트 완료");
            return;
        }

        var note = pattern[_currentNotes].notes[currentNoteIndex];
        float currentTime = RhythmManager.Instance.GetCurrentMusicTime();
        float noteTime = note.beat * beatDuration;
        float delta = currentTime - noteTime;
        float abs = Mathf.Abs(delta);
        if (abs > beatDuration * (badRange))
        {
            Debug.Log($"[무시됨] 노트 시간과 입력 시간차 초과 | 오차: {delta:F3}s");
            return;
        }

        if (key != note.expectedKey)
        {
            Debug.Log("잘못된 키");
            return;
        }

        var result = GetJudgement(delta);
        Debug.Log($"{result} | 노트: {noteTime:F3}s, 입력: {currentTime:F3}s, 오차: {delta:F3}s");

        RhythmEvents.InvokeOnInputJudged(result);
        currentNoteIndex++;
    }

    private JudgementResult GetJudgement(float delta)
    {
        float abs = Mathf.Abs(delta);

        if (abs <= beatDuration * perfectRange)
            return JudgementResult.Perfect;
        else if (abs <= beatDuration * goodRange)
            return JudgementResult.Good;
        else if (abs <= beatDuration * badRange)
            return JudgementResult.Bad;
        else
            return JudgementResult.Miss;
    }
}
