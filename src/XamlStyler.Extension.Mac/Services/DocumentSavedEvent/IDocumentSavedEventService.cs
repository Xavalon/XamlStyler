// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Extension.Mac.Services.DocumentSavedEvent
{
    public interface IDocumentSavedEventService
    {
        event EventHandler DocumentSaved;

        bool IsListening { get; }

        void StartListening();

        void StopListening();
    }
}