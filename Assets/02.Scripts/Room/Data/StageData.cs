using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public string StageName;

    public int StageIndex;
}
