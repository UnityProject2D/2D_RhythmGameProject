using WFC.Domain.Core;
using static WaveFunction;
using CellMask = WFC.Domain.Core.CellDomainMask;

namespace WFC.Domain.Rules
{
    public static class PropagationLUTBuilder
    {
        public static PropagationLUT Build(int tileCount, ICompatibility rules)
        {
            CellMask[,] forwardLUT = new CellMask[(int)DIRECT.DIRECT_END, tileCount];
            CellMask[,] backwardLUT = new CellMask[(int)DIRECT.DIRECT_END, tileCount];

            for (int dir = 0; dir < (int)DIRECT.DIRECT_END; dir++)
            {
                int rev = dir ^ 1;
                for (int a = 0; a < tileCount; a++)
                {
                    var fMask = default(CellMask);
                    for (int neigh = 0; neigh < tileCount; neigh++)
                    {
                        // Forward: A(dir) -> neigh 허용
                        if (rules.Allow(a, neigh, dir))
                            BitMaskUtils.SetBit(ref fMask, neigh);

                        // Backward: neigh(rev) -> A 허용
                        if (rules.Allow(neigh, a, rev))
                            BitMaskUtils.SetBit(ref backwardLUT[dir, neigh], a);
                    }
                    forwardLUT[dir, a] = fMask;
                }
            }
            return new PropagationLUT(forwardLUT, backwardLUT);
        }
    }
}