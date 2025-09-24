namespace WFC.Domain.Rules
{
    using CellMask = WFC.Domain.Core.CellDomainMask;
	public class PropagationLUT
    {
        public readonly CellMask[,] Forward;
        public readonly CellMask[,] Backward;

        public PropagationLUT(CellMask[,] forward, CellMask[,] backward)
        {
            Forward = forward;
            Backward = backward;
        }
    }
}