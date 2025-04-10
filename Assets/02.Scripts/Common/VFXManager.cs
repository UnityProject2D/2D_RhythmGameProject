using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        LightMMFPlayers = new List<MMF_Player>();
        RhythmEvents.OnBeat += PlayOnBeatFeedback;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += DestroyOnRestart;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= DestroyOnRestart;
    }

    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameTitle")
        {
            Destroy(gameObject);
        }
    }


    [Header("MMF 피드백")]
    public MMF_Player[] OnNoteFeedback;
    public MMF_Player OnGoodFeedback;
    public MMF_Player OnPerfectFeedback;
    public MMF_Player ExplosionFeedback;
    public MMF_Player hitFlashFeedback;
    public List<MMF_Player> LightMMFPlayers;
   
    public void PlayOnBeatFeedback(float t)
    {
        Debug.Log("PlayOnBeatFeedback");
        foreach (var mmfPlayer in LightMMFPlayers)
        {
            mmfPlayer?.PlayFeedbacks();
        }
    }
    public void PlayOnNoteFeedback()
    {
        Debug.Log("PlayOnNoteFeedback");
    }
    public void PlayOnPerfectFeedback() => OnPerfectFeedback?.PlayFeedbacks();
    public void PlayOnGoodFeedback() => OnGoodFeedback?.PlayFeedbacks();
    public void PlayExplosionFeedback() => ExplosionFeedback?.PlayFeedbacks();
    public void PlayhitFlashFeedback() => hitFlashFeedback?.PlayFeedbacks();

}
