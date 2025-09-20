using System;
using UnityEngine;
using System.Collections.Generic;
using WFC.Domain.Core;

namespace WFC.Domain.Policy
{
    public static class EdgePolicies
    {
        public sealed class EdgePolicyContext
        {
            public int GridHeight;  // Y_GRID_SIZE
            public int TileCount;   // TileData.Count

            // ------------------ 포트들 ------------------

            // 셀 접근
            public Func<int, int, List<int>> GetCandidates;         // (x, y) -> 후보 리스트
            public Action<int, int, List<int>> SetCandidates;       // (x, y, newList)

            // 태그 마스크
            public Func<string, CellDomainMask> GetTagMask;         // SURFACE, LEFT=HIGH,LOW

            // 전파
            public Func<int, int, bool> Propagate;                  // 좌표(x, y) 성공여부

        }

        public static bool ApplyLeftEdgeSurface(EdgePolicyContext ctx, int y, bool leftHigh)
        {
            if (y < 0 || y >= ctx.GridHeight)
            {
                return false;
            }

            // 후보 리스트 가져오기
            List<int> currentFunc = ctx.GetCandidates(0, y);

            // 확정
            if (currentFunc.Count == 1)
                return true;

            // 특정 태그 가져오기
            CellDomainMask surface = ctx.GetTagMask("SURFACE"); // SURFACE 마스크
            CellDomainMask edge = ctx.GetTagMask(leftHigh ? "LEFT=HIGH" : "LEFT=LOW"); // LEFT=HIGH/LOW 마스크

            CellDomainMask newMask = BitMaskUtils.And(in surface, in edge); // 두 마스크 교집합

            // 0일 경우 종료
            if (BitMaskUtils.IsZero(in newMask)) // 교집합 없음
            {
                return false; // 실패(고정 불가능)
            }

            List<int> newList = BitMaskUtils.MaskToList(in newMask, ctx.TileCount); // 비트->후보 리스트
            ctx.SetCandidates(0, y, newList); // (0, y) 후보를 새 리스트로 변경
            return ctx.Propagate(0, y); // 전파 시작 & 결과 반환
        }
    }
}