using UnityEngine;
using System.Collections.Generic;

// 잡, 버스트는 List 같은 것을 쓰지 못함.
// 그래서 숫자 비트로 압축한 스냅샷(복사본)을 만들어 읽기 전용 데이터로 던져주는 것
// 비트가 1이면 해당 후보가 가능하다는 것
public class WfcSnapshot : MonoBehaviour
{
    public struct cellMask
    {
        public ulong firstBit;      // 0 ~ 63까지 타일 후보 정보(비트 공간)
        public ulong secondBit;     // 64 ~ 127까지 타일 후보 정보(비트 공간)
    }

    public sealed class BoardSnap
    {
        public int W;
        public int H;
        public int tileCount;
        public cellMask[] cells; // 길이 = W * H, 인덱스 = y * W + x
    }

    static int GetIdx(int x, int y, int width) => y * width + x;

    /// <summary>
    /// 현재 보드의 후보들을 비트마스크로 압축해서 스냅샷 생성
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="tileCount"></param>
    /// <returns></returns>
    public static BoardSnap Capture(List<List<Cell>> grid, int width, int height, int tileCount)
    {
        // 방어 코드: 현재 0 ~ 127까지만 지원(ulong 2개)
        if (tileCount < 0 || tileCount > 128)
        {
            Debug.LogError("error!-tileCount는 0~127까지만 지원");
            return null;
        }

        BoardSnap snap = new BoardSnap
        {
            W = width,
            H = height,
            tileCount = tileCount,
            cells = new cellMask[width * height]
        };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int idx = GetIdx(x, y, width);

                List<int> list = grid[x][y].PossibleTiles;

                cellMask m = default(cellMask);

                // 후보 만들기 - bit On
                for (int i = 0; i < list.Count; i++)
                {
                    int t = list[i];

                    // 범위 벗어난 후보 스킵
                    if (t < 0 || t >= tileCount)
                    {
                        continue;
                    }

                    SnapShotBitMaskUtils.SetBit(ref m, t);
                }
                snap.cells[idx] = m;
            }
        }

        return snap;
    }

}
