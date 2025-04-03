using System.Collections;
using UnityEngine;

public class RhythmPreviewer : MonoBehaviour
{
    [SerializeField] private RhythmPatternSO pattern;
    [SerializeField] private GameObject noteUIPrefab;
    [SerializeField] private Transform uiSpawnParent;

    private float beatDuration;

    public void StartPreview()
    {
        StopAllCoroutines();
        beatDuration = 60f / pattern.bpm;
        StartCoroutine(PlayPatternPreview());
    }

    private IEnumerator PlayPatternPreview()
    {
        foreach (var note in pattern.notes)
        {
            float delay = note.beat * beatDuration;
            yield return new WaitForSeconds(delay - Time.timeSinceLevelLoad);

            SpawnNote(note.expectedKey);
        }
    }

    private void SpawnNote(string key)
    {
        var obj = Instantiate(noteUIPrefab, uiSpawnParent);
        obj.GetComponent<NotePreviewUI>().Setup(key);
        Destroy(obj, 2f);
    }
}
