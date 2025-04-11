using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;
using System;

[CreateAssetMenu(fileName = "TutorialStepSo", menuName = "Scriptable Objects/TutorialStepSo")]
public class TutorialStepSo : ScriptableObject
{
    [TextArea] public string Text;
    public float Duration;
    public float DelayTime;

    public TriggerKeyType TriggerKeyType;

    public TextNextConditionType TextNextConditionType;
}
