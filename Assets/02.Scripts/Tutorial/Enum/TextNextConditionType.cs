using UnityEngine;

public enum TextNextConditionType
{
    [InspectorName("시간 경과 후 자동 전환 or 키 입력 시 전환")]
    OnTimeElapsedOrInput,
    [InspectorName("이벤트 발생 시 전환")]
    OnEvent,

    TextNextConditionType_End
}

public enum TriggerKeyType
{
    NONE,
    W,
    A,
    S,
    D,
    TriggerKeyType_END
}