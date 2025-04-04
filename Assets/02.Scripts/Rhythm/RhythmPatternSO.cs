using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRhythmPattern", menuName = "Scriptable Objects/RhythmPattern")]
public class RhythmPatternSO : ScriptableObject
{
    public float bpm;
    public List<NoteData> notes = new();
}

[System.Serializable]
public class NoteData
{
    public float beat;
    public string expectedKey;
}