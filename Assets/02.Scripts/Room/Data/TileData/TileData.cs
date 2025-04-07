using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public int Id;
    public Sprite Sprite;

    [Serializable]
    public class Constraints
    {
        public WaveFunction.DIRECT Direction; // 방향
        public List<int> AllowNeighbours; // 허용된 이웃 타일 리스트
    }

    public List<Constraints> constraints = new List<Constraints>();

    public bool UseYConstraint = false;
    public int MinY = 0;
    public int MaxY = 10;

    public bool UseXConstraint = false;
    public int MinX = 0;
    public int MaxX = 10;
}
