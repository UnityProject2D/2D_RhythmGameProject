using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "StorySceneDataSO", menuName = "Story/StorySceneDataSO")]
public class StorySceneDataSO : ScriptableObject
{
    public Sprite background;
    public EventReference? bgm;
    public List<StoryDialogueDataSO> dialogues;
}
