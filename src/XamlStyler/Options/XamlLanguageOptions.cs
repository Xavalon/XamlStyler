// (c) Xavalon. All rights reserved.

using System.Collections.Generic;

namespace Xavalon.XamlStyler.Options
{
    public class XamlLanguageOptions
    {
        public bool IsFormatable { get; set; }

        public HashSet<char> UnescapedAttributeCharacters { get; } = new HashSet<char>();
    }
}