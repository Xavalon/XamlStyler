// © Xavalon. All rights reserved.

using MonoDevelop.Ide.Gui;
using System;
using System.Collections.Generic;
using Xavalon.XamlStyler.Mac.Services.DocumentSavedEvent;
using Xavalon.XamlStyler.Mac.Services.XamlFormatting;
using Xavalon.XamlStyler.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Mac.Plugins.XamlFormattingOnSave
{
    public class XamlFormattingOnSavePlugin : IXamlFormattingOnSavePlugin
    {
        private readonly IDocumentSavedEventService _documentSavedEventService;
        private readonly IXamlFormattingService _xamlFormattingService;
        private readonly IXamlStylerOptionsService _xamlStylerOptionsService;

        private readonly List<Document> _currentlySavedDocuments;

        public XamlFormattingOnSavePlugin(IDocumentSavedEventService documentSavedEventService,
                                          IXamlFormattingService xamlFormattingService,
                                          IXamlStylerOptionsService xamlStylerOptionsService)
        {
            _documentSavedEventService = documentSavedEventService;
            _xamlFormattingService = xamlFormattingService;
            _xamlStylerOptionsService = xamlStylerOptionsService;

            _currentlySavedDocuments = new List<Document>();
        }

        public void Initialize()
        {
            _documentSavedEventService.DocumentSaved += OnDocumentSaved;
        }

        private void OnDocumentSaved(object sender, EventArgs e)
        {
            var document = (Document)sender;
            if (!_xamlFormattingService.IsDocumentFormattable(document))
            {
                return;
            }

            var stylerOptions = _xamlStylerOptionsService.GetDocumentOptions(document);
            if (!stylerOptions.FormatOnSave)
            {
                return;
            }

            // NOTE We already handled this document, preventing infinite save loop
            if (_currentlySavedDocuments.Contains(document))
            {
                _currentlySavedDocuments.Remove(document);
                return;
            }

            if (!_xamlFormattingService.TryFormatXamlDocument(document, stylerOptions))
            {
                return;
            }

            _currentlySavedDocuments.Add(document);
            document.Save();
        }

        public void Dispose()
        {
            _documentSavedEventService.DocumentSaved -= OnDocumentSaved;
        }
    }
}