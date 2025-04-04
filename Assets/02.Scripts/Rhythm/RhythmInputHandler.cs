using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class RhythmInputHandler : MonoBehaviour
{
    private @PlayerInputAction inputActions;
    public static RhythmInputHandler Instance;

    public event Action<string> OnInputPerformed; // 입력 이벤트 (리듬 키)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        inputActions = new @PlayerInputAction();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.RhythmAction_W.performed += OnWPressed;
        inputActions.Player.RhythmAction_A.performed += OnAPressed;
        inputActions.Player.RhythmAction_S.performed += OnSPressed;
        inputActions.Player.RhythmAction_D.performed += OnDPressed;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();

        inputActions.Player.RhythmAction_W.performed -= OnWPressed;
        inputActions.Player.RhythmAction_A.performed -= OnAPressed;
        inputActions.Player.RhythmAction_S.performed -= OnSPressed;
        inputActions.Player.RhythmAction_D.performed -= OnDPressed;
    }

    private void OnWPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("W");
    private void OnAPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("A");
    private void OnSPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("S");
    private void OnDPressed(InputAction.CallbackContext ctx) => OnInputPerformed?.Invoke("D");
}
