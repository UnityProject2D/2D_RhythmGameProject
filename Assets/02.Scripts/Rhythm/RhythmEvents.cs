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

public static class RhythmEvents
{
    /// <summary>
    /// 정박 비트마다 호출 (예: 1, 2, 3, 4...)
    /// </summary>
    public static event Action OnBeat;

    /// <summary>
    /// 반박 또는 서브 비트 호출 (예: 1 & 2 & 3 & ...)
    /// </summary>
    public static event Action OnSubBeat;

    /// <summary>
    /// 리듬 입력에 대한 판정 결과
    /// </summary>
    public static event Action<JudgementResult> OnInputJudged;

    /// <summary>
    /// 특정 마커가 통과됐을 때 호출 (FMOD Marker 이름 기반)
    /// </summary>
    public static event Action<string> OnMarkerHit;

    // ===== Invoke =====

    public static void InvokeOnBeat() => OnBeat?.Invoke();
    public static void InvokeOnSubBeat() => OnSubBeat?.Invoke();
    public static void InvokeOnInputJudged(JudgementResult result) => OnInputJudged?.Invoke(result);
    public static void InvokeOnMarkerHit(string name) => OnMarkerHit?.Invoke(name);
}
