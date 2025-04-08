using MoreMountains.Feedbacks;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("MMF 피드백")]
    public MMF_Player[] OnNoteFeedback;
    public MMF_Player OnGoodFeedback;
    public MMF_Player OnPerfectFeedback;
    public MMF_Player ExplosionFeedback;
    public MMF_Player hitFlashFeedback;
   

    public void PlayOnNoteFeedback()
    {
        foreach(var feedback in OnNoteFeedback)
        {
            feedback?.PlayFeedbacks();
        }
    }
    public void PlayOnPerfectFeedback() => OnPerfectFeedback?.PlayFeedbacks();
    public void PlayOnGoodFeedback() => OnGoodFeedback?.PlayFeedbacks();
    public void PlayExplosionFeedback() => ExplosionFeedback?.PlayFeedbacks();
    public void PlayhitFlashFeedback() => hitFlashFeedback?.PlayFeedbacks();

}
