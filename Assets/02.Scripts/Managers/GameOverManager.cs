using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }
    public int WaitTime = 3; // 대기 시간 (초)
    public int WaitTime2 = 1; // 대기 시간 (초)

    public float startPitch = 1f;
    public float endPitch = 0.2f;
    [SerializeField] private string gameOverSceneName = "UI_GameOver";
    private Bus masterBus;

    private void Awake()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        if (GameManager.Instance.Player.Health != null)
        {
            OnPlayerRegistered();
        }
        else
        {
            GameManager.Instance.PlayerRegistered += OnPlayerRegistered;
        }
    }
    private void OnPlayerRegistered()
    {
        GameManager.Instance.Player.Health.OnPlayerDied += OnPlayerDied;
    }
    public void OnPlayerDied()
    {
        Debug.Log("[GameOverManager] 게임 오버 처리");
        StartCoroutine(HandleDie());
    }
    private void OnDestroy()
    {

        if (GameManager.Instance.Player.Health != null)
        {
            GameManager.Instance.Player.Health.OnPlayerDied -= OnPlayerDied;
        }
        else
        {
            GameManager.Instance.PlayerRegistered -= OnPlayerRegistered;
        }
    }
    private IEnumerator HandleDie()
    {
        Debug.Log("[GameOverManager] HandleDie() 호출");
        Time.timeScale = 0.2f;

        yield return new WaitForSecondsRealtime(WaitTime);
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(WaitTime2);
        SceneManager.LoadScene(gameOverSceneName);
    }

    
}
