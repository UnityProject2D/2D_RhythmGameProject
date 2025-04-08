using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RhythmPreviewer : MonoBehaviour
{
    [SerializeField] private RhythmPatternSO pattern;
    [SerializeField] private GameObject noteUIPrefab;
    [SerializeField] private Transform uiSpawnParent;

    public TMP_Dropdown dropdown;

    private float beatDuration;

    private void OnEnable()
    {
        RhythmEvents.OnNotePreview += ShowNoteUI;
    }
    void OnDisable()
    {
        RhythmEvents.OnNotePreview -= ShowNoteUI;
    }

    public void OnClickStartButton()
    {
        RhythmManager.Instance.OnLoadedStage(dropdown.value);
    }

    void ShowNoteUI(NoteData note)
    {
        var obj = Instantiate(noteUIPrefab, uiSpawnParent);
        obj.GetComponent<NotePreviewUI>().Setup(note.expectedKey);

        Destroy(obj, 4f);
    }
}
