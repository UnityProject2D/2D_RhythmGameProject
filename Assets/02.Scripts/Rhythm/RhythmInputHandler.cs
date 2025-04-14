using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class RhythmInputHandler : MonoBehaviour
{
    private @PlayerInputAction inputActions;
    public static RhythmInputHandler Instance { get; private set; }

    public event Action<string> OnInputPerformed; // 입력 이벤트 (리듬 키)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        inputActions = new @PlayerInputAction();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.RhythmAction_W.performed += OnWPressed;
        inputActions.Player.RhythmAction_A.performed += OnAPressed;
        inputActions.Player.RhythmAction_S.performed += OnSPressed;
        inputActions.Player.RhythmAction_D.performed += OnDPressed;

        
        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();

        inputActions.Player.RhythmAction_W.performed -= OnWPressed;
        inputActions.Player.RhythmAction_A.performed -= OnAPressed;
        inputActions.Player.RhythmAction_S.performed -= OnSPressed;
        inputActions.Player.RhythmAction_D.performed -= OnDPressed;
        SceneManager.sceneLoaded -= DestroyOnRestart;
    }
    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameTitle")
        {
            Destroy(gameObject);
        }
    }

    private void OnWPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("W");
    private void OnAPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("A");
    private void OnSPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("S");
    private void OnDPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("D");
    private void OnNPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("N");

    public void SimulateInput(string key)
    {
        if (!Application.isPlaying|| Application.isPlaying) return;
        OnInputPerformed?.Invoke(key);
    }
}
