using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RhythmPatternWindow : EditorWindow
{
    private RhythmPatternSO pattern;
    private Vector2 scrollPos;
    private float bpm;
    private float beatDuration;
    private int totalBeats = 240; // 임시: 4마디

    public static void Open(RhythmPatternSO pattern)
    {
        var window = GetWindow<RhythmPatternWindow>("Rhythm Pattern Editor");
        window.pattern = pattern;
        window.bpm = pattern.bpm;
        window.beatDuration = 60f / pattern.bpm;
    }

    void OnGUI()
    {
        if (pattern == null) return;

        GUILayout.Label($"🎼 BPM: {bpm} | 1박자 = {beatDuration:F2}초", EditorStyles.boldLabel);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < totalBeats; i++)
        {
            DrawBeatLine(i);
        }

        GUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("Clear Notes"))
        {
            if (EditorUtility.DisplayDialog("삭제 확인", "모든 노트를 지울까요?", "네", "아니오"))
            {
                Undo.RecordObject(pattern, "Clear Notes");
                pattern.notes.Clear();
                EditorUtility.SetDirty(pattern);
            }
        }
    }

    void DrawBeatLine(int beatIndex)
    {
        GUILayout.BeginHorizontal("box");

        GUILayout.Label($"Beat {beatIndex + 1}", GUILayout.Width(70));

        if (GUILayout.Button("Add Note", GUILayout.Width(100)))
        {
            var newNote = new NoteData { beat = beatIndex + 1f, expectedKey = "W" };
            Undo.RecordObject(pattern, "Add Note");
            pattern.notes.Add(newNote);
            EditorUtility.SetDirty(pattern);
        }

        foreach (var note in pattern.notes.FindAll(n => Mathf.Approximately(n.beat, beatIndex + 1f)))
        {
            note.expectedKey = EditorGUILayout.TextField(note.expectedKey, GUILayout.Width(50));
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                Undo.RecordObject(pattern, "Remove Note");
                pattern.notes.Remove(note);
                EditorUtility.SetDirty(pattern);
                break;
            }
        }

        GUILayout.EndHorizontal();
    }
}
