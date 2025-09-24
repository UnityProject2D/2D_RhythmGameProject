namespace WFC.Domain.Rules
{
    public interface ICompatibility
    {
        bool Allow(int selfTileId, int neighborTileId, int dir);
    }
}