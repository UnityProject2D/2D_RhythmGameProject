using System;
using UnityEngine;

public enum JudgementResult
{
    Perfect,
    Good,
    Bad,
    Miss,

    Count
}

public struct JudgedContext
{
    public JudgementResult Result;
    public float RandomValue;
}

public static class RhythmEvents
{
    /// <summary>
    /// 정박 비트마다 호출 (예: 1, 2, 3, 4...)
    /// </summary>
    public static event Action<float> OnBeat;

    /// <summary>
    /// 반박 또는 서브 비트 호출 (예: 1 & 2 & 3 & ...)
    /// </summary>
    public static event Action OnSubBeat;

    /// <summary>
    /// Note 마다 호출
    /// </summary>
    public static event Action<NoteData> OnNote;

    /// <summary>
    /// 리듬 입력에 대한 판정 결과
    /// </summary>
    public static event Action<JudgedContext> OnInputJudged;

    /// <summary>
    /// 특정 마커가 통과됐을 때 호출 (FMOD Marker 이름 기반)
    /// </summary>
    public static event Action<string> OnMarkerHit;

    /// <summary>
    /// Note의 한마디 전 호출
    /// </summary>
    public static event Action<NoteData> OnNotePreview;



    /// <summary>
    /// 음악이 시작될 때 호출
    /// </summary>
    public static event Action OnMusicStart;


    /// <summary>
    /// 음악이 끝날 때 호출
    /// </summary>
    public static event Action OnMusicStopped;

    public static event Action OnMusicReady;

    // ===== Invoke =====
    public static void InvokeOnMusicStart() => OnMusicStart?.Invoke();
    public static void InvokeOnMusicStopped()
    {
        if (!RhythmManager.Instance.IsRestart) OnMusicStopped?.Invoke();
    }
    
    public static void InvokeOnBeat(float beat)
    {
        OnBeat?.Invoke(beat);
    }
    public static void InvokeOnSubBeat() => OnSubBeat?.Invoke();
    public static void InvokeOnInputJudged(JudgementResult result)
    {
        OnInputJudged?.Invoke(new JudgedContext
        {
            RandomValue = UnityEngine.Random.value,
            Result = result
        });
    }
    public static void InvokeOnMarkerHit(string name) => OnMarkerHit?.Invoke(name);

    public static void InvokeOnNotePreview(NoteData note) => OnNotePreview?.Invoke(note);
    public static void InvokeOnNote(NoteData note) => OnNote?.Invoke(note); 
    
    public static void InvokeOnMusicReady() => OnMusicReady?.Invoke();
}
