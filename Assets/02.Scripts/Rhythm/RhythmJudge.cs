using UnityEngine;
using System;

public class RhythmJudge : MonoBehaviour
{
    public static RhythmJudge Instance { get; private set; }

    [SerializeField] private RhythmPatternSO pattern;
    private int currentNoteIndex = 0;
    private float beatDuration;

    [Header("판정 시간 범위 (초)")]
    [SerializeField] private float _perfectRange = 0.3f;
    [SerializeField] private float _goodRange = 0.6f;
    [SerializeField] private float _badRange = 0.9f;

    private float _lastBeatTime;
    private float _bpm => RhythmManager.Instance != null ? RhythmManager.Instance.BPM : 120f;
    private float _beatDuration => 60f / _bpm;
    private bool _isInputReceived = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        RhythmEvents.OnBeat += UpdateBeatTime;
        RhythmInputHandler.Instance.OnInputPerformed += EvaluateInput;
    }

    void OnDisable()
    {
        RhythmEvents.OnBeat -= UpdateBeatTime;
        RhythmInputHandler.Instance.OnInputPerformed -= EvaluateInput;
    }

    /// <summary>
    /// 최근 비트 시간 저장 (리듬 판정 기준점)
    /// </summary>
    private void UpdateBeatTime()
    {
        _lastBeatTime = RhythmManager.Instance.GetCurrentMusicTime();
        _isInputReceived = false;
    }

    private void CheckMiss()
    {
        if (!_isInputReceived)
        {
            Debug.Log("입력 없음 → Miss 판정");
            RhythmEvents.InvokeOnInputJudged(JudgementResult.Miss);
        }
    }

    public void EvaluateInput(string inputKey)
    {
        if (currentNoteIndex >= pattern.notes.Count) return;

        NoteData note = pattern.notes[currentNoteIndex];
        float inputTime = RhythmManager.Instance.GetCurrentMusicTime();
        float noteTime = note.beat * beatDuration;
        float delta = inputTime - noteTime;

        if (inputKey != note.expectedKey)
        {
            Debug.Log("틀린 키!");
            return;
        }

        JudgementResult result = JudgeByDelta(delta);
        Debug.Log($"{result} | 오차: {delta:F3}");

        currentNoteIndex++;
    }

    private JudgementResult JudgeByDelta(float delta)
    {
        float abs = Mathf.Abs(delta);
        if (abs <= beatDuration * _perfectRange)
            return JudgementResult.Perfect;
        else if (abs <= beatDuration * _goodRange)
            return JudgementResult.Good;
        else if (abs <= beatDuration * _badRange)
            return JudgementResult.Bad;
        else
            return JudgementResult.Miss;
    }
}
