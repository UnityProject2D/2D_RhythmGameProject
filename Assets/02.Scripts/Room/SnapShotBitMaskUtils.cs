using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WfcSnapshot;
using CellMask = WfcSnapshot.cellMask;

public static class SnapShotBitMaskUtils
{
    public static void SetBit(ref cellMask mask, int tileIndex)
    {
        if (tileIndex < 64)
            mask.firstBit |= (1UL << tileIndex);
        else
            mask.secondBit |= (1UL << (tileIndex - 64));
    }
    public static void ClearBit(ref cellMask mask, int tileIndex)
    {
        if (tileIndex < 64)
            mask.firstBit &= ~(1UL << tileIndex);
        else
            mask.secondBit &= ~(1UL << (tileIndex - 64));
    }

    /// 특정 비트(tileIndex)가 켜져있는지 확인
    public static bool HasBit(in CellMask mask, int tileIndex)
    {
        // 켜져있을 경우 true 반환
        if (tileIndex < 64)
            return (mask.firstBit & (1UL << tileIndex)) != 0;
        else
            return (mask.secondBit & (1UL << (tileIndex - 64))) != 0;
    }

    // 두 마스크가 같은지 비교
    public static bool Equal(in CellMask maskA, in CellMask maskB)
    {
        return ((maskA.firstBit == maskB.firstBit)
            && (maskA.secondBit == maskB.secondBit));
    }

    // 후보가 아예 없는 상태인지(둘 다 0인지)
    public static bool IsZero(in CellMask mask)
    {
        return (mask.firstBit == 0UL) && (mask.secondBit == 0UL);
    }

    // 리스트 -> 비트 마스크 변환
    public static CellMask ListToMask(IReadOnlyList<int> list)
    {
        CellMask mask = default;
        for (int i = 0; i < list.Count; i++)
        {
            int t = list[i];
            if (t < 0 || t > 128)
                continue;
            SetBit(ref mask, list[i]); // 기존 스냅챗 유틸 사용
        }
        return mask;
    }

    // 비트 마스크 -> 리스트 변환
    public static List<int> ListToMask(in CellMask mask, int tileCount)
    {
        List<int> list = new List<int>(tileCount);
        int limitCount = tileCount < 128 ? tileCount : 128;

        for (int i = 0; i < limitCount; i++)
        {
            if (HasBit(in mask, tileCount)) list.Add(tileCount);
        }
        return list;
    }
}
