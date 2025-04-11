using System;
using UnityEngine;

// TextNextConditionType에 따라서 이벤트 실행
public class TutorialEventSystem : MonoBehaviour
{
    public static Action<string> OnTutorialTextEvent;
    
    public static void OnTextEvents(string eventName = ""){
        OnTutorialTextEvent?.Invoke(eventName);
    }
}
