using System;
using System.Collections.Generic;
using System.Linq;
using XamlStyler.Core.Options;

namespace XamlStyler.Core.Model
{
    public class AttributeOrderRules
    {
        private readonly IList<AttributeOrderRule> _rules;

        public AttributeOrderRules(IStylerOptions options)
        {
            _rules = new List<AttributeOrderRule>();

            var groupIndex = 1;
            foreach (var @group in options.AttributeOrderingRuleGroups)
            {
                if (!string.IsNullOrWhiteSpace(@group))
                {
                    int priority = 1;

                    string[] names = @group.Split(',')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .ToArray();

                    foreach (var name in names)
                    {
                        _rules.Add(new AttributeOrderRule(name, groupIndex, priority));
                        priority++;
                    }
                }
                groupIndex++;
            }

            // Add catch all group at the end ensuring we always get a match;
            _rules.Add(new AttributeOrderRule("*",groupIndex,0));
        }

        public AttributeOrderRule GetRuleFor(string attributeName)
        {
            return _rules
                .Where(x => x.Name.IsMatch(attributeName))
                .OrderByDescending(x => x.MatchScore)
                .FirstOrDefault();
        }
    }
}