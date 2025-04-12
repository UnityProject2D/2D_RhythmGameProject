using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public GameObject LoadingUI;
    public Image LoadingBar;
    public GameObject LoadingBarBack;
    public Image FadeImage;
    public GameObject LoadingText;
    public GameObject LoadCompleteText;
    public static GameSceneManager Instance { get; private set; }

    public event Action<StageData> OnStageDataLoaded;
    void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadStageData(StageData stageData)
    {
        // 스테이지 데이터 로드
        OnStageDataLoaded?.Invoke(stageData);
    }


    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithLoadingScene(sceneName));
    }

    private IEnumerator LoadSceneWithLoadingScene(string targetScene)
    {

        if(RhythmManager.Instance != null)
        {
            RhythmManager.Instance.StopMusic();
            RhythmManager.Instance.IsPlaying = false;
        }
        LoadingUI.SetActive(true);
        // 1. 로딩 씬 먼저 로드
        AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync("LoadingScene");
        yield return new WaitUntil(() => loadingSceneOp.isDone);

        // 2. 이제 로딩 UI를 보여주고 다음 씬 비동기 로딩 시작
        StartCoroutine(LoadSceneAsync(targetScene));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        

        // 비동기 씬 로딩
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        asyncLoad.allowSceneActivation = false;


        LoadingText.SetActive(true);
        while (asyncLoad.progress < 0.9f)
        {
            LoadingBar.fillAmount = asyncLoad.progress / 0.9f; // 0.0 ~ 1.0 으로 맞추기
            yield return null;
        }
        LoadingBar.fillAmount = 1f;

        LoadingText.SetActive(false);

        LoadCompleteText.SetActive(true);
    yield return new WaitUntil(() => Input.anyKeyDown);

        LoadCompleteText.SetActive(false);
        LoadingBar.gameObject.SetActive(false);
        LoadingBarBack.SetActive(false);
        asyncLoad.allowSceneActivation = true;
        yield return StartCoroutine(FadeOutScreen());
    }

    private IEnumerator FadeOutScreen()
    {
        Color color = FadeImage.color;
        float alpha = color.a;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            FadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        LoadingUI.SetActive(false);
    }
}
