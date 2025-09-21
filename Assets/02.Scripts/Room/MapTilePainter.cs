using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTilePainter : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap _visualTilemap;
    [SerializeField]
    private Tilemap _groundColliderTilemap;

    [Header("GridSize")]
    public int X_GRID_SIZE;
    public int Y_GRID_SIZE;

    [Header("Surface Tag")]
    [SerializeField]
    private string surfaceTag = "SURFACE";

    // 캐시 타일 데이터
    [SerializeField]
    private Dictionary<int, Tile> _visualTileCache = new Dictionary<int, Tile>();
    private Dictionary<int, Tile> _colliderTileCache = new Dictionary<int, Tile>();
    public void SettingTileMap(List<TileData> tiles, List<List<Cell>> cells)
    {
        for (int x = 0; x < X_GRID_SIZE; x++)
        {
            for (int y = 0; y < Y_GRID_SIZE; y++)
            {
                PaintCell(new Vector3Int(x, y, 0), tiles, cells[x][y]);
            }
        }        
    }

    private void PaintCell(Vector3Int pos, List<TileData> tiles, Cell cell)
    {
        // 미결정 셀 비우기
        if (!GetCollapsedId(cell, out int id))
        {
            ResetTile(pos);
            return;
        }

        TileData data = tiles[id];

        // 꾸미기용 타일
        _visualTilemap.SetTile(pos, GetVisualTile(id, data));

        // 충돌 타일(그라운드)
        if (IsGroundTile(data))
        {
            _groundColliderTilemap.SetTile(pos, GetColliderTile(id, data));
        }
        else
        {
            _groundColliderTilemap.SetTile(pos, null);
        }
    }

    private bool GetCollapsedId(Cell cell, out int id)
    {
        if (cell.PossibleTiles.Count == 1)
        {
            id = cell.PossibleTiles[0];
            return true;
        }
        id = -1;
        return false;
    }
    private void ResetTile(Vector3Int pos)
    {
        _visualTilemap.SetTile(pos, null);
        _groundColliderTilemap.SetTile(pos, null);
    }

    private bool IsGroundTile(TileData tileData)
    {
        return tileData.tags != null && (tileData.tags.Contains(surfaceTag));
    }

    private Tile GetVisualTile(int id, TileData tileData)
    {
        if (_visualTileCache.TryGetValue(id, out var tile))
        {
            return tile;
        }
        tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = tileData.Sprite;
        _visualTileCache[id] = tile;
        return tile;
    }

    private Tile GetColliderTile(int id, TileData tileData)
    {
        if (_colliderTileCache.TryGetValue(id, out var tile))
        {
            return tile;
        }
        tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = tileData.Sprite;
        tile.colliderType = Tile.ColliderType.Sprite;
        _colliderTileCache[id] = tile;
        return tile;
    }

}