using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryDialogueDataSO", menuName = "Story/StoryDialogueDataSO")]
public class StoryDialogueDataSO : ScriptableObject
{
    public StoryCharacterDataSO character;
    [TextArea] public string text;
    public float delayAfter; // 다이얼로그가 끝난 후 다음 다이얼로그로 전환하는 시간
    public List<StoryEffectDataSO> effects;
}
