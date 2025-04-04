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
        float beatDuration = 60f / pattern.bpm;
        float musicStart = RhythmManager.Instance.MusicStartTime;

        foreach (var note in pattern.notes)
        {
            float noteTime = musicStart + (note.beat * beatDuration) - 2f;
            float waitTime = noteTime - Time.time;

            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            SpawnNote(note.expectedKey);
        }
    }

    private void SpawnNote(string key)
    {
        var obj = Instantiate(noteUIPrefab, uiSpawnParent);
        obj.GetComponent<NotePreviewUI>().Setup(key);
        Destroy(obj, 4f);
    }
}
