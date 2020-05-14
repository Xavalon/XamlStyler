// (c) Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Parser
{
    [Flags]
    public enum ContentTypes
    {
        None = 0,
        SingleLineTextOnly = 1,
        MultiLineTextOnly = 2,
        Mixed = 4,
    }
}