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
    public List<int> PossibleTiles { get; set; }
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

            // 가중치 랜덤 선택

            int totalWeight = PossibleTiles.Sum(t => TileData[t].weight); // 현재 셀이 가질 수 있는 후보 타일의 weight 총합
            int randomValue = UnityEngine.Random.Range(0, totalWeight); // 0 ~ totalWeight 랜덤 값
            int accumulatedWeight = 0; // 가중치 누적값
            int chosen = PossibleTiles[0];
            foreach (var idx in PossibleTiles)
            {
                accumulatedWeight += TileData[idx].weight;
                if (randomValue < accumulatedWeight)
                {
                    chosen = idx;
                    break;
                }
            }
            Collapse(chosen, yPosition, xPosition, TileData);
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