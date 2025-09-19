namespace WFC.Domain.Core
{
    public struct CellDomainMask
    {
        public ulong firstBit;      // 0 ~ 63까지 타일 후보 정보(비트 공간)
        public ulong secondBit;     // 64 ~ 127까지 타일 후보 정보(비트 공간)
    }
}