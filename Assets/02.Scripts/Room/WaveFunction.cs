using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int X_GRID_SIZE;
    public int Y_GRID_SIZE;
    public const int GRID_SCALE = 128;

    public List<TileData> TileData;
    public MapTilePainter MapTilePainter;

    public TileRuleSetData ruleSet;

    private Stack<Vector2Int> propagateStack = new();


    /// <summary>
    /// trail: 전파 중 후보가 제거될 때마다 push - 데이터(coords, removedTile)
    /// choices: 셀 확정할 때 남은 후보들을 담아두는 분기점

    private Stack<(Vector2Int cell, int removedTile)> trail = new();
    private Stack<(Vector2Int cell, List<int> remaining)> choices = new();

    /// </summary>
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

    private void Start()
    {
        Init();
        // StartSolve();
    }

    // 초기화 - 셀 기본 정보 설정
    public void Init()
    {
        cellDatas.Clear();
        for (int x = 0; x < X_GRID_SIZE; x++)
        {
            List<Cell> cells = new List<Cell>();
            for (int y = 0; y < Y_GRID_SIZE; y++)
            {
                Cell cellComponent = new Cell(new Vector2Int(x, y), TileData.Count);
                int cx = x;
                int cy = y;
                cellComponent.OnCollapsed += (Cell) => On_cell_collapsed(new Vector2Int(cx, cy));
                cells.Add(cellComponent);
            }
            cellDatas.Add(cells);
        }
    }

    // 모든 타일이 결정되었는지 여부
    public bool Is_fully_collapsed()
    {
        for (int x = 0; x < X_GRID_SIZE; x++)
        {
            for (int y = 0; y < Y_GRID_SIZE; y++)
            {
                if (!cellDatas[x][y].IsCollapsed)
                    return false;
            }
        }
        return true;
    }

    // 배치 가능한 타일 종류가 가장 적은 인덱스 반환
    public Vector2Int GetLowestValueCoords()
    {
        int lowValue = int.MaxValue;
        List<Vector2Int> cands = null;

        for (int x = 0; x < X_GRID_SIZE; x++)
        {
            for (int y = 0; y < Y_GRID_SIZE; y++)
            {
                int entropy = cellDatas[x][y].PossibleTiles.Count();
                if (entropy <= 1) continue;

                if (entropy < lowValue)
                {
                    lowValue = entropy;
                    cands = new List<Vector2Int> { new Vector2Int(x, y) };
                }
                else if (entropy == lowValue)
                {
                    cands.Add(new Vector2Int(x, y));
                }
            }
        }
        if (cands == null || cands.Count == 0) return Vector2Int.zero;
        return cands[Random.Range(0, cands.Count)];
    }

    // 인덱스 셀 확정
    // -1일 경우 랜덤 값 지정
    public void CollapseCoords(Vector2Int coords)
    {
        cellDatas[coords.x][coords.y].Collapse(-1, coords.y, coords.x, TileData);

        Debug.Log($"현재 인덱스: x: {coords.x} y: {coords.y}");
    }

    public void On_cell_collapsed(Vector2Int coords)
    {
        Propagate(coords);
    }

    // 전파 함수
    public bool Propagate(Vector2Int coords)
    {
        // 처음 전파할 값
        propagateStack.Push(coords);

        while (propagateStack.Count > 0){
            
            // 현재 기준 값
            Vector2Int cur_coord = propagateStack.Pop();
            Cell cur_cell = cellDatas[cur_coord.x][cur_coord.y];
            List<int> cur_tiles = cur_cell.PossibleTiles;

            // 상하좌우 셀 체크
            for(int i = 0; i < (int)DIRECT.DIRECT_END; i++)
            {
                // 4가지 방향
                Vector2Int other_coords = cur_coord + Dirs[i];

                // 유효성 체크
                if (!Is_valid_direction(other_coords))
                {
                    continue;
                }

                Cell other_cell = cellDatas[other_coords.x][other_coords.y];
                List<int> other_tiles = other_cell.PossibleTiles;

                // 해당 방향으로 가능한 이웃들 반환
                List<int> possible_neighbours = Get_all_possible_neighbours(DIRECT.LEFT + i, cur_tiles);

                // 역방향 인덱스
                int revIndex = i ^ 1;

                // 특정 방향 타일 리스트
                bool bChange = false;
                for (int j = other_tiles.Count - 1; j >= 0; j--)
                {
                    int tileId = other_tiles[j];
                    TileData tileData = TileData[tileId];

                    // 역방향 체크
                    // B의 역방향이 A 후보들 중 하나라도 허용하는지 (B->A)
                    bool passBackward = false;
                    for (int k = 0; k < cur_tiles.Count; k++)
                    {
                        var selfA = TileData[cur_tiles[k]];// A 후보
                        var neighborB = TileData[other_tiles[j]];// B 후보
                        if (Compatible(neighborB, (WaveFunction.DIRECT)revIndex, selfA, ruleSet))
                        {
                            passBackward = true;
                            break;
                        }
                    }

                    // 가능한 이웃 타일 목록에 해당 인덱스가 없으면 제거(정방향: A->B)
                    bool passForward = possible_neighbours.Contains(other_tiles[j]);

                    // 정방향, 역방향 모두 만족하지 않으면 제거
                    if (!(passForward && passBackward))
                    {
                        int removed = other_tiles[j];
                        other_tiles.RemoveAt(j);// 해당 인덱스 삭제
                        
                        // 삭제 트레일 추가
                        trail.Push((other_coords, removed));
                        bChange = true;
                        continue;
                    }
                }

                // 없으면 false 반환
                if (other_tiles.Count == 0)
                {
                    return false;
                }

                if (bChange && !propagateStack.Contains(other_coords))
                    propagateStack.Push(other_coords);
            }
        }
        return true;
    }

    public bool TryAssign(Vector2Int cell, int tileId)
    {
        // trail 길이 저장
        int mark = trail.Count;

        // 해당 셀 도메인을 tileId 하나로 축소하면서 삭제 기록
        List<int> values = cellDatas[cell.x][cell.y].PossibleTiles;
        for (int i = values.Count - 1; i >= 0; --i)
        {
            if (values[i] != tileId)
            {
                int removed = values[i];
                values.RemoveAt(i);
                trail.Push((cell, removed));
            }
        }

        // 전파 실패시 false
        if (!Propagate(cell))
        {
            // trail을 mark까지 undo
            while(trail.Count > mark)
            {
                var trailValue = trail.Pop();
                List<int> list = cellDatas[cell.x][cell.y].PossibleTiles;

                // 다시 가능한 타일로 추가
                if (!list.Contains(trailValue.removedTile))
                {
                    list.Add(trailValue.removedTile);
                }
            }
            return false;
        }
        return true;
    }

    private bool StepOnce()
    {
        Vector2Int cell = GetLowestValueCoords();
        if (cell == Vector2Int.zero)
        {
            return true;
        }

        List<int> cellValue = cellDatas[cell.x][cell.y].PossibleTiles.ToList();

        choices.Push((cell, cellValue.ToList()));

        // 첫 후보부터 시도
        int last = cellValue[cellValue.Count - 1];
        cellValue.RemoveAt(cellValue.Count - 1);
        choices.Pop();
        choices.Push((cell, cellValue));

        if (TryAssign(cell, last))
        {
            return true;
        }
        return Backtracking();
        // return Backtracking();
    }

    private bool Backtracking()
    {
        while (choices.Count > 0)
        {
            var (cell, remainging) = choices.Pop();
            if (remainging.Count == 0)
            {
                continue;
            }

            int next = remainging[remainging.Count - 1];
            remainging.RemoveAt(remainging.Count - 1);
            choices.Push((cell, remainging));

            if (TryAssign(cell, next))
            {
                return true;
            }
        }
        return false;
    }

    // 셀 유효성 체크
    public bool Is_valid_direction(Vector2Int coords)
    {
        return !((coords.x >= X_GRID_SIZE || coords.x < 0) || (coords.y >= Y_GRID_SIZE || coords.y < 0));
    }

    // 태그 유효성 체크
    bool HasTag(TileData tileData, string tag) => string.IsNullOrEmpty(tag) || (tileData && tileData.tags != null && tileData.tags.Contains(tag));

    public bool Compatible(TileData self, WaveFunction.DIRECT dir, TileData neighbor, TileRuleSetData ruleSetData)
    {
        // 우선 순위 높은 규칙부터 체크
        foreach (var rule in ruleSetData.rules)
        {
            if (rule.dir != dir) continue;
            if (!HasTag(self, rule.selfTag)) continue;
            if (!HasTag(neighbor, rule.neighborTag)) continue;
            return rule.allow; // 규칙 매칭 적용
        }
        return ruleSetData.defaultAllow; // 매칭 규칙 없음
    }

    // 특정 방향으로 유효한 타일 리스트 반환
    public List<int> Get_all_possible_neighbours(DIRECT dir, List<int> tiles)
    {
        var possibilities = new List<int>();
        // 후보와 모든 타일을 모두 대조 후, 하나라도 허용하면 후보로 추가
        for (int i = 0; i < TileData.Count; i++)
        {
            TileData tileData = TileData[i];
            bool bPossibleTile = false;
            for (int t = 0; t < tiles.Count; t++)
            {
                TileData self = TileData[tiles[t]];
                if (Compatible(self, dir, tileData, ruleSet))
                { 
                    bPossibleTile = true; 
                    break;
                }
            }
            if (bPossibleTile) 
                possibilities.Add(i);
        }
        return possibilities;
    }

    public void Iterate()
    {
        Vector2Int coords = GetLowestValueCoords();
        CollapseCoords(coords);
        Propagate(coords);
    }

    public void RenderIndex()
    {
        for (int i = 0; i < X_GRID_SIZE; i++)
        {
            for (int j = 0; j < Y_GRID_SIZE; j++)
            {
                if (cellDatas[i][j].IsCollapsed && cellDatas[i][j].PossibleTiles.Count > 0)
                    Debug.Log(cellDatas[i][j].PossibleTiles[0] + " ");
                else
                    Debug.Log("X");
            }
        }
    }

    IEnumerator CoSolve()
    {
        Init(); // 시도마다 다시 리셋
        LimitTagToBand("SURFACE", 0, 1);
        LimitTagToBand("OBJECT_CEIL", Y_GRID_SIZE - 2, Y_GRID_SIZE - 1);

        int playN = 0, maxPlay = 20000;
        while (!Is_fully_collapsed() && playN++ < maxPlay)
        {
            if (!StepOnce())
            {
                Debug.LogError("error!-step");
                break;
            }
            // 중간 렌더
            MapTilePainter.SettingTileMap(TileData, cellDatas);
            yield return null;
        }

        MapTilePainter.SettingTileMap(TileData, cellDatas);

    }

    public void StartSolve()
    {
        StartCoroutine(CoSolve());
    }

    bool HasContradiction()
    {
        for (int x = 0; x < cellDatas.Count; x++)
        {
            for (int y = 0; y < cellDatas[x].Count; y++)
            {
                Cell c = cellDatas[x][y];
                if (c == null || c.PossibleTiles == null)
                {
                    Debug.LogError($"Cell: ({x},{y}): NULL");
                    return true;
                }
                if (c.PossibleTiles.Count == 0)
                {
                    Debug.LogError($"Contradiction: Cell ({x},{y})");
                    return true;
                }
            }
        }
        return false;
    }



