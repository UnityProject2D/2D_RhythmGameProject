using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

//const int CELL_SIZE = 128;
//const float TILE_SCALE_ADJ = 0.9f;
public class Cell
{
    public int GridSize;
    public Vector2Int Coords {  get; private set; }
    public List<int> PossibleTiles { get; private set; }
    public bool IsCollapsed => PossibleTiles.Count == 1;

    public event Action<Cell> OnCollapsed; // collapse 알림
    public Cell(Vector2Int coords, int totalTileCount)
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

    public bool Collapse(int selectedIndex, int yPosition, int xPosition, List<TileData> TileData)
    {
        if (IsCollapsed)
            return true;

        // -1일 경우 랜덤 적용
        if (-1 == selectedIndex)
        {
            if (PossibleTiles.Count == 0)
            {
                return false;
            }

            int randomIndex = UnityEngine.Random.Range(0, PossibleTiles.Count);
            Collapse(PossibleTiles[randomIndex], yPosition, xPosition, TileData);
            return true;
        }

        // 후보에 없을 경우
        if (!PossibleTiles.Contains(selectedIndex))
        {
            return false;
        }
            

        PossibleTiles = new List<int> { selectedIndex };
        OnCollapsed?.Invoke(this);

        return true;
    }
}



/*
 

public void Collapse(int selectedIndex = -1, int yPosition = -1)
{
    if (IsCollapsed)
        return;

    // Collapse 대상 필터링
    if (selectedIndex == -1)
    {
        var validTiles = PossibleTiles.Where(id =>
        {
            var tile = tileDataDict[id];
            if (!tile.UseYConstraint) return true;
            return yPosition >= tile.MinY && yPosition <= tile.MaxY;
        }).ToList();

        if (validTiles.Count == 0)
        {
            Debug.LogWarning($"[Collapse] 유효한 후보가 없습니다. y: {yPosition}");
            return; // 또는 리셋 처리
        }

        int randomIndex = UnityEngine.Random.Range(0, validTiles.Count);
        Collapse(validTiles[randomIndex], yPosition);
        return;
    }

    // 필터 통과한 인덱스만 허용
    if (!PossibleTiles.Contains(selectedIndex)) return;

    PossibleTiles = new List<int> { selectedIndex };
    OnCollapsed?.Invoke(this);
}



 */