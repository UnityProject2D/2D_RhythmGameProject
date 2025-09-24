using WFC.Domain.Core;

namespace WFC.Domain.Contracts
{
    // 전파 시 (self, neighbor) 마스크와 방향을 받아 이웃의 새로운 마스크 계산
    public interface IPropagationSolver
    {
        bool TrySolve(
            int dir,
            in CellDomainMask selfMask,
            in CellDomainMask neighborMask,
            out CellDomainMask outNewMask
            );
    }

}
