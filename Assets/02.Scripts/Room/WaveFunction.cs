using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using WFC.Domain.Core;
using WFC.Domain.Policy;
using WFC.Domain.Rules;
using CellMask = WFC.Domain.Core.CellDomainMask;

public class WaveFunction : MonoBehaviour
{
    public int X_GRID_SIZE;
    public int Y_GRID_SIZE;
    public const int GRID_SCALE = 128;

    public List<TileData> TileData;
    public MapTilePainter MapTilePainter;

    public TileRuleSetData ruleSet;

    private Stack<Vector2Int> propagateStack = new();
    private CellMask[,] _forwardLUT;
    private CellMask[,] _backwardLUT;
    /// <summary>
    /// trail: 전파 중 후보가 제거될 때마다 push - 데이터(coords, removedTile)
    /// choices: 셀 확정할 때 남은 후보들을 담아두는 분기점

    private Stack<(Vector2Int cell, int removedTile)> trail = new();
    private Stack<(Vector2Int cell, List<int> remaining)> choices = new();

    private Dictionary<string, CellMask> _tagMasks = new();
    private int _hintY;
    private bool _hintLeftHigh;

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
        SettingLUT();
        // StartSolve();
    }

    private CellMask GetTagMask(string tag)
    {
        return _tagMasks.TryGetValue(tag, out var m) ? m : default;
    }

    // 왼쪽 경계 + SURFACE + LEFT(HIGH/LOW) 체크
    // 도메인으로 점검

    private void ApplyLeftEdgeSurface(int y, bool leftHigh)
    {
        EdgePolicies.EdgePolicyContext ctx = new EdgePolicies.EdgePolicyContext
        {
            GridHeight = Y_GRID_SIZE,
            TileCount = TileData.Count,
            GetCandidates = (x, y) => cellDatas[x][y].PossibleTiles,
            SetCandidates = (x, y, list) =>
                cellDatas[x][y].PossibleTiles = list,
            GetTagMask = tag => GetTagMask(tag),
            Propagate = (x, y) =>
                Propagate(new UnityEngine.Vector2Int(x, y))
        };

        EdgePolicies.ApplyLeftEdgeSurface(ctx, y, leftHigh);
    }

    public (int y, bool rightHigh) GetRightSurfaceInfo()
    {
        int x = X_GRID_SIZE - 1;
        for (int y = 0; y < Y_GRID_SIZE; y++)
        {
            var cell = cellDatas[x][y];
            if (!cell.IsCollapsed || cell.PossibleTiles.Count == 0) continue;

            int id = cell.PossibleTiles[0];
            var tags = TileData[id]?.tags;
            if (tags != null && tags.Contains("SURFACE"))
                return (y, tags.Contains("RIGHT=HIGH"));
        }
        return (-1, false);
    }

    public void SettingLUT()
    {
        int tileCount = TileData.Count;

        // 도메인 룰셋으로 변환
        RuleSetModel domainRuleSet = RuleSetMapper.ToDomain(ruleSet);

        // 도메인 규칙 어댑터
        TileRuleCompatibilityAdapter compatiblilty = new TileRuleCompatibilityAdapter(TileData, domainRuleSet);

        // 도메인 빌더로 정/역 방향 LUT 생성
        PropagationLUT lut = PropagationLUTBuilder.Build(tileCount, compatiblilty);

        // 기존 필드에 그대로 복사
        _forwardLUT = new CellMask[(int)DIRECT.DIRECT_END, tileCount];
        _backwardLUT = new CellMask[(int)DIRECT.DIRECT_END, tileCount];

        for (int dir = 0; dir < (int)DIRECT.DIRECT_END; dir++)
        {
            for (int a = 0; a < tileCount; a++)
            {
                _forwardLUT[dir, a].firstBit = lut.Forward[dir, a].firstBit;
                _forwardLUT[dir, a].secondBit = lut.Forward[dir, a].secondBit;
            }

            for (int b = 0; b < tileCount; b++)
            {
                _backwardLUT[dir, b].firstBit = lut.Backward[dir, b].firstBit;
                _backwardLUT[dir, b].secondBit = lut.Backward[dir, b].secondBit;
            }
        }
    }
    public void SettingGrid()
    {
        cellDatas.Clear();
        for (int x = 0; x < X_GRID_SIZE; x++)
        {
            List<Cell> cells = new List<Cell>();
            for (int y = 0; y < Y_GRID_SIZE; y++)
            {
                Cell cellComponent = new Cell(new Vector2Int(x, y), TileData.Count);
                cellComponent.OnCollapsed += HandleCollapsed;
                cells.Add(cellComponent);
            }
            cellDatas.Add(cells);
        }
    }
    // 초기화 - 셀 기본 정보 설정
    public void Init()
    {
        SettingGrid();
    }


    void HandleCollapsed(Cell c)
    {
        Propagate(c.Coords);
    }
    public void ResetDomains()
    {
        int tileCount = TileData.Count;
        for (int x = 0; x < X_GRID_SIZE; x++)
        {
            for (int y = 0; y < Y_GRID_SIZE; y++)
            {
                List<int> list = cellDatas[x][y].PossibleTiles;
                list.Clear();
                for (int t = 0; t < tileCount; t++)
                {
                    list.Add(t);
                }
            }
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

        // Debug.Log($"현재 인덱스: x: {coords.x} y: {coords.y}");
    }

    public bool Propagate(Vector2Int coords)
    {
        // 처음 전파할 값
        propagateStack.Push(coords);

        while (propagateStack.Count > 0)
        {
            // 현재 기준 값
            Vector2Int cur_coord = propagateStack.Pop();
            Cell cur_cell = cellDatas[cur_coord.x][cur_coord.y];
            List<int> cur_tiles = cur_cell.PossibleTiles;


            // 상하좌우 셀 체크
            for (int i = 0; i < (int)DIRECT.DIRECT_END; i++)
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

                // 1) 리스트 -> 비트
                var selfMask = BitMaskUtils.ListToMask(cur_tiles);
                var neighborMask = BitMaskUtils.ListToMask(other_tiles);


                // 2) 정방향 지원: A(dir) -> 허용되는 B들의 집합
                // 내 실제 후보 a들에 대해 OR 누적
                CellMask forwardSupport = default(CellMask);
                for (int a = 0; a < TileData.Count; a++)
                {
                    if (BitMaskUtils.HasBit(in selfMask, a))
                    {
                        BitMaskUtils.Or(ref forwardSupport, in _forwardLUT[i, a]);
                    }
                }

                // 3) 역방향: B(rev) -> A 허용하는지
                CellMask backSupport = default(CellMask);
                for (int b = 0; b < TileData.Count; b++)
                {
                    if (!BitMaskUtils.HasBit(in neighborMask, b)) continue;

                    var allowA = _backwardLUT[i, b];
                    bool ok = ((allowA.firstBit & selfMask.firstBit) != 0UL) ||
                                ((allowA.secondBit & selfMask.secondBit) != 0UL);

                    if (ok)
                    {
                        BitMaskUtils.SetBit(ref backSupport, b);
                    }
                }

                // 4) 최종 마스크 = 기존 (교집합) 정방향 (교집합) 역방향
                var newMask = BitMaskUtils.And(in neighborMask, in forwardSupport);
                newMask = BitMaskUtils.And(in newMask, in backSupport);

                // 5) 모순 체크(후보 0개면 실패 반환)
                if (BitMaskUtils.IsZero(in newMask))
                    return false;

                // 6) 변경 시에만 리스트로 한 번에 반영 + 큐 push
                if (!BitMaskUtils.Equal(in newMask, in neighborMask))
                {
                    other_cell.PossibleTiles = BitMaskUtils.MaskToList(in newMask, TileData.Count);

                    if (!propagateStack.Contains(other_coords))
                        propagateStack.Push(other_coords);
                }
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
            while (trail.Count > mark)
            {
                var trailValue = trail.Pop();
                List<int> list = cellDatas[trailValue.cell.x][trailValue.cell.y].PossibleTiles;

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

        int pickRandIndex = Random.Range(0, cellValue.Count);
        int pick = cellValue[pickRandIndex];

        cellValue.RemoveAt(pickRandIndex);

        choices.Pop();
        choices.Push((cell, cellValue));

        if (TryAssign(cell, pick))
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
        ResetDomains(); // 시도마다 다시 리셋
        SettingTagIndex();

        ApplyLeftEdgeSurface(_hintY, _hintLeftHigh);

        // yield return StartCoroutine(LimitTagToBand_Batched("SURFACE", 0, 1));
        yield return StartCoroutine(LimitTagToBand_Batched("FILL", 0, Y_GRID_SIZE - 5));
        yield return StartCoroutine(LimitTagToBand_Batched("OBJECT_CEIL", Y_GRID_SIZE - 2, Y_GRID_SIZE - 1));
        
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

    public void StartSolve(int stitchY = 0, bool requireLeftHigh = false)
    {
        _hintY = stitchY;
        _hintLeftHigh = requireLeftHigh;
        StartCoroutine(CoSolve());
    }

    Dictionary<string, HashSet<int>> _tagTiles = new();
    private void SettingTagIndex()
    {
        if (_tagTiles.Count > 0)
            return;

        _tagTiles.Clear();
        _tagMasks.Clear();
        for (int i = 0; i < TileData.Count; i++)
        {
            List<string> tags = TileData[i]?.tags;
            if (tags == null)
            {
                continue;
            }
            foreach (var t in tags)
            {
                if (!_tagTiles.ContainsKey(t))
                {
                    _tagTiles[t] = new HashSet<int>();
                }
                _tagTiles[t].Add(i);

                if (!_tagMasks.ContainsKey(t))
                    _tagMasks[t] = default;
                if (!_tagMasks.TryGetValue(t, out var mask))
                    mask = default;

                BitMaskUtils.SetBit(ref mask, i);
                _tagMasks[t] = mask;
            }
        }
    }

    IEnumerator LimitTagToBand_Batched(string tag, int yMin, int yMax,
                                        int scanChunk = 4096,
                                        int propChunk = 128)
    {
        int W = X_GRID_SIZE, H = Y_GRID_SIZE;
        yMin = Mathf.Clamp(yMin, 0, H - 1);
        yMax = Mathf.Clamp(yMax, 0, H - 1);
        if (yMin > yMax) yield break;

        _tagTiles.TryGetValue(tag, out var tagData);

        var seeds = new List<Vector2Int>(scanChunk);
        int scanned = 0;

        // 1) 스캔
        // 후보 제거, 변경된 좌표 seeds에 추가
        for (int y = 0; y < H; y++)
        {
            bool inside = (y >= yMin && y <= yMax);
            if (inside)
            {
                continue; // 밴드 내부 값은 허용
            }

            for (int x = 0; x < W; x++)
            {
                var c = cellDatas[x][y];
                if (c.IsCollapsed)
                {
                    continue;
                }

                // 캐시 체크
                int beforeCount = c.PossibleTiles.Count;
                if (tagData != null)
                    c.PossibleTiles.RemoveAll(id => tagData.Contains(id));
                else
                    c.PossibleTiles.RemoveAll(id => TileData[id]?.tags != null && TileData[id].tags.Contains(tag));

                if (c.PossibleTiles.Count != beforeCount)
                    seeds.Add(new Vector2Int(x, y));

                if (++scanned % scanChunk == 0)
                    yield return null;

            }
            for (int i = 0; i < seeds.Count;)
            {
                int n = Mathf.Min(propChunk, seeds.Count - i);
                for (int k = 0; k < n; k++)
                    Propagate(seeds[i + k]);

                i += n;
                yield return null;
            }

            seeds.Clear();
        }
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
            if (inside)
            {
                continue; // 밴드 내부 값은 허용
            }

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