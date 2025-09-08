using UnityEngine;

public class UI_Game : MonoBehaviour
{
    public UIResultPanel resultPanel;
    private void Start()
    {
        //ShowResultPanel();
    }

    public bool IsShowing;
    private void JudgeEnd(string marker)
    {
        if (marker == "End")
        {
            ShowResultPanel();
        }
    }
    private void OnEnable()
    {
        RhythmEvents.OnMusicStopped += ShowResultPanel;
        RhythmEvents.OnMarkerHit += JudgeEnd;
    }
    private void OnDisable()
    {
        RhythmEvents.OnMusicStopped -= ShowResultPanel;

        RhythmEvents.OnMarkerHit -= JudgeEnd;
    }

    private void ShowResultPanel()
    {
        if (IsShowing) return;

        IsShowing = true;
        resultPanel.gameObject.SetActive(true);
        resultPanel.PlayOpenEffect();
    }
}
