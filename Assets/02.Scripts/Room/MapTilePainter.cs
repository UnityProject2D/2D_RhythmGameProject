using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTilePainter : MonoBehaviour
{
    public Tilemap TileMap;
    public int GridSize;

    public void SettingTileMap(List<TileData> tileDataList, List<List<Cell>> cellDataList)
    {
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                Tile tileData = ScriptableObject.CreateInstance<Tile>();
                if (cellDataList[x][y].PossibleTiles.Count == 1)
                {
                    tileData.sprite = tileDataList[cellDataList[x][y].PossibleTiles[0]].Sprite;
                    TileMap.SetTile(new Vector3Int(x, y, 0), tileData);
                }
                else
                {
                    TileMap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }        
    }
}