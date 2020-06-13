// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Model
{
    public class AttributeOrderRules
    {
        private readonly IList<AttributeOrderRule> rules;

        public AttributeOrderRules(IStylerOptions options)
        {
            this.rules = new List<AttributeOrderRule>();

            var groupIndex = 1;
            foreach (var @group in options.AttributeOrderingRuleGroups)
            {
                if (!string.IsNullOrWhiteSpace(@group))
                {
                    int priority = 1;

                    string[] names = @group.Split(',')
                        .Where(_ => !String.IsNullOrWhiteSpace(_))
                        .Select(_ => _.Trim())
                        .ToArray();

                    foreach (var name in names)
                    {
                        this.rules.Add(new AttributeOrderRule(name, groupIndex, priority));
                        priority++;
                    }
                }

                groupIndex++;
            }

            // Add catch all group at the end ensuring we always get a match;
            this.rules.Add(new AttributeOrderRule("*", groupIndex, 0));
        }

        public AttributeOrderRule GetRuleFor(string attributeName)
        {
            return this.rules
                .Where(_ => _.Name.IsMatch(attributeName))
                .OrderByDescending(_ => _.MatchScore)
                .FirstOrDefault();
        }
    }
}