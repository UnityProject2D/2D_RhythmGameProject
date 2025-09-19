using WFC.Domain.Rules;
using System.Collections.Generic;
public sealed class TileRuleCompatibilityAdapter : ICompatibility
{
    private readonly List<TileData> _tiles;
    private readonly RuleSetModel _rules;

    public TileRuleCompatibilityAdapter(List<TileData> tiles, RuleSetModel rules)
    {
        _tiles = tiles;
        _rules = rules;
    }

    public bool Allow(int selfId, int neighId, int dir)
    {
        List<string> selfTag = _tiles[selfId].tags;
        List<string> neighTag = _tiles[neighId].tags;

        return CompatibilityService.Allow(selfTag, dir, neighTag, _rules);
    }
}
