using WFC.Domain.Core;
using WFC.Domain.Contracts;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
namespace WFC.Infrastructure
{
    // 전파 계산(정/역방향)을 잡으로 수행하는 구현체
    // DDD에서 Infrastructure에 속함(프레임워크 의존성 포함)
    // Domain.Contracts(IPropagationSolver)만 만족시키면 WaveFunction
    public sealed class JobPropagationSolver : IPropagationSolver, System.IDisposable
    {
        private readonly int _tileCount;
        private readonly int _dirCount;
        private NativeArray<CellDomainMask> _fwd;
        private NativeArray<CellDomainMask> _bwd;

        public JobPropagationSolver(CellDomainMask[, ] forwardLut,
                                    CellDomainMask[, ] backwardLut,
                                    int tileCount, int dirCount)
        {
            _tileCount = tileCount;
            _dirCount = dirCount;

            // persistent: 생성/객체 파괴까지 1회 할당/재사용(할당/Dispose 오버헤드 최소화)
            _fwd = new NativeArray<CellDomainMask>(_dirCount * _tileCount, Allocator.Persistent);
            _bwd = new NativeArray<CellDomainMask>(_dirCount * _tileCount, Allocator.Persistent);

            // 2차원 -> 1차원: 잡에서 연속 메모리 접근
            for (int d = 0; d < _dirCount; d++)
            {
                for (int t = 0; t < _tileCount; t++)
                {
                    _fwd[d * _tileCount + t] = forwardLut[d, t];
                    _bwd[d * _tileCount + t] = backwardLut[d, t];
                }
            }
        }
        public bool TrySolve(int dir, in CellDomainMask selfMask, in CellDomainMask neighborMask, out CellDomainMask outNewMask)
        {
            var outArr = new NativeArray<CellDomainMask>(1, Allocator.TempJob);
            var okArr = new NativeArray<byte>(1, Allocator.TempJob);

            var job = new PairJob
            {
                Fwd = _fwd,
                Bwd = _bwd,
                Dir = dir,
                TileCount = _tileCount,
                SelfMask = selfMask,
                NeighborMask = neighborMask,
                OutNewMask = outArr,
                Success = okArr
            };

            job.Schedule().Complete();
            bool pass = okArr[0] == 1;
            outNewMask = pass ? outArr[0] : default;

            outArr.Dispose();
            okArr.Dispose();
            return pass;
        }
        // 네이티브 메모리 해제
        // 컴포넌트 파괴
        // 씬 종료 시 호출
        public void Dispose()
        {
            if (_fwd.IsCreated)
            {
                _fwd.Dispose();
            }
            if (_bwd.IsCreated)
            {
                _bwd.Dispose();
            }
        }

        [BurstCompile]
        private struct PairJob : IJob 
        {
            [ReadOnly] public NativeArray<CellDomainMask> Fwd;
            [ReadOnly] public NativeArray<CellDomainMask> Bwd;

            public int Dir;
            public int TileCount;

            public CellDomainMask SelfMask;
            public CellDomainMask NeighborMask;

            public NativeArray<CellDomainMask>  OutNewMask;
            public NativeArray<byte> Success;

            public void Execute()
            {
                // 정방향: self 후보 a 각자 허용 B를 OR로 누적
                CellDomainMask forwardSupport = default;        // 초기값 0(모든 비트 off)
                int baseIdx = Dir * TileCount;                  // 현재 방향의 LUT 시작 인덱스
                for (int t = 0; t < TileCount; t++)
                {
                    if (!BitMaskUtils.HasBit(in SelfMask, t)) // a가 self 후보가 아니면 스킵
                    {
                        continue;
                    }

                    CellDomainMask support = Fwd[baseIdx + t]; // a가 허용하는 B들의 비트셋
                    BitMaskUtils.Or(ref forwardSupport, in support); // 누적 OR
                }

                // 역방향: neighbor 후보 b 중, a들과 서로 허용 관계인 b만 남기기
                CellDomainMask backSupport = default;        // 초기값 0(모든 비트 off)
                for (int t = 0; t < TileCount; t++)
                {
                    if (!BitMaskUtils.HasBit(in NeighborMask, t)) // b가 원래 후보가 아니면 스킵
                    {
                        continue;
                    }
                    // b가 허용하는 A와 실제 SELF 후보 A가 한 비트라도 겹치면 true
                    var allowA = Bwd[baseIdx + t];
                    bool pass = ((allowA.firstBit & SelfMask.firstBit) != 0UL) ||
                                ((allowA.secondBit & SelfMask.secondBit) != 0UL);
                    if (pass) BitMaskUtils.SetBit(ref backSupport, t);
                }

                var newMask = BitMaskUtils.And(in NeighborMask, in forwardSupport);
                newMask = BitMaskUtils.And(in newMask, in backSupport);

                // 모순 처림: 후보 0개면 실패 플래그, 아니면 결과 기록
                if (BitMaskUtils.IsZero(in newMask)) { Success[0] = 0; return; }
                OutNewMask[0] = newMask;
                Success[0] = 1;
            }
        }
    }

}
