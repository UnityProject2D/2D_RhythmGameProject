using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class RhythmShake : MonoBehaviour
{
    [SerializeField] private MMF_Player kickShakeFeedback;

    private void Start()
    {
        RhythmEvents.OnNote += OnBeat;
    }

    private void OnBeat(NoteData data)
    {
        kickShakeFeedback?.PlayFeedbacks();
    }

}
