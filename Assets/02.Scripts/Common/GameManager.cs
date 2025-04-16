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
    public float PlayerHealth = 20;
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
        RhythmEvents.OnMusicStopped += InactivePauseManager;
        RhythmEvents.OnMusicStart += ActivePauseManager;
        SavedItems = new ItemSO[2];
    }

    private void InactivePauseManager()
    {
        PauseManager.Instance.gameObject.SetActive(false);
    }
    private void ActivePauseManager()
    {
        PauseManager.Instance.gameObject.SetActive(true);
    }

    //추후 GameResultHandler로 분리 예정
    #region GameResult
    public double winScoreThreshold = 10000; // 승리 기준 점수
    private void OnEnable()
    {
        RhythmEvents.OnMusicStopped += OnMusicStopped;

        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정 // 추후 SceneCleanupHandler로 분리 예정
    }

    private void OnDisable()
    {
        RhythmEvents.OnMusicStopped -= OnMusicStopped;
        SceneManager.sceneLoaded -= DestroyOnRestart;
        RhythmEvents.OnMusicStart -= ActivePauseManager;
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
        Debug.Log("게임 종료");
    }

    #endregion
}
