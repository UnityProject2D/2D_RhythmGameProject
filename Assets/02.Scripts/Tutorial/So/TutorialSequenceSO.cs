using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSequenceSO", menuName = "Scriptable Objects/TutorialSequenceSO")]
public class TutorialSequenceSO : ScriptableObject
{
    public List<TutorialStepSo> Steps;

    public List<TutorialStepSo> Loops;
}
