using System.Linq;
using UnityEngine;
using WFC.Domain.Rules;
public static class RuleSetMapper
{
    public static RuleSetModel ToDomain(TileRuleSetData ruleSetData)
    {
        return new RuleSetModel
        {
            Rules = ruleSetData.rules.Select(r => new RuleData
            {
                Dir         = (int)r.dir,
                SelfTag     = r.selfTag,
                NeighborTag = r.neighborTag,
                Allow       = r.allow
            }).ToArray(),
            DefaultAllow = ruleSetData.defaultAllow
        };
    }
}
