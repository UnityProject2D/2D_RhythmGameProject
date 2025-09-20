using WFC.Domain.Contracts;
using WFC.Domain.Core;


namespace WFC.Infrastructure
{
    public sealed class CpuPropagationSolver : IPropagationSolver
    {
        private readonly CellDomainMask[,] _forwardLUT;
        private readonly CellDomainMask[,] _backwardLUT;
        private readonly int _tileCount;
        public CpuPropagationSolver(CellDomainMask[, ] forward, CellDomainMask[,] backward, int tileCount)
        {
            _forwardLUT = forward;
            _backwardLUT = backward;
            _tileCount = tileCount;
        }

        public bool TrySolve(int dir, in CellDomainMask selfMask, in CellDomainMask neighborMask, out CellDomainMask outNewMask)
        {
            // 1) 정방향 지원: A(dir) -> 허용되는 B들의 집합
            // 내 실제 후보 a들에 대해 OR 누적
            CellDomainMask forwardSupport = default;
            for (int a = 0; a < _tileCount; a++)
            {
                if (BitMaskUtils.HasBit(in selfMask, a))
                {
                    BitMaskUtils.Or(ref forwardSupport, in _forwardLUT[dir, a]);
                }
            }

            // 2) 역방향: B(rev) -> A 허용하는지
            CellDomainMask backSupport = default;
            for (int b = 0; b < _tileCount; b++)
            {
                if (!BitMaskUtils.HasBit(in neighborMask, b)) continue;

                var allowA = _backwardLUT[dir, b];
                bool ok = ((allowA.firstBit & selfMask.firstBit) != 0UL) ||
                            ((allowA.secondBit & selfMask.secondBit) != 0UL);

                if (ok)
                {
                    BitMaskUtils.SetBit(ref backSupport, b);
                }
            }

            // 3) 최종 마스크 = 기존 (교집합) 정방향 (교집합) 역방향
            var newMask = BitMaskUtils.And(in neighborMask, in forwardSupport);
            newMask = BitMaskUtils.And(in newMask, in backSupport);

            // 4) 모순 체크(후보 0개면 실패 반환)
            if (BitMaskUtils.IsZero(in newMask))
            {
                outNewMask = default;
                return false;
            }

            outNewMask = newMask;
            return true;
        }

    }
}