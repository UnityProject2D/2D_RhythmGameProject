using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using FronkonGames.Glitches.Artifacts;

public class VFXManager : MonoBehaviour
{

    private Artifacts _artifactsSettings;
    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        LightMMFPlayers = new List<MMF_Player>();
        RhythmEvents.OnBeat += PlayOnBeatFeedback;
        RhythmEvents.OnMarkerHit += Add;
        if (_artifactsSettings == null)
        {
            _artifactsSettings = Artifacts.Instance;
            if (_artifactsSettings == null)
            {
                Debug.LogError("Settings not found in VFXManager.");
            }
        }
    }
    private void OnDisable()
    {
        RhythmEvents.OnBeat -= PlayOnBeatFeedback;
        RhythmEvents.OnMarkerHit -= Add;
        _artifactsSettings.SetActive(false);
    }
    private int page;
    private void Add(string marker)
    {
        if (marker == "Start") page++;
    }
    private void Start()
    {
        PlayerState.Instance.OnItemUsed += HandleItemEffect;
    }

    void HandleItemEffect(ItemUseStatus item)
    {
        switch(item.itemID)
        {
            case ItemID.EmergencyEvasion:
                
                break;
            default:
                break;
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
        foreach (var mmfPlayer in LightMMFPlayers)
        {
            mmfPlayer?.PlayFeedbacks();
        }
    }
    public void PlayOnNoteFeedback()
    {
    }
    public void PlayOnPerfectFeedback()
    {
        if (page != 1)
        {
            OnPerfectFeedback?.PlayFeedbacks();
        }
        else
        {
            ExplosionFeedback?.PlayFeedbacks();
        }
    }
    public void PlayOnGoodFeedback()
    {
        if(page != 1)
        {
            OnGoodFeedback?.PlayFeedbacks();
        }
        else
        {
            ExplosionFeedback?.PlayFeedbacks();
        }
    }
    public void PlayExplosionFeedback() => ExplosionFeedback?.PlayFeedbacks();
    public void PlayhitFlashFeedback() => hitFlashFeedback?.PlayFeedbacks();

    public void SetArtifacts(bool flag)
    {
        if (_artifactsSettings == null)
        {
            Debug.LogError("Settings not found in VFXManager.");
            return;
        }
        _artifactsSettings.SetActive(flag);
    }

}
