using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
public class PlayerContext
{
    public PlayerController Controller = null;
    public PlayerHealth Health = null;
    public Transform Transform = null;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Target;
    public PlayerContext Player = new PlayerContext();
    public event Action PlayerRegistered;
    public ItemFlag SavedItemFlags;
    public float PlayerHealth = 10;
    public ItemSO[] SavedItems;

    public void RegisterPlayer(PlayerController controller)
    {
        Player.Controller = controller;
        Player.Health = controller.GetComponent<PlayerHealth>();
        Player.Transform = controller.transform;

        PlayerRegistered?.Invoke();
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);



        SavedItems = new ItemSO[2];
    }


    //추후 GameResultHandler로 분리 예정
    #region GameResult
    [SerializeField] private double winScoreThreshold = 10000; // 승리 기준 점수
    private void OnEnable()
    {
        RhythmEvents.OnMusicStopped += OnMusicStopped;

        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정 // 추후 SceneCleanupHandler로 분리 예정
    }

    private void OnDisable()
    {
        RhythmEvents.OnMusicStopped -= OnMusicStopped;
        SceneManager.sceneLoaded -= DestroyOnRestart;
    }

    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        Player = new PlayerContext();
        Debug.Log($"PlayerContext 초기화: {Player.Transform} {Player.Health} {Player.Controller}");
        if (scene.name == "GameTitle")
        {
            Destroy(gameObject);
        }
    }
    private void OnMusicStopped()
    {
        Debug.Log("음악 끝. 3초 뒤 결과 출력");
        StartCoroutine(HandleResultAfterDelay());
    }

    private IEnumerator HandleResultAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        double totalScore = ScoreManager.Instance.TotalScore;

        if (totalScore >= winScoreThreshold)
        {
            Debug.Log($"승리! 총 점수: {totalScore}");
        }
        else
        {
            Debug.Log($"패배 총 점수: {totalScore}");
        }
    }
    #endregion
}
