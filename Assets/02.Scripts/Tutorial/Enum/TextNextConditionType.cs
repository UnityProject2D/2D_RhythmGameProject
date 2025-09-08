using UnityEngine;

public enum TextNextConditionType
{
    [InspectorName("시간 경과 후 자동 전환 or 키 입력 시 전환")]
    OnTimeElapsedOrInput,
    [InspectorName("이벤트 발생 시 전환")]
    OnEvent,
    [InspectorName("버튼 클릭 시 전환")]
    OnButtonClick,
    [InspectorName("키 입력 시 전환")]
    OnInput
}

public enum TriggerKeyType
{
    NONE,
    Next,
    Prev,
}

public enum MusicState
{
    NonPlay,
    Play,
    Stop,
}

public enum NextSquence
{
    Basic,
    Loop,
}