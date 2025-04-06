using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    const int GRID_SIZE = 6;
    const int GRID_SCALE = 128;

    const int TILE_SIZE = 8;
    enum DIRECT
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM,
        DIRECT_END
    }

    Vector2[] Dirs =
    {
        Vector2.left,
        Vector2.right,
        Vector2.up,
        Vector2.down
    };

    public List<List<Cell>> cellDatas;

    public void Init()
    {
        for(int i = 0; i < GRID_SIZE; i++)
        {
            List<Cell> cells = new List<Cell>();
            for (int j = 0; j < GRID_SIZE; j++)
            {
                cells.Add(new Cell(new Vector2(i, j), TILE_SIZE));
            }
            cellDatas.Add(cells);
        }
    }

    public bool Is_fully_collapsed()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                if (!cellDatas[i][j].IsCollapsed)
                    return false;
            }
        }
        return true;
    }

    public Vector2 get_lowest_entropy_coords()
    {
        Vector2 lowest_coords = Vector2.zero;
        int lowest_entropy = int.MaxValue;
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                int entropy = cellDatas[i][j].PossibleTiles.Count();
                if (entropy <= 1)
                    continue;
                else if (entropy < lowest_entropy)
                {
                    lowest_entropy = entropy;
                    lowest_coords = new Vector2(i, j);
                }
            }
        }
        return lowest_coords;
    }

    public void collapse_at_coords(Vector2Int coords)
    {
        cellDatas[coords.x][coords.y].Collapse(-1);
    }
}
