using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Cell
{
    public Vector2 Coords {  get; private set; }
    public List<int> PossibleTiles { get; private set; }
    public bool IsCollapsed => PossibleTiles.Count == 1;

    public event Action<Cell> OnCollapsed; // collapse 알림
    public Cell(Vector2 coords, int totalTileCount)
    {
        Coords = coords;
        PossibleTiles = Enumerable.Range(0, totalTileCount).ToList();
    }

    public void Constrain(int tileIndex)
    {
        PossibleTiles.Remove(tileIndex);
        if (IsCollapsed)
            OnCollapsed?.Invoke(this);
    }


}