// 태그로 타일 후보 모으기
List<int> GetTilesWithTag(string tag)
{
    var list = new List<int>();
    if (string.IsNullOrEmpty(tag)) return list;
    for (int i = 0; i < TileData.Count; i++)
    {
            var tags = TileData[i]?.tags;
            if (tags != null && tags.Contains(tag))
            {
                list.Add(i);
            }
        }
    return list;
}

    public void LimitTagToBand(string tag, int yMin, int yMax)
    {
        int W = X_GRID_SIZE, H = Y_GRID_SIZE;

        yMin = Mathf.Clamp(yMin, 0, H - 1);
        yMax = Mathf.Clamp(yMax, 0, H - 1);
        if (yMin > yMax) return;

        for (int y = 0; y < H; y++)
        {
            bool inside = (y >= yMin && y <= yMax);
            if (inside) continue; // 밴드 내부 값은 허용

            for (int x = 0; x < W; x++)
            {
                var c = cellDatas[x][y];
                if (c.IsCollapsed)
                {
                    continue;
                }
                int removed = c.PossibleTiles.RemoveAll(id =>
                    TileData[id]?.tags != null && TileData[id].tags.Contains(tag));
                if (removed > 0)
                {
                    Propagate(new Vector2Int(x, y));
                }
            }
        }
    }
}