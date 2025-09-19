using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace WFC.Domain.Rules
{
    public static class CompatibilityService
    {
        public static bool Allow(IReadOnlyList<string> selfTags,
                                 int dir, 
                                 IReadOnlyList<string> neighborTags,
                                 RuleSetModel ruleSet)
        {
            foreach (RuleData rule in ruleSet.Rules)
            {
                if (rule.Dir != dir)
                {
                    continue;
                }
                if (!HasTag(selfTags, rule.SelfTag))
                {
                    continue;
                }
                if (!HasTag(neighborTags, rule.NeighborTag))
                {
                    continue;
                }
                return rule.Allow;
            }
            return ruleSet.DefaultAllow;
        }

        private static bool HasTag(IReadOnlyList<string> tags, string tag)
        {
            // 비었으면 매칭
            if (string.IsNullOrEmpty(tag))
            {
                return true;
            }

            if (tags == null)
            {
                return false;
            }
            return tags.Contains(tag);
        }
    }
}