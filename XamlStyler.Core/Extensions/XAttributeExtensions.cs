// © Xavalon. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.Extensions
{
    public static class XAttributeExtensions
    {
        public static bool Matches(this XAttribute self, XAttribute attribute)
        {
            return self.ToString().Equals(attribute.ToString());
        }

        public static bool MatchesAny(this XAttribute self, XAttribute[] attributes)
        {
            return attributes.Any(_ => _.Matches(self));
        }

        public static XAttribute FindAttribute(this IEnumerable<XAttribute> self, string key)
        {
            return self.Where(_ => (_.Name.LocalName.Equals(key))).FirstOrDefault();
        }
    }
}
