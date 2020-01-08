// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Mac.Plugins.XamlFormattingOnSave
{
    public interface IXamlFormattingOnSavePlugin : IDisposable
    {
        void Initialize();
    }
}