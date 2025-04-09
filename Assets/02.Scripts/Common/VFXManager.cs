using MoreMountains.Feedbacks;
using UnityEngine;
using System.Collections.Generic;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        LightMMFPlayers = new List<MMF_Player>();
    }

    [Header("MMF 피드백")]
    public MMF_Player[] OnNoteFeedback;
    public MMF_Player OnGoodFeedback;
    public MMF_Player OnPerfectFeedback;
    public MMF_Player ExplosionFeedback;
    public MMF_Player hitFlashFeedback;
    public List<MMF_Player> LightMMFPlayers;
   
    public void PlayOnNoteFeedback()
    {
        Debug.Log("PlayOnNoteFeedback");
        foreach (var mmfPlayer in LightMMFPlayers)
        {
            mmfPlayer?.PlayFeedbacks();
        }
    }
    public void PlayOnPerfectFeedback() => OnPerfectFeedback?.PlayFeedbacks();
    public void PlayOnGoodFeedback() => OnGoodFeedback?.PlayFeedbacks();
    public void PlayExplosionFeedback() => ExplosionFeedback?.PlayFeedbacks();
    public void PlayhitFlashFeedback() => hitFlashFeedback?.PlayFeedbacks();

}
