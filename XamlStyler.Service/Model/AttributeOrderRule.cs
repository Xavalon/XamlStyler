using System.Linq;
using XamlStyler.Core.Reorder;

namespace XamlStyler.Core.Model
{
    public class AttributeOrderRule
    {
        public Wildcard Name { get; }
        public int Group { get; }
        public int Priority { get; }
        public int MatchScore { get; }

        public AttributeOrderRule(string name, int group, int priority)
        {
            Name = new Wildcard(name);
            Group = group;
            Priority = priority;
            // Calculate match score. 1=no wildcards 0:contains ? -1:contains *
            MatchScore = name.Any(x => x == '*') ? -1 : name.Any(x => x == '?') ? 0 : 1;
        }
    }
}