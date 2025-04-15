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
    public static void SafeInvokeOnMusicStart()
    {
        try { OnMusicStart?.Invoke(); }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnMusicStart Error: {ex.Message}"); }
    }

    public static void SafeInvokeOnMusicStopped()
    {
        if (!RhythmManager.Instance.IsRestart)
        {
            try { OnMusicStopped?.Invoke(); }
            catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnMusicStopped Error: {ex.Message}"); }
        }
    }

    public static void SafeInvokeOnBeat(float beat)
    {
        try { OnBeat?.Invoke(beat); }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnBeat Error: {ex.Message}"); }
    }

    public static void SafeInvokeOnSubBeat()
    {
        try { OnSubBeat?.Invoke(); }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnSubBeat Error: {ex.Message}"); }
    }

    public static void SafeInvokeOnInputJudged(JudgementResult result)
    {
        try
        {
            OnInputJudged?.Invoke(new JudgedContext
            {
                Result = result,
                RandomValue = UnityEngine.Random.value
            });
        }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnInputJudged Error: {ex.Message}"); }
    }

    public static void SafeInvokeOnMarkerHit(string name)
    {
        try { OnMarkerHit?.Invoke(name); }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnMarkerHit Error: {ex.Message}"); }
    }

    public static void SafeInvokeOnNotePreview(NoteData note)
    {
        try { OnNotePreview?.Invoke(note); }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnNotePreview Error: {ex.Message}"); }
    }

    public static void SafeInvokeOnNote(NoteData note)
    {
        try { OnNote?.Invoke(note); }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnNote Error: {ex.Message}"); }
    }

    public static void SafeInvokeOnMusicReady()
    {
        try { OnMusicReady?.Invoke(); }
        catch (Exception ex) { Debug.LogError($"[RhythmEvents] OnMusicReady Error: {ex.Message}"); }
    }

    public static void ClearAllSubscriptions()
    {
        OnBeat = null;
        OnSubBeat = null;
        OnNote = null;
        OnInputJudged = null;
        OnMarkerHit = null;
        OnNotePreview = null;
        OnMusicStart = null;
        OnMusicStopped = null;
    }
}
