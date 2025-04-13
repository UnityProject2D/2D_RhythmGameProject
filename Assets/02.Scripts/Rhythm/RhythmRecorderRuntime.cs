using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RhythmRecorderRuntime : MonoBehaviour
{
    public RhythmPatternSO targetPattern;
    private bool isRecording = false;
    public TMP_Dropdown dropdown;
    public float bpm = 100f;

    private float beatDuration;
    public void StartRecording()
    {
        beatDuration = 60f / bpm;
        var tmp = new StageData();
        tmp.StageIndex =dropdown.value;

        RhythmManager.Instance.OnLoadedStage(tmp);
        StartCoroutine(WaitUntil());
    }

    public IEnumerator WaitUntil()
    {
        while (!RhythmManager.Instance.IsPlaying)
        {
            yield return null;
        }

        isRecording = true;
        Debug.Log("게임 중 녹음 시작!");
    }

    public void StopRecording()
    {
        isRecording = false;
        Debug.Log($"녹음 완료 - 총 {targetPattern.notes.Count}개 저장됨");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetPattern);
#endif
    }

    void Update()
    {
        if (!isRecording) return;
        beatDuration = 60f / RhythmManager.Instance.BPM;
        if (Input.anyKeyDown)
        {
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    RecordKey(k.ToString());
                    break;
                }
            }
        }
    }

    void RecordKey(string key)
    {
        if (key != "W" && key != "A" && key != "S" && key != "D") return;
        float time = RhythmManager.Instance.GetCurrentMusicTime();
        float beat = time / beatDuration;

        targetPattern.notes.Add(new NoteData
        {
            beat = beat,
            expectedKey = key
        });

        Debug.Log($"{key} 입력 기록됨 @ {time:F2}s → beat {beat:F2}");
    }
}
