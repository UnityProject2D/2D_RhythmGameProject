using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TutorialStepSo", menuName = "Scriptable Objects/TutorialStepSo")]
public class TutorialStepSo : ScriptableObject
{
    [TextArea] public string Text;
    public UnityEvent OnStartEvent;
    public UnityEvent OnEndEvent;
}
