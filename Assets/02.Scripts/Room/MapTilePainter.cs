using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTilePainter : MonoBehaviour
{
    public Tilemap TileMap;

    public void SettingTileMap(List<TileData> tileDataList, List<List<Cell>> cellDataList)
    {
        for (int i = 0; i < WaveFunction.GRID_SIZE; i++)
        {
            for (int j = 0; j < WaveFunction.GRID_SIZE; j++)
            {
                Tile tileData = ScriptableObject.CreateInstance<Tile>();
                tileData.sprite = tileDataList[cellDataList[i][j].PossibleTiles[0]].Sprite;
                TileMap.SetTile(new Vector3Int(i, j, 0), tileData);
            }
        }        
    }
}