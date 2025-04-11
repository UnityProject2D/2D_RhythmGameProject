using UnityEngine;

public enum CharacterType
{
    Hero, // 주인공
    AI, // AI
    Director, // 연구소 소장
    System // 시스템 메시지
}

[CreateAssetMenu(fileName = "StoryCharacterDataSO", menuName = "Story/StoryCharacterDataSO")]
public class StoryCharacterDataSO : ScriptableObject
{
    public CharacterType type;
    public string displayName; // UI에 표시될 이름
    public Color nameColor; // 이름 색상
    public Sprite portrait; // 초상화 스프라이트
}
