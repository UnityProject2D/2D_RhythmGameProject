using UnityEngine;

public class RhythmRecorderRuntime : MonoBehaviour
{
    public RhythmPatternSO targetPattern;
    public float bpm = 130f;

    private float beatDuration;
    private bool isRecording = false;

    void Start()
    {
        beatDuration = 60f / bpm;
    }

    public void StartRecording()
    {
        isRecording = true;
        Debug.Log("🎙️ 게임 중 녹음 시작!");
    }

    public void StopRecording()
    {
        isRecording = false;
        Debug.Log($"🎧 녹음 완료 - 총 {targetPattern.notes.Count}개 저장됨");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetPattern);
#endif
    }

    void Update()
    {
        if (!isRecording) return;

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
        float time = RhythmManager.Instance.GetCurrentMusicTime();
        float beat = time / beatDuration;

        targetPattern.notes.Add(new NoteData
        {
            beat = beat,
            expectedKey = key
        });

        Debug.Log($"🔴 {key} 입력 기록됨 @ {time:F2}s → beat {beat:F2}");
    }
}
