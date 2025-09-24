namespace WFC.Domain.Rules
{
    public sealed class RuleData
    {

        public int Dir; // 방향 - 0:LEFT, 1:RIGHT, 2:TOP, 3:BOTTOM
        public string SelfTag; // self 타일 태그(null, empty 가능)
        public string NeighborTag; // neighbor 타일 태그(null, empty 가능)
        public bool Allow; // 허용/비허용
    }

    public sealed class RuleSetModel
    {
        public RuleData[] Rules;
        public bool DefaultAllow;
    }
}
