// (c) Xavalon. All rights reserved.

using System;
using System.Linq;
using Xavalon.XamlStyler.DocumentManipulation;

namespace Xavalon.XamlStyler.Model
{
    public class AttributeOrderRule
    {
        public Wildcard Name { get; }

        public int Group { get; }

        public int Priority { get; }

        public int MatchScore { get; }

        public AttributeOrderRule(string name, int group, int priority)
        {
            this.Name = new Wildcard(name);
            this.Group = group;
            this.Priority = priority;

            // Calculate match score.  
            // -2 = Catch-all ("*" or "*:*")  
            // -1 = Contains '*'  
            //  0 = Contains '?'  
            //  1 = No Wildcards  
            this.MatchScore = (name.Equals("*", StringComparison.Ordinal) || name.Equals("*:*", StringComparison.Ordinal))
                ? -2
                : name.Any(_ => (_ == '*'))
                    ? -1
                    : name.Any(_ => _ == '?')
                        ? 0
                        : 1;
        }
    }
}