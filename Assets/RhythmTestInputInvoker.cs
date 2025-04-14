using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;

public class RhythmTestInputInvoker : MonoBehaviour
{
    [SerializeField] private PlayerInputAction inputAction;
    public bool isTest = true;
    private void Awake()
    {
        if (inputAction == null)
            inputAction = new PlayerInputAction();

        inputAction.Player.Enable();

        RhythmEvents.OnNote += InvokeNote;

        DontDestroyOnLoad(gameObject);
    }
    private void InvokeNote(NoteData note)
    {
        RhythmInputHandler.Instance?.SimulateInput(note.expectedKey);
    }
}
