using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//const int CELL_SIZE = 128;
//const float TILE_SCALE_ADJ = 0.9f;
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

    public void Collapse(int selectedIndex)
    {
        if (IsCollapsed)
            return;
        // -1일 경우 랜덤
        if(-1 == selectedIndex)
        {
            int randomIndex = UnityEngine.Random.Range(0, PossibleTiles.Count);
            Collapse(PossibleTiles[randomIndex]);
            return;
        }

        // 후보에 없을 경우
        if (!PossibleTiles.Contains(selectedIndex)) return;

        PossibleTiles = new List<int> { selectedIndex };
        OnCollapsed?.Invoke(this);
    }

    public void Propagate(Vector2Int coords)
    {

    }

}
