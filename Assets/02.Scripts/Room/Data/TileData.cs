using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public Sprite Sprite;
    public List<string> tags;
}
