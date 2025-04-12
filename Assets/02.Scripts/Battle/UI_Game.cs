using UnityEngine;

public class UI_Game : MonoBehaviour
{
    public UIResultPanel resultPanel;
    private void Start()
    {
        //ShowResultPanel();
    }

    private void OnEnable()
    {
        RhythmEvents.OnMusicStopped += ShowResultPanel;

    }
    private void OnDisable()
    {
        RhythmEvents.OnMusicStopped -= ShowResultPanel;
    }

    private void ShowResultPanel()
    {
        resultPanel.gameObject.SetActive(true);
        resultPanel.PlayOpenEffect();
    }
}
