using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using UnityEngine.Events;

public class MMFeedbacks : MMF_Player
{
    private Action _onStartAction;
    private Action _onEndAction;

    // Play가 시작되었을 때 호출
    public override void PlayFeedbacks()
    {
        _onStartAction?.Invoke();
        base.PlayFeedbacks();
    }

    // Play가 종료되었을 때 호출
    // ---- MMFeedbacks에서 Update에서 내부적으로 enable = false 호출 ----
    protected override void OnDisable()
    {
        _onEndAction?.Invoke();
        base.OnDisable();
    }

    public void AddOnStartListener(Action listener) => _onStartAction += listener;
    public void AddOnEndListener(Action listener) => _onEndAction += listener;
}
