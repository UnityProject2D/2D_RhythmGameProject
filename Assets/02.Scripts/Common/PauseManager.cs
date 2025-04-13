using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public GameObject pauseUI;


    private bool isAppFocused = true;
    private bool isPaused = false;
    private Bus masterBus;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        masterBus = RuntimeManager.GetBus("bus:/BGM");
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {

        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정 // 추후 SceneCleanupHandler로 분리 예정
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= DestroyOnRestart;
    }

    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameTitle" || scene.name == "UI_GameOver")
        {
            Destroy(gameObject);
        }
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        isAppFocused = hasFocus;

        if (!hasFocus && !isPaused)
        {
            PauseGame();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
        masterBus.setPaused(true);

        GameManager.Instance.Player.Controller.SetInputEnabled(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        masterBus.setPaused(false);


        GameManager.Instance.Player.Controller.SetInputEnabled(true);
    }
}
