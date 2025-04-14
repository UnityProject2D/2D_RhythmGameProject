using MoreMountains.Feedbacks;
using UnityEngine;

public class SettingLightManager : MonoBehaviour
{
    private void Start()
    {
        RhythmEvents.OnBeat += PlayOnBeatFeedback;
        if (TitleMusicPlayer != null)
        {
            TitleMusicPlayer.Play();
        }
    }

    private void OnDestroy()
    {
        RhythmEvents.OnBeat -= PlayOnBeatFeedback;
    }

    [Header("MMF 피드백")]
    public MMF_Player LightMMFPlayer;

    public TitleMusicPlayer TitleMusicPlayer;

    public void PlayOnBeatFeedback(float t)
    {
        LightMMFPlayer?.PlayFeedbacks();
    }
}
