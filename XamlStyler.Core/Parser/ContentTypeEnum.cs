// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Core.Parser
{
    [Flags]
    public enum ContentTypeEnum
    {
        // ReSharper disable InconsistentNaming
        NONE = 0,

        SINGLE_LINE_TEXT_ONLY = 1,
        MULTI_LINE_TEXT_ONLY = 2,
        MIXED = 4
        // ReSharper restore InconsistentNaming
    }
}