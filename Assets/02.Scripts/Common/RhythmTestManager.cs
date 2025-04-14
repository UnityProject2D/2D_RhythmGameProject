using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MoreMountains.Feedbacks;

public class RhythmTestManager : MonoBehaviour
{
    public float BPM = 120f;
    private float beatInterval => 60f / BPM;
    List<float> offsetList = new();
    float lastBeatTime;
    public TextMeshProUGUI MinusText;
    public TextMeshProUGUI OffsetText;
    public TextMeshProUGUI MeanErrorText;
    public TextMeshProUGUI MsText;
    private @PlayerInputAction inputActions;
    public GameObject Canvas;
    public GameObject Canvas2;
    public Button button;
    public RectTransform circle;
    public bool Audio;
    public MMF_Player MMF_Player;
    private void OnEnable()
    {
        RhythmEvents.OnBeat += OnBeatHandler;
        if(circle != null)
        {
            StartCoroutine(StartVideoCalibration());
        }
    }

    private IEnumerator StartVideoCalibration()
    {
        while(circle.gameObject.activeSelf)
        {
            circle.localScale = Vector3.one * 0.2f;
            circle.DOScale(1f, 1f).SetEase(Ease.Linear).OnComplete(()=> lastBeatTime = Time.time);
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void Start()
    {

        inputActions = new @PlayerInputAction();

        inputActions.Setting.Enable();
        inputActions.Setting.Space.performed += OnUserPressedKey;
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        inputActions.Setting.Disable();
    }
    void OnBeatHandler(float t)
    {
        lastBeatTime = Time.time;
    }


    public void OnUserPressedKey(InputAction.CallbackContext callback)
    {

        float now = Time.time;
        float beatIndex = Mathf.Round(now / beatInterval);
        float closestBeatTime = beatIndex * beatInterval;
        float delta = (now - closestBeatTime) * 1000f;
        if (Audio)
        {
            beatIndex = Mathf.Round(now / beatInterval);
            closestBeatTime = beatIndex * beatInterval;
            delta = (now - closestBeatTime) * 1000f;
            MMF_Player?.PlayFeedbacks();
        }
        else
        {
            delta = (now + SyncSettings.InputOffsetMs / 1000f) - lastBeatTime;
        }
        
        offsetList.Add(delta);

        MinusText.text = delta < 0 ? "-" : "+";
        OffsetText.text = Mathf.Abs(delta).ToString("F1");
        MsText.text = "ms";

        if (offsetList.Count >= 5)
        {
            float avg = offsetList.Average();
            MeanErrorText.text = $"평균 오차: {avg:F1}ms";
        }
    }

    public void OnClick()
    {
        Canvas.SetActive(false);

        if (Canvas2 != null)
        {
            Canvas2.SetActive(true);
        }
        else
        {
            circle.gameObject.SetActive(false);
            PlayerPrefs.SetFloat("AudioSync", SyncSettings.InputOffsetMs);
            PlayerPrefs.SetFloat("VideoSync", SyncSettings.VideoOffsetMs);
            SceneManager.LoadScene("GameTitle");
        }
    }

}
