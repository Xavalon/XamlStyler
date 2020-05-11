// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Extension.Mac.Plugins.XamlFormattingOnSave
{
    public interface IXamlFormattingOnSavePlugin : IDisposable
    {
        void Initialize();
    }
}