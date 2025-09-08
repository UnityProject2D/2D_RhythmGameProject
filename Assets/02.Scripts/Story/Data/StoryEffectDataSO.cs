using UnityEngine;

public enum StoryEffectType
{
    Typing, // 타이핑
    FadeOut, // 페이드 아웃
    InsertImage, // 이미지 삽입
    ChangeBackground, // 배경 변경
}

[CreateAssetMenu(fileName = "StoryEffectDataSO", menuName = "Story/StoryEffectDataSO")]
public class StoryEffectDataSO : ScriptableObject
{
    public StoryEffectType type;
    public float duration; // 효과 지속시간(타이핑인 경우 간격)
    public Sprite image; // 삽입/교체할 이미지
}
