using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RhythmPatternSO))]
public class RhythmPatternEditor : Editor
{
    private RhythmPatternSO pattern;
    private float beatDuration;

    private void OnEnable()
    {
        pattern = (RhythmPatternSO)target;
        beatDuration = 60f / pattern.bpm;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        GUILayout.Label("🎵 리듬 에디터", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Beat Editor"))
        {
            RhythmPatternWindow.Open(pattern);
        }
    }
}
