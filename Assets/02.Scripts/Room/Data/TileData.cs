using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public Sprite Sprite;
    public List<string> tags;
    [Range(1, 100)]
    public int weight = 1; // 기본 가중치
}
