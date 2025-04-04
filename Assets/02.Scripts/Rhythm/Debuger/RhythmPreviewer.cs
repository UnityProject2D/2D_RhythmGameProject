using System.Collections;
using UnityEngine;

public class RhythmPreviewer : MonoBehaviour
{
    [SerializeField] private RhythmPatternSO pattern;
    [SerializeField] private GameObject noteUIPrefab;
    [SerializeField] private Transform uiSpawnParent;

    private float beatDuration;

    private void OnEnable()
    {
        RhythmEvents.OnNotePreview += ShowNoteUI;
    }
    void OnDisable()
    {
        RhythmEvents.OnNotePreview -= ShowNoteUI;
    }

    void ShowNoteUI(NoteData note)
    {
        var obj = Instantiate(noteUIPrefab, uiSpawnParent);
        obj.GetComponent<NotePreviewUI>().Setup(note.expectedKey);

        Destroy(obj, 4f);
    }
}
