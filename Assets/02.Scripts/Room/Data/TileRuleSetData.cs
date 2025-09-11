using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TileRuleSetData", menuName = "Scriptable Objects/TileRuleSetData")]
public class TileRuleSetData : ScriptableObject
{
    [Serializable]
    public class Rule
    {
        public bool allow = true;           // 허용 여부(이웃)
        public WaveFunction.DIRECT dir;     // 이웃 방향
        public string selfTag;              // 현재 타일 태그(없을 경우: 모든 타일)
        public string neighborTag;          // 이웃 태그(없을 경우: 모든 타일)
        public int priority = 0;            // 우선 순위 적용(높을 수록 먼저) 
    }
    public bool defaultAllow = true;        // 기본 허용(규칙에 걸리지 않으면): false -> 기본 금지
    public List<Rule> rules;
}