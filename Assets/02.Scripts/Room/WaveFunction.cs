using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class WaveFunction : MonoBehaviour
{
    public const int GRID_SIZE = 6;
    public const int GRID_SCALE = 128;

    // 타일 종류
    public const int TILE_TYPE = 8;
    public const int TILE_SIZE = 8;

    //public Dictionary<int, Dictionary<DIRECT, HashSet<int>>> Constraints;
    //// 1. 타일 인덱스, 2. 방향에 따라 적용할 수 있는 타일 셋(방향, 타일 번호)

    public List<TileData> TileData;

    public MapTilePainter MapTilePainter;

    private Stack<Vector2Int> propagateStack = new Stack<Vector2Int>();
    public enum DIRECT
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM,
        DIRECT_END
    }

    Vector2Int[] Dirs =
    {
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up,
        Vector2Int.down
    };

    public List<List<Cell>> cellDatas = new List<List<Cell>>();


    // 초기화 - 셀 정보 설정
    public void Init()
    {
        cellDatas.Clear();
        for (int i = 0; i < GRID_SIZE; i++)
        {
            List<Cell> cells = new List<Cell>();
            for (int j = 0; j < GRID_SIZE; j++)
            {
                cells.Add(new Cell(new Vector2(i, j), TILE_SIZE));
            }
            cellDatas.Add(cells);
        }
    }

    // 모든 타일이 결정(collapse) 되었는지 반환
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

    // 가장 엔트로피가 낮은(배치 가능한 타일 종류가 적은 인덱스 반환)
    public Vector2Int Get_lowest_entropy_coords()
    {
        Vector2Int lowest_coords = Vector2Int.zero;
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
                    lowest_coords = new Vector2Int(i, j);
                }
            }
        }
        return lowest_coords;
    }

    // 특정 인덱스 셀 확정(Collapse) - -1 설정(랜덤 값 지정)
    public void Collapse_at_coords(Vector2Int coords)
    {
        cellDatas[coords.x][coords.y].Collapse(-1);
        Debug.Log($"현재 인덱스: x: {coords.x} y: {coords.y}");
    }

    // 전파 함수
    public void Propagate(Vector2Int coords)
    {
        // 처음 전파할 값
        propagateStack.Append(coords);

        while (propagateStack.Count > 0){
            // 현재 셀 기준 값
            Vector2Int cur_coord = propagateStack.Pop();
            Cell cur_cell = cellDatas[cur_coord.x][cur_coord.y];
            List<int> cur_tiles = cur_cell.PossibleTiles;

            // 상하좌우 방향이 다른 셀
            for(int i = 0; i < (int)DIRECT.DIRECT_END; i++)
            {
                // 4가지 방향
                Vector2Int other_coords = cur_coord + Dirs[i];

                // 해당 인덱스가 유효한지 체크
                if (!Is_valid_direction(other_coords)) continue;

                Cell other_cell = cellDatas[other_coords.x][other_coords.y];
                List<int> other_tiles = other_cell.PossibleTiles;

                // 해당 방향 가능한 이웃 타일 리스트 반환
                List<int> possible_neighbours = Get_all_possible_neighbours(DIRECT.LEFT + i, cur_tiles);

                // 특정 방향 타일 리스트
                for(int j = 0; j < other_tiles.Count; j++)
                {
                    // 가능한 이웃 타일 목록에 해당 인덱스가 없으면 제거
                    if (!possible_neighbours.Contains(other_tiles[i]))
                    {
                        other_tiles.RemoveAt(i--); // 해당 인덱스 삭제 후 인덱스 조절(--)
                    }
                }
            }

        }
    }


    // 해당 셀이 유효한지
    public bool Is_valid_direction(Vector2Int coords)
    {
        return (coords.x >= GRID_SIZE || coords.x < 0) ||
            (coords.y >= GRID_SIZE || coords.y < 0);
    }

    // 특정 방향으로 유효한 타일 리스트 반환
    public List<int> Get_all_possible_neighbours(DIRECT dir, List<int> tiles)
    {
        List<int> possibilities = new List<int>();

        // 현재 들어올 수 있는 타일들을 기준으로 이웃 추가
        foreach (int index in tiles) {
            possibilities.AddRange(TileData[index].constraints[(int)dir].AllowNeighbours);
        }

        return possibilities;
    }

    public void Solve()
    {
        Init();
        // 모두 Collapse될 때까지 실행
        while (!Is_fully_collapsed()){
            Iterate();
        }

        MapTilePainter.SettingTileMap(TileData, cellDatas);
    }

    public void Iterate()
    {
        Vector2Int coords = Get_lowest_entropy_coords();
        Collapse_at_coords(coords);
        Propagate(coords);
    }
}
