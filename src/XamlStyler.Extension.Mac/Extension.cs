// © Xavalon. All rights reserved.

using Xavalon.XamlStyler.Extension.Mac.Plugins.XamlFormattingOnSave;
using Xavalon.XamlStyler.Extension.Mac.Services.DocumentSavedEvent;
using Xavalon.XamlStyler.Extension.Mac.Services.XamlFiles;
using Xavalon.XamlStyler.Extension.Mac.Services.XamlFormatting;
using Xavalon.XamlStyler.Extension.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Extension.Mac
{
    public class Extension
    {
        public void Initialize()
        {
            var container = Container.Instance;
            RegisterServices(container);
            RegisterPlugins(container);

            InitializeDocumentSavedLogic(container);
        }

        private void RegisterServices(Container container)
        {
            container.LazyRegisterSingleton<IXamlFormattingService, XamlFormattingService>();
            container.LazyRegisterSingleton<IXamlStylerOptionsService, XamlStylerOptionsService>();
            container.LazyRegisterSingleton<IXamlFilesService, XamlFilesService>();
            container.LazyRegisterSingleton<IDocumentSavedEventService, DocumentSavedEventService>();
        }

        private void RegisterPlugins(Container container)
        {
            container.LazyRegisterSingleton<IXamlFormattingOnSavePlugin, XamlFormattingOnSavePlugin>();
        }

        private void InitializeDocumentSavedLogic(Container container)
        {
            var documentSavedEventService = container.Resolve<IDocumentSavedEventService>();
            var xamlFormattingOnSavePlugin = container.Resolve<IXamlFormattingOnSavePlugin>();

            documentSavedEventService.StartListening();
            xamlFormattingOnSavePlugin.Initialize();
        }
    }
}